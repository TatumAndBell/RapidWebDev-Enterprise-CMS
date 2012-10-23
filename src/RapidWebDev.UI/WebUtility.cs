/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Eunge, Legal Name: Jian Liu, Email: eunge.liu@RapidWebDev.org

	The GNU Library General Public License (LGPL) used in RapidWebDev is 
	intended to guarantee your freedom to share and change free software - to 
	make sure the software is free for all its users.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Library General Public License (LGPL) for more details.

	You should have received a copy of the GNU Library General Public License (LGPL)
	along with this program.  
	If not, see http://www.rapidwebdev.org/Content/ByUniqueKey/OpenSourceLicense
 ****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using RapidWebDev.Common;
using RapidWebDev.Common.Globalization;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI
{
    /// <summary>
    /// Web utility class
    /// </summary>
    public static class WebUtility
    {
        private static readonly Regex regexToPickVariables = new Regex(@"\$[^\$]+\$", RegexOptions.Compiled);
		private static readonly Regex regexToPickValidControlIdCharacters = new Regex(@"[a-z0-9_]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>
		/// The separator to combine the array items into a string. "+++"
		/// </summary>
		public static readonly string ARRAY_ITEM_SEPARATOR = "+++";

		/// <summary>
		/// The httppost parameter prefix name of query field control. "QFCIndex"
		/// </summary>
		public static readonly string QUERY_FIELD_CONTROL_POST_PREFRIX_NAME = "QFCIndex";

        /// <summary>
        /// Get page URL for Page Not Found.
        /// </summary>
        public static string PageNotFoundUrl { get { return Kit.ResolveAbsoluteUrl("~/PageNotFound.aspx"); } }

        /// <summary>
        /// Get page URL for Internal Server Error.
        /// </summary>
		public static string InternalServerError { get { return Kit.ResolveAbsoluteUrl("~/InternalServerError.aspx"); } }

        /// <summary>
        /// Get page URL for Not Authorized page.
        /// </summary>
		public static string NotAuthorizedUrl { get { return Kit.ResolveAbsoluteUrl("~/NotAuthorized.aspx"); } }

		/// <summary>
		/// Generate JavaScript variable name registered to client for accessing control value.
		/// </summary>
		/// <param name="controlId"></param>
		/// <returns></returns>
		public static string GenerateVariableName(string controlId)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}_Variable", controlId);
		}

		/// <summary>
		/// Encode JavaScript string replace single-quotation-mark with "\u0027" and double-quotation-mark with "\u0022".
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string EncodeJavaScriptString(string s)
		{
			if (string.IsNullOrEmpty(s)) return "";
			return s.Replace("'", "\\u0027").Replace("\"", "\\u0022");
		}

		/// <summary>
		/// Redirect to specified page url.
		/// </summary>
		/// <param name="pageUrl"></param>
		public static void RedirectToPage(string pageUrl)
		{
			string currentUrl = HttpContext.Current.Request.Url.ToString();
			string redirectPageUrl = pageUrl;
			if (!redirectPageUrl.Contains('?'))
				redirectPageUrl += "?";
			else
				redirectPageUrl += "&";

			string.Format("{0}returnedurl={1}", pageUrl, HttpContext.Current.Server.UrlEncode(currentUrl));
			HttpContext.Current.Response.Redirect(redirectPageUrl);
		}

        /// <summary>
        /// Resolve dynamic page url of workshop by specify object id and query string parameters.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="queryStrings"></param>
        /// <returns></returns>
        public static string ResolveDynamicPageUrl(string objectId, IDictionary<string, string> queryStrings)
        {
            StringBuilder pageUrl = new StringBuilder();
			pageUrl.Append(Kit.ResolveAbsoluteUrl(string.Format(CultureInfo.InvariantCulture, "~/{0}/DynamicPage.svc?objectid={0}", objectId)));
            if (queryStrings != null && queryStrings.Count > 0)
            {
                foreach (string queryStringKey in queryStrings.Keys)
                    pageUrl.AppendFormat("&{0}={1}", Kit.UrlEncode(queryStringKey), Kit.UrlEncode(queryStrings[queryStringKey]));
            }

            return pageUrl.ToString();
        }

        /// <summary>
        /// Redirect back to previous page.
        /// </summary>
        public static void BackToPreviousPage()
        {
            string pageUrl = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request["returnedurl"]);
            HttpContext.Current.Response.Redirect(pageUrl);
        }

        /// <summary>
		/// Popup an browser window (920, 600) to render the page by URL.
        /// </summary>
        /// <param name="pageUrl"></param>
        public static void OpenWindow(string pageUrl)
        {
            OpenWindow(pageUrl, 920, 600);
        }

        /// <summary>
		/// Popup an browser window to render the page by URL.
        /// </summary>
        /// <param name="pageUrl">web page url</param>
		/// <param name="width">width of popup window</param>
		/// <param name="height">height of popup window</param>
        public static void OpenWindow(string pageUrl, int width, int height)
        {
            string javascript = GetJavaScriptToOpenWindow(pageUrl, width.ToString(), height.ToString(), false);
            Page webpage = HttpContext.Current.Handler as Page;
            ScriptManager scriptManager = ScriptManager.GetCurrent(webpage);
            if (scriptManager != null && scriptManager.IsInAsyncPostBack)
            {
                ScriptManager.RegisterClientScriptBlock(webpage, webpage.GetType(), webpage.GetType().FullName, javascript, true);
            }
            else
            {
                webpage.ClientScript.RegisterClientScriptBlock(webpage.GetType(), webpage.GetType().FullName, javascript, true);
            }
        }

        /// <summary>
        /// Generate javascript to popup webpage at specified page url.
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
		/// <param name="scrollBars"></param>
        /// <returns></returns>
        public static string GetJavaScriptToOpenWindow(string pageUrl, string width, string height, bool scrollBars)
        {
            return string.Format("var left = (window.screen.availWidth-10-{1}) / 2; var top = (window.screen.availHeight-30-{2}) / 2;window.open('{0}', '', 'captionbar=no,toolbar=no,scrollbars={3},width={1},height={2}, top=' + top + ', left= ' + left, '')", pageUrl, width, height, scrollBars ? "yes" : "no");
        }

        /// <summary>
        /// Registers a control for asynchronous postbacks in ASP.NET ScriptManager.
        /// </summary>
        /// <param name="control"></param>
        public static void RegisterAsyncPostBackControl(Control control)
        {
            Page webpage = HttpContext.Current.Handler as Page;
            if (webpage == null) return;

            if (control is INamingContainer || control is IPostBackDataHandler || control is IPostBackEventHandler)
            {
                ScriptManager scriptManager = ScriptManager.GetCurrent(webpage);
                if (scriptManager != null) scriptManager.RegisterAsyncPostBackControl(control);
            }
        }

        /// <summary>
        /// Redirect the page container to the URL if the current page is in an iFrame, otherwise redirect the current page directly to the URL.
        /// </summary>
        /// <param name="absolutePageUrl"></param>
        public static void NavigatePageFrameTo(string absolutePageUrl)
        {
			string javascript = string.Format(CultureInfo.InvariantCulture, "if(window.parent!=null) window.parent.location.href = '{0}'; else window.location.href = '{0}'", absolutePageUrl);
			ClientScripts.RegisterScriptBlock(javascript);
        }

		/// <summary>
		/// Find sub controls in specified container by control id.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="controlId"></param>
		/// <returns></returns>
		public static Control FindControl(Control container, string controlId)
		{
			Control ctrl = container.FindControl(controlId);
			if (ctrl == null && container.Controls.Count == 1)
				ctrl = container.Controls[0].FindControl(controlId);

			if (ctrl == null)
				ctrl = container.Controls[0].Controls[1].Controls[0].FindControl(controlId);

			if (ctrl == null)
				ctrl = container.Controls[0].Controls[0].Controls[0].FindControl(controlId);

			if (ctrl == null)
				ctrl = FindControlRecursively(container.Controls.Cast<Control>(), controlId);

			return ctrl;
		}

		/// <summary>
		/// Find all fields with BindingAttribute marked in "target". 
		/// Then try to find control specified in BindingAttribute.ParentControlPath  in "formControl". 
		/// Finally set the found control back to the field of "target".
		/// </summary>
		/// <param name="formControl"></param>
		/// <param name="target"></param>
		public static void SetControlsByBindingAttribute(Control formControl, object target)
		{
			Type dynamicComponentType = target.GetType();
			FieldInfo[] fields = dynamicComponentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields)
			{
				object[] bindings = field.GetCustomAttributes(typeof(BindingAttribute), true);
				if (bindings == null || bindings.Length == 0) continue;

				// Last attribute overrides all before
				BindingAttribute binding = bindings[bindings.Length - 1] as BindingAttribute;

				Control foundControl = null;
				if (Kit.IsEmpty(binding.ParentControlPath))
				{
					foundControl = formControl.FindControl(field.Name);
				}
				else
				{
					string[] parentControlIds = binding.ParentControlPath.Split(new string[] { @"\", "/" }, StringSplitOptions.RemoveEmptyEntries);
					Control parentControl = formControl;
					foreach (string parentControlId in parentControlIds)
					{
						if (parentControl == null)
							throw new InvalidProgramException(string.Format(Resources.DP_DetailPanel_InvalidParentControlPath, binding.ParentControlPath));

						parentControl = parentControl.FindControl(parentControlId);
					}

					if (parentControl != null)
						foundControl = parentControl.FindControl(field.Name);
				}

				if (foundControl != null)
					field.SetValue(target, foundControl);
			}
		}

		/// <summary>
		/// Make all controls declared in target as data members which are marked with BindingAttribute to be readonly.
		/// </summary>
		/// <param name="target"></param>
		public static void MakeBindingControlsReadOnly(object target)
		{
			Kit.NotNull(target, "target");

			Type dynamicComponentType = target.GetType();
			FieldInfo[] fields = dynamicComponentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields)
			{
				object[] bindings = field.GetCustomAttributes(typeof(BindingAttribute), true);
				if (bindings == null || bindings.Length == 0) continue;

				object control = field.GetValue(target);
				if (control == null) continue;

				// check WebControls
				bool isEditableControl = control is ITextControl || control is ICheckBoxControl;
				// check HtmlControls
				isEditableControl = isEditableControl || control is HtmlInputText || control is HtmlSelect || control is HtmlInputCheckBox || control is HtmlInputRadioButton || control is HtmlTextArea;
				if (!isEditableControl) continue;

				bool setReadOnly = false;
				Type controlType = control.GetType();
				PropertyInfo readonlyProperty = controlType.GetProperty("ReadOnly", BindingFlags.Public | BindingFlags.Instance);
				if (readonlyProperty != null)
				{
					readonlyProperty.SetValue(control, true, null);
					setReadOnly = true;
				}

				if (!setReadOnly)
				{
					PropertyInfo enabledProperty = controlType.GetProperty("Enabled", BindingFlags.Public | BindingFlags.Instance);
					if (enabledProperty != null)
					{
						enabledProperty.SetValue(control, false, null);
						setReadOnly = true;
					}
				}
				
				if(!setReadOnly)
				{
					PropertyInfo attributesProperty = controlType.GetProperty("Attributes", BindingFlags.Public | BindingFlags.Instance);
					if (attributesProperty != null)
					{
						object attributes = attributesProperty.GetValue(control, null);
						Type attributesType = attributes.GetType();
						PropertyInfo indexerProperty = attributesType.GetProperty("Item");
						if (indexerProperty != null)
						{
							indexerProperty.SetValue(attributes, "true", new object[] { "disabled" });
							setReadOnly = true;
						}
					}
				}

				if (setReadOnly)
					MakeBindingControlCssReadOnly(control, controlType);
			}
		}

		/// <summary>
		/// Convert input string value to valid web control id.
		/// </summary>
		/// <param name="inputValue"></param>
		/// <returns></returns>
		public static string ConvertControlId(string inputValue)
		{
			if (string.IsNullOrEmpty(inputValue)) return inputValue;

			MatchCollection matchCollection = regexToPickValidControlIdCharacters.Matches(inputValue);
			StringBuilder output = new StringBuilder();
			foreach (Match match in matchCollection)
				if (match.Success)
					output.Append(match.Value);

			string result = output.ToString();
			if (result == inputValue) return result;
			else
				return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", result, Math.Abs(inputValue.GetHashCode()));
		}

        /// <summary>
		/// Replace variables (in the format "$variableName$") included in input string with values set in "IApplicationContext.LabelVariables" and globalization variables defined as "$Assembly, Namespace.Class, Key$".
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceVariables(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

			string results = GlobalizationUtility.ReplaceGlobalizationVariables(input);
            MatchCollection matchCollection = regexToPickVariables.Matches(results);

            if (matchCollection.Count > 0)
				results = ReplaceVariables(results, matchCollection);

			return results;
		}

		#region Private Static Methods

		private static string ReplaceVariables(string input, MatchCollection matchCollection)
        {
			IApplicationContext applicationContext = SpringContext.Current.GetObject<IApplicationContext>();
            StringBuilder returnValue = new StringBuilder();
            int lastMatchEndIndex = 0;
            for (int i = 0; i < matchCollection.Count; i++)
            {
                Match match = matchCollection[i];
                if (match.Index > lastMatchEndIndex)
                {
                    string constValue = input.Substring(lastMatchEndIndex, match.Index - lastMatchEndIndex);
                    returnValue.Append(constValue);
                }

                string variableName = match.Value.Substring(1, match.Value.Length - 2);
				if (!applicationContext.TempVariables.Contains(variableName))
					throw new InvalidProgramException(string.Format(Resources.DP_VariableNotExistInCurrentContext, variableName, input));

				object variableValue = applicationContext.TempVariables[variableName];
                string partialValue = variableValue != null ? variableValue.ToString() : "";
                returnValue.Append(partialValue);

                lastMatchEndIndex = match.Index + match.Length;

                if (i == matchCollection.Count - 1 && lastMatchEndIndex < input.Length)
                {
                    returnValue.Append(input.Substring(lastMatchEndIndex));
                }
            }

            return returnValue.ToString();
        }

		private static Control FindControlRecursively(IEnumerable<Control> controls, string controlId)
		{
			foreach (Control control in controls)
				if (control.ID == controlId)
					return control;

			IEnumerable<Control> subControls = controls.SelectMany(c => c.Controls.Cast<Control>());
			if (subControls.Count() > 0)
				return FindControlRecursively(subControls, controlId);

			return null;
		}

		private static void MakeBindingControlCssReadOnly(object control, Type controlType)
		{
			string className;
			PropertyInfo cssClassProperty = controlType.GetProperty("CssClass", BindingFlags.Public | BindingFlags.Instance);
			if (cssClassProperty != null)
			{
				className = cssClassProperty.GetValue(control, null) as string ?? "";
				className += " readonly";
				cssClassProperty.SetValue(control, className, null);
			}
			else
			{
				PropertyInfo attributesProperty = controlType.GetProperty("Attributes", BindingFlags.Public | BindingFlags.Instance);
				if (attributesProperty != null)
				{
					object attributes = attributesProperty.GetValue(control, null);
					Type attributesType = attributes.GetType();
					PropertyInfo indexerProperty = attributesType.GetProperty("Item");
					if (indexerProperty != null)
					{
						className = indexerProperty.GetValue(attributes, new object[] { "class" }) as string ?? "";
						className += " readonly";
						indexerProperty.SetValue(attributes, className, new object[] { "class" });
					}
				}
			}
		}

		#endregion
	}
}
