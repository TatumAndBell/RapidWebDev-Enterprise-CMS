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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using RapidWebDev.Common;
using RapidWebDev.Common.CodeDom;
using RapidWebDev.Common.Globalization;
using RapidWebDev.Common.Validation;
using RapidWebDev.Common.Web;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;
using Spring.Core.IO;
using Spring.Objects;

namespace RapidWebDev.UI.DynamicPages.PrintAndExcel
{
	/// <summary>
	/// Interface to print queried data in the dynamic page to any format depends on the implementation.
	/// </summary>
	public class DynamicPagePrinter : IDynamicPagePrinter
	{
		/// <summary>
		/// The server encountered an unknown error when accessing the URI {0}.
		/// </summary>
		private const string UNKNOWN_ERROR_LOGGING_MSG = "The server encountered an unknown error when accessing the URI {0}.";
		private const string DEFAULT_STYLE_SHEET_URI = "assembly://RapidWebDev.UI/RapidWebDev.UI.DynamicPages.PrintAndExcel/DynamicPageExcelPrinter.xslt";

		private static readonly XmlSerializer serializer = new XmlSerializer(typeof(DataSource));
		private IApplicationContext applicationContext;
		private string styleSheetUri;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="applicationContext"></param>
		public DynamicPagePrinter(IApplicationContext applicationContext)
		{
			this.applicationContext = applicationContext;
		}

		/// <summary>
		/// Style sheet Uri used to format data source to output file.
		/// The printer uses assembly manifest stylesheet as default XSLT to transform queried data source.
		/// </summary>
		public string StyleSheetUri 
		{
			get { return this.styleSheetUri ?? DEFAULT_STYLE_SHEET_URI; }
			set { this.styleSheetUri = value; }
		}

		/// <summary>
		/// Directory to store generated temporary files.
		/// </summary>
		public string TemporaryFileDirectory { get; set; }

		/// <summary>
		/// The extension name of temporary file.
		/// </summary>
		public string TemporaryFileExtensionName { get; set; }

		/// <summary>
		/// Print queried data in the dynamic page to any format depends on the implementation.
		/// </summary>
		/// <param name="dynamicPageService">The dynamic page handler.</param>
		/// <param name="queryStringParameters">The query string parameters.</param>
		/// <returns></returns>
		public DynamicPagePrintResult Print(IDynamicPage dynamicPageService, NameValueCollection queryStringParameters)
		{
			try
			{
				QueryParameter queryParameter = QueryParameter.CreateQueryParameter(dynamicPageService, queryStringParameters, 0, int.MaxValue, null, null);

				// Execute query.
				QueryResults queryResults = dynamicPageService.Query(queryParameter);
				DataSource dataSource = ConvertQueryResults(dynamicPageService, queryResults.Results);

				return TransformDataSource(dataSource);
			}
			catch (ValidationException exp)
			{
				Logger.Instance(typeof(DynamicPagePrinter)).Warn(exp);
				throw;
			}
			catch (Exception exp)
			{
				Logger.Instance(typeof(DynamicPagePrinter)).Error(exp);
				throw;
			}
		}

		/// <summary>
		/// Convert QueryResults by generate decorate serializable class with transformed values.
		/// </summary>
		/// <param name="dynamicPageService"></param>
		/// <param name="results"></param>
		/// <returns></returns>
		private DataSource ConvertQueryResults(IDynamicPage dynamicPageService, IEnumerable results)
		{
			IEnumerable<object> originalObjects = results.Cast<object>();

			DataSource dataSource = new DataSource
			{
				Author = this.applicationContext.Identity.Name,
				CreatedOn = LocalizationUtility.ConvertUtcTimeToClientTime(DateTime.UtcNow),
				Name = dynamicPageService.Configuration.Title,
				Description = string.Format(CultureInfo.InvariantCulture, Resources.DP_DynamicPage_Print_HeadBar_Template, originalObjects.Count(), LocalizationUtility.ToDateTimeString(DateTime.Now), this.applicationContext.Identity.Name),
				LogoUrl = Path.Combine(Kit.WebSiteBaseUrl, "resources/images/logo.png")
			};

			if (originalObjects.Count() == 0) return dataSource;

			IDictionary<PropertyDefinition, string> propertyDefinitionAndHeaderText = CreatePropertyDecorateConfigs(dynamicPageService);
			IEnumerable<PropertyDefinition> propertyDefinitionEnumerable = propertyDefinitionAndHeaderText.Keys;
			IEnumerable<object> convertResults = null;

			try
			{
				convertResults = ClassDecorator.CreateDecorateObjects(originalObjects, propertyDefinitionEnumerable).Cast<object>();
			}
			catch (CompileException exp)
			{
				string loggingMessage = string.Format(CultureInfo.InvariantCulture, "There has errors to compile a dynamic class from dynamic page configuration to render UI grid of the dynamic page with object id equals to \"{0}\".", QueryStringUtility.ObjectId);
				Logger.Instance(typeof(DynamicPagePrinter)).Error(loggingMessage, exp);
				throw;
			}			

			FillQueryResult(dataSource, propertyDefinitionAndHeaderText, convertResults);
			return dataSource;
		}

		/// <summary>
		/// Create a collection of <see cref="PropertyDefinition"/> to configure how to create decorate class and clone objects into new types from original query results.
		/// </summary>
		/// <param name="dynamicPageService"></param>
		/// <returns></returns>
		private static IDictionary<PropertyDefinition, string> CreatePropertyDecorateConfigs(IDynamicPage dynamicPageService)
		{
			GridViewPanelConfiguration gridViewPanel = dynamicPageService.Configuration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.GridViewPanel) as GridViewPanelConfiguration;
			if (gridViewPanel == null)
				return new Dictionary<PropertyDefinition, string>();

			Dictionary<PropertyDefinition, string> results = new Dictionary<PropertyDefinition, string>();

			ExtGridConfiguration extGridConfiguration = ExtGridConfigurationResolver.Resolve();

			// Add wrappers for original properties
			for (int index = 0; index < gridViewPanel.Fields.Count; index++)
			{
				if (extGridConfiguration != null)
				{
					int findingColumnIndex = index + 1;
					if (gridViewPanel.EnabledCheckBoxField) findingColumnIndex++;
					ExtGridColumnConfiguration extGridColumnConfiguration = extGridConfiguration.Columns.FirstOrDefault(col => col.ColumnIndex == findingColumnIndex);
					if (extGridColumnConfiguration == null || extGridColumnConfiguration.Hidden) continue;
				}

				GridViewFieldConfiguration gridViewField = gridViewPanel.Fields[index];
				PropertyDefinition propertyDecorateConfig = new PropertyDefinition(typeof(string))
				{
					PropertyName = gridViewField.FieldName,
					NewPropertyName = FieldNameTransformUtility.DataBoundFieldName(gridViewField.FieldName, index)
				};

				GridViewFieldValueTransformConfiguration transformer = gridViewField.Transformer;
				if (transformer != null && transformer.TransformType != GridViewFieldValueTransformTypes.None)
				{
					propertyDecorateConfig.PropertyValueConvertCallback = arg => { return TransformFieldValue(transformer, gridViewField.FieldName, arg); };
				}
				else
				{
					propertyDecorateConfig.PropertyValueConvertCallback = arg =>
					{
						if (!(arg is string) && arg is IEnumerable) return arg;
						return arg != null ? arg.ToString() : "";
					};
				}

				results.Add(propertyDecorateConfig, gridViewField.HeaderText);
			}

			// Add wrapper for row view property.
			if (gridViewPanel.RowView != null)
			{
				results.Add(new PropertyDefinition(typeof(string))
				{
					PropertyName = gridViewPanel.RowView.FieldName,
					NewPropertyName = FieldNameTransformUtility.ViewFragmentFieldName(gridViewPanel.RowView.FieldName),
					PropertyValueConvertCallback = (arg) => { return arg != null ? arg.ToString() : ""; }
				}, Resources.DP_RowView_HeaderText);
			}

			return results;
		}

		/// <summary>
		/// The callback method to transform original field value by configuration of field transformation.
		/// </summary>
		/// <param name="transformer"></param>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <returns></returns>
		private static object TransformFieldValue(GridViewFieldValueTransformConfiguration transformer, string fieldName, object fieldValue)
		{
			switch (transformer.TransformType)
			{
				case GridViewFieldValueTransformTypes.Callback:
					return transformer.Callback.Format(fieldName, fieldValue) ?? "";

				case GridViewFieldValueTransformTypes.Switch:
					if (fieldValue == null) return "";
					GridViewFieldValueTransformSwitchCase switchCase = transformer.SwitchCases.FirstOrDefault(c => string.Compare(c.Value, fieldValue.ToString(), !c.CaseSensitive) == 0);
					if (switchCase == null) return fieldValue.ToString();
					return switchCase.Output ?? "";

				case GridViewFieldValueTransformTypes.ToStringParameter:
					if (fieldValue == null) return "";
					Type fieldValueType = fieldValue.GetType();
					MethodInfo toStringMethod = fieldValueType.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance);
					if (toStringMethod != null)
					{
						try
						{
							return toStringMethod.Invoke(fieldValue, new object[] { transformer.ToStringParameter }).ToString();
						}
						catch (Exception exp)
						{
							Logger.Instance(typeof(DynamicPagePrinter)).Warn(exp);
						}
					}

					return "";

				case GridViewFieldValueTransformTypes.VariableString:
					if (fieldValue == null) return "";
					return WebUtility.ReplaceVariables(fieldValue.ToString()) ?? "";
			}

			return fieldValue.ToString();
		}

		private static void FillQueryResult(DataSource dataSource, IDictionary<PropertyDefinition, string> propertyDefinitionAndHeaderText, IEnumerable<object> queryResults)
		{
			List<string> dataSchema = new List<string>();
			foreach (PropertyDefinition propertyDefinition in propertyDefinitionAndHeaderText.Keys)
				dataSchema.Add(propertyDefinitionAndHeaderText[propertyDefinition]);

			List<Data> dataCollection = new List<Data>(queryResults.Count());
			foreach (object queryResult in queryResults)
			{
				List<DataProperty> dataProperties = new List<DataProperty>();
				foreach (PropertyDefinition propertyDefinition in propertyDefinitionAndHeaderText.Keys)
				{
					DataProperty property = new DataProperty { Name = propertyDefinitionAndHeaderText[propertyDefinition], Value = DataBinder.Eval(queryResult, propertyDefinition.NewPropertyName) as string };
					dataProperties.Add(property);
				}

				Data data = new Data { Property = dataProperties.ToArray() };
				dataCollection.Add(data);
			}

			dataSource.Schema = dataSchema.ToArray();
			dataSource.Data = dataCollection.ToArray();
		}

		private DynamicPagePrintResult TransformDataSource(DataSource dataSource)
		{
			StringBuilder xmlBuilder = new StringBuilder();
			using (StringWriter writer = new StringWriter(xmlBuilder))
			{
				serializer.Serialize(writer, dataSource);
			}

			IResource resource = SpringContext.Current.GetResource(this.StyleSheetUri);
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(resource.InputStream))
				{
					XslCompiledTransform transformer = new XslCompiledTransform();
					transformer.Load(xmlReader);

					string temporaryFileDirectory = HttpContext.Current.Server.MapPath(this.TemporaryFileDirectory);
					string temporaryFileName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Guid.NewGuid().ToString("N"), this.TemporaryFileExtensionName);
					string temporaryFilePath = Path.Combine(temporaryFileDirectory, temporaryFileName);

					using (StringReader xmlSourceTextReader = new StringReader(xmlBuilder.ToString()))
					using (XmlReader xmlSourceReader = XmlReader.Create(xmlSourceTextReader))
					using (FileStream outputFileStream = new FileStream(temporaryFilePath, FileMode.Create, FileAccess.Write))
					{
						transformer.Transform(xmlSourceReader, new XsltArgumentList(), outputFileStream);
					}

					return new DynamicPagePrintResult { ResultType = DynamicPagePrintResultType.TemporaryFile, Result = Path.Combine(this.TemporaryFileDirectory, temporaryFileName) };
				}
			}
			finally
			{
				if (resource.InputStream != null)
				{
					resource.InputStream.Close();
					resource.InputStream.Dispose();
				}
			}
		}
	}
}

