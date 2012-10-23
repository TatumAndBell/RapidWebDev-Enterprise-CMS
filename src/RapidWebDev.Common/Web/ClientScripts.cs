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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web;
using System.Globalization;

namespace RapidWebDev.Common.Web
{
	/// <summary>
	/// Helpful methods on client scripts registration.
	/// </summary>
	public static class ClientScripts
	{
		private static object syncObject = new object();
		private static volatile OnDocumentReady onDocumentReady;

		/// <summary>
		/// Add javascript block to the client which will be executed only when the document is on ready.
		/// </summary>
		public static OnDocumentReady OnDocumentReady
		{
			get
			{
				if (onDocumentReady == null)
				{
					lock (syncObject)
					{
						if (onDocumentReady == null)
						{
							onDocumentReady = new OnDocumentReady();
						}
					}
				}

				return onDocumentReady;
			}
		}

		/// <summary>
		/// Register style reference into page header section. 
		/// The method does nothing in asynchronous post.
		/// </summary>
		/// <param name="styleFileUrl"></param>
		public static void RegisterHeaderStyleInclude(string styleFileUrl)
		{
			Page webpage = System.Web.HttpContext.Current.Handler as Page;
			if (webpage != null)
			{
				ScriptManager scriptManager = ScriptManager.GetCurrent(webpage);
				if (scriptManager == null || !scriptManager.IsInAsyncPostBack)
				{
					string styleFileUrlKey = styleFileUrl.ToLowerInvariant().GetHashCode().ToString();
					if (!webpage.Items.Contains(styleFileUrlKey))
					{
						HtmlGenericControl styleReference = new HtmlGenericControl("link");
						styleReference.Attributes["rel"] = "stylesheet";
						styleReference.Attributes["type"] = "text/css";
						styleReference.Attributes["href"] = styleFileUrl;
						webpage.Header.Controls.Add(styleReference);
						webpage.Items.Add(styleFileUrlKey, true);
					}
				}
			}
		}

		/// <summary>
		/// Register script reference into page header section. 
		/// The method does nothing in asynchronous post.
		/// </summary>
		/// <param name="scriptFileUrl"></param>
		public static void RegisterHeaderScriptInclude(string scriptFileUrl)
		{
			Page webpage = System.Web.HttpContext.Current.Handler as Page;
			if (webpage != null)
			{
				ScriptManager scriptManager = ScriptManager.GetCurrent(webpage);
				if (scriptManager == null || !scriptManager.IsInAsyncPostBack)
				{
					string scriptFileUrlKey = scriptFileUrl.ToLowerInvariant().GetHashCode().ToString();
					if (!webpage.Items.Contains(scriptFileUrlKey))
					{
						HtmlGenericControl scriptReference = new HtmlGenericControl("script");
						scriptReference.Attributes["type"] = "text/javascript";
						scriptReference.Attributes["src"] = Kit.ResolveAbsoluteUrl(scriptFileUrl);
						webpage.Header.Controls.Add(scriptReference);
						webpage.Items.Add(scriptFileUrlKey, true);
					}
				}
			}
		}

		/// <summary>
		/// Register script reference block at the beginning of page body. 
		/// The method doesn't work in ScriptManager asynchronous callback. 
		/// We have to call this method to register javascript reference in OnInit event or before.
		/// </summary>
		/// <param name="scriptFileUrl">Script file URL</param>
		public static void RegisterBodyScriptInclude(string scriptFileUrl)
		{
			Page webpage = System.Web.HttpContext.Current.Handler as Page;
			if (webpage != null)
			{
				string scriptFileKey = scriptFileUrl.ToLowerInvariant().GetHashCode().ToString();
				if (!webpage.ClientScript.IsClientScriptIncludeRegistered(scriptFileKey))
					webpage.ClientScript.RegisterClientScriptInclude(scriptFileKey, scriptFileUrl);
			}
		}

		/// <summary>
		/// Register javascript block at the begining of page body.
		/// The method will use ScriptManager for registration if post in asynchronous way, otherwise it uses Page.ClientScript for registration.
		/// </summary>
		/// <param name="scriptBlock">javascript block</param>
		public static void RegisterScriptBlock(string scriptBlock)
		{
			Page webpage = System.Web.HttpContext.Current.Handler as Page;
			if (webpage != null)
			{
				string scriptBlockKey = scriptBlock.ToLowerInvariant().GetHashCode().ToString();
				ScriptManager scriptManager = ScriptManager.GetCurrent(webpage);
				if (scriptManager != null && scriptManager.IsInAsyncPostBack)
					ScriptManager.RegisterClientScriptBlock(webpage, webpage.GetType(), scriptBlockKey, scriptBlock, true);
				else if (!webpage.ClientScript.IsClientScriptBlockRegistered(scriptBlockKey))
					webpage.ClientScript.RegisterClientScriptBlock(webpage.GetType(), scriptBlockKey, scriptBlock, true);
			}
		}

		/// <summary>
		/// Register javascript block at the end of page body.
		/// The method will use ScriptManager for registration if post in asynchronous way, otherwise it uses Page.ClientScript for registration.
		/// </summary>
		/// <param name="scriptBlock"></param>
		public static void RegisterStartupScript(string scriptBlock)
		{
			Page webpage = System.Web.HttpContext.Current.Handler as Page;
			if (webpage != null)
			{
				string scriptBlockKey = scriptBlock.ToLowerInvariant().GetHashCode().ToString();
				ScriptManager scriptManager = ScriptManager.GetCurrent(webpage);
				if (scriptManager != null && scriptManager.IsInAsyncPostBack)
					ScriptManager.RegisterStartupScript(webpage, webpage.GetType(), scriptBlockKey, scriptBlock, true);
				else if (!webpage.ClientScript.IsStartupScriptRegistered(scriptBlockKey))
					webpage.ClientScript.RegisterStartupScript(webpage.GetType(), scriptBlockKey, scriptBlock, true);
			}
		}
	}

	/// <summary>
	/// Register JavaScript block into "$(document).onReady(function(){ ... });" block of client so that the javascript block will only be executed when the dom is ready.
	/// </summary>
	public class OnDocumentReady
	{
		private Dictionary<JavaScriptPosition, List<JavaScriptItem>> javaScriptBlocks = new Dictionary<JavaScriptPosition, List<JavaScriptItem>>();

		private Dictionary<JavaScriptPosition, List<JavaScriptItem>> JavaScriptBlocks
		{
			get
			{
				if (HttpContext.Current == null) 
					return javaScriptBlocks;

				const string contextKey = "RapidWebDev.Common.OnDomReady::JavaScriptBlocks";
				if (!HttpContext.Current.Items.Contains(contextKey))
					HttpContext.Current.Items[contextKey] = new Dictionary<JavaScriptPosition, List<JavaScriptItem>>();

				return HttpContext.Current.Items[contextKey] as Dictionary<JavaScriptPosition, List<JavaScriptItem>>;
			}
		}

		/// <summary>
		/// Protected constructor
		/// </summary>
		internal OnDocumentReady() { }

		/// <summary>
		/// Add script block to the begin of document body with Normal priority.
		/// </summary>
		/// <param name="scriptBlock">javascript block</param>
		public void Add2BeginOfBody(string scriptBlock)
		{
			Add2BeginOfBody(scriptBlock, JavaScriptPriority.Normal);
		}

		/// <summary>
		/// Add script block to the begin of document body with Normal priority.
		/// </summary>
		/// <param name="scriptBlock">javascript block</param>
		/// <param name="priority">javascript executing priority</param>
		public void Add2BeginOfBody(string scriptBlock, JavaScriptPriority priority)
		{
			GetJavaScriptItems(JavaScriptPosition.BeginOfBody).Add(new JavaScriptItem(scriptBlock, priority));
		}

		/// <summary>
		/// Add script block to the end of document body with Normal priority.
		/// </summary>
		/// <param name="scriptBlock">javascript block</param>
		public void Add2EndOfBody(string scriptBlock)
		{
			Add2EndOfBody(scriptBlock, JavaScriptPriority.Normal);
		}

		/// <summary>
		/// Add script block to the end of document body with Normal priority.
		/// </summary>
		/// <param name="scriptBlock">javascript block</param>
		/// <param name="priority">javascript executing priority</param>
		public void Add2EndOfBody(string scriptBlock, JavaScriptPriority priority)
		{
			GetJavaScriptItems(JavaScriptPosition.EndOfBody).Add(new JavaScriptItem(scriptBlock, priority));
		}

		/// <summary>
		/// Flush added javascript blocks to clients.
		/// </summary>
		public void Flush()
		{
			const string onDocumentReadyTemplate = @"Ext.onReady(function() {{ {0} }});";

			IEnumerable<JavaScriptItem> javaScriptItems = GetJavaScriptItems(JavaScriptPosition.BeginOfBody);
			IEnumerable<string> javaScriptBlocks;
			if (javaScriptItems.Count() > 0)
			{
				javaScriptBlocks = javaScriptItems.OrderByDescending(item => item.Priority).Select(item => "(function(){" + item.JavaScript + "}());");
				ClientScripts.RegisterScriptBlock(string.Format(CultureInfo.InvariantCulture, onDocumentReadyTemplate, javaScriptBlocks.Concat("\r\n")));
			}

			javaScriptItems = GetJavaScriptItems(JavaScriptPosition.EndOfBody);
			if (javaScriptItems.Count() > 0)
			{
				javaScriptBlocks = javaScriptItems.OrderByDescending(item => item.Priority).Select(item => "(function(){" + item.JavaScript + "}());");
				ClientScripts.RegisterScriptBlock(string.Format(CultureInfo.InvariantCulture, onDocumentReadyTemplate, javaScriptBlocks.Concat("\r\n")));
			}

			this.JavaScriptBlocks.Clear();
		}

		private List<JavaScriptItem> GetJavaScriptItems(JavaScriptPosition position)
		{
			if (!this.JavaScriptBlocks.ContainsKey(position))
				this.JavaScriptBlocks.Add(position, new List<JavaScriptItem>());

			return this.JavaScriptBlocks[position];
		}

		private class JavaScriptItem
		{
			public JavaScriptItem(string javaScript, JavaScriptPriority priority)
			{
				this.JavaScript = javaScript;
				this.Priority = priority;
			}

			public string JavaScript { get; set; }
			public JavaScriptPriority Priority { get; set; }
		}
	}

	/// <summary>
	/// JavaScript execute priority.
	/// </summary>
	public enum JavaScriptPriority
	{
		/// <summary>
		/// Low
		/// </summary>
		Low = 0,

		/// <summary>
		/// Normal
		/// </summary>
		Normal = 1,

		/// <summary>
		/// Hight
		/// </summary>
		High = 2,
	}

	/// <summary>
	/// The position where JavaScript render to.
	/// </summary>
	public enum JavaScriptPosition
	{
		/// <summary>
		/// Begin of body
		/// </summary>
		BeginOfBody = 0,

		/// <summary>
		/// End of body
		/// </summary>
		EndOfBody = 1,
	}
}

