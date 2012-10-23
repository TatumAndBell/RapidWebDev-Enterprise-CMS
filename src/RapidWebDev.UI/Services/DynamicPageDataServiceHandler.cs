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
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using RapidWebDev.Common;
using RapidWebDev.Common.CodeDom;
using RapidWebDev.Common.Validation;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using RapidWebDev.UI.Properties;
using Spring.Objects;
using Spring.Aop.Framework;
using Spring.Aop.Support;

namespace RapidWebDev.UI.Services
{
	/// <summary>
	/// The service for client script to query data for the data grid of dynamic web page.
	/// </summary>
	public class DynamicPageDataServiceHandler : IHttpHandler, IRequiresSessionState
	{
		/// <summary>
		/// The server encountered an unknown error when accessing the URI {0}.
		/// </summary>
		private const string UNKNOWN_ERROR_LOGGING_MSG = "The server encountered an unknown error when accessing the URI {0}.";

		internal const string ShowCheckBoxColumnPropertyName = "_ShowCheckBoxColumn";
		internal const string ShowEditButtonColumnPropertyName = "_ShowEditButtonColumn";
		internal const string ShowViewButtonColumnPropertyName = "_ShowViewButtonColumn";
		internal const string ShowDeleteButtonColumnPropertyName = "_ShowDeleteButtonColumn";

		private static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
		private static readonly IPermissionBridge permissionBridge = SpringContext.Current.GetObject<IPermissionBridge>();

		bool IHttpHandler.IsReusable { get { return true; } }

		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			try
			{
				context.Response.StatusCode = (int)HttpStatusCode.OK;
				context.Response.ContentType = "application/json";

				string output = this.ProcessRequest(context);
				context.Response.Write(output);
			}
			catch (CommonException exp)
			{
				HandleException(context, exp.Message);
			}
			catch (Exception exp)
			{
				string errorMessage = string.Format(CultureInfo.InvariantCulture, UNKNOWN_ERROR_LOGGING_MSG, HttpContext.Current.Request.Url);
				Logger.Instance(this).Error(errorMessage, exp);

				HandleException(context, Resources.DP_UnknownErrorDetail);
			}

			context.Response.Cache.SetNoServerCaching();
			context.Response.Cache.SetCacheability(HttpCacheability.Private);
			context.Response.Cache.SetExpires(DateTime.Now);
			context.Response.Cache.SetLastModified(DateTime.Now);
		}

		private string ProcessRequest(HttpContext context)
		{
			if (Kit.IsEmpty(QueryStringUtility.ObjectId))
				throw new BadRequestException("The query string parameter ObjectId is not specified.");

			if (Kit.IsEmpty(context.Request["Start"]))
				throw new BadRequestException("The query string parameter Start is not specified.");

			if (Kit.IsEmpty(context.Request["Limit"]))
				throw new BadRequestException("The query string parameter Limit is not specified.");

			string objectId = QueryStringUtility.ObjectId;
			IDynamicPage dynamicPageService = null;
			try
			{
				dynamicPageService = DynamicPageContext.Current.GetDynamicPage(objectId, false, this.GetRequestHandler());
			}
			catch (ConfigurationErrorsException)
			{
				throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "The query string parameter ObjectId \"{0}\" is invalid. Please be caution the parameter is case sensitive.", objectId));
			}

			if (!permissionBridge.HasPermission(dynamicPageService.Configuration.PermissionValue))
				throw new UnauthorizedException(Resources.DP_UnauthorizedAccess);

			int start;
			if (!int.TryParse(context.Request["Start"], out start) || start < 0)
				throw new BadRequestException("The query string parameter Start is not a non-negative integer.");

			int limit;
			if (!int.TryParse(context.Request["Limit"], out limit) || limit < 0)
				throw new BadRequestException("The query string parameter Limit is not a non-negative integer.");

			string errorMessage = null;
			if (!string.IsNullOrEmpty(QueryStringUtility.DeleteId))
				errorMessage = DeleteEntityById(dynamicPageService, QueryStringUtility.DeleteId);

			return ExecuteQuery(dynamicPageService, start, limit, context.Request["SortField"], context.Request["SortDirection"], errorMessage);
		}

		private IRequestHandler requestHandler;
		private IRequestHandler GetRequestHandler()
		{
			if (this.requestHandler == null)
			{
				ProxyFactory proxyFactory = new ProxyFactory(this);
				proxyFactory.AddInterface(typeof(IRequestHandler));
				proxyFactory.AddIntroduction(new DefaultIntroductionAdvisor(new HttpRequestHandler()));
				proxyFactory.ProxyTargetType = true;
				this.requestHandler = proxyFactory.GetProxy() as IRequestHandler;
			}

			return this.requestHandler;
		}

		/// <summary>
		/// Delete an existed entity by id.
		/// </summary>
		/// <param name="dynamicPageService"></param>
		/// <param name="entityId"></param>
		private static string DeleteEntityById(IDynamicPage dynamicPageService, string entityId)
		{
			try
			{
				dynamicPageService.Delete(entityId);
				return null;
			}
			catch (ValidationException exp)
			{
				return exp.Message;
			}
			catch (Exception exp)
			{
				Logger.Instance(typeof(DynamicPageDataServiceHandler)).Error(exp);
				return Resources.DP_UnknownErrorDetail;
			}
		}

		/// <summary>
		/// Execute query for specified dynamic page by object id.
		/// </summary>
		/// <param name="dynamicPageService">dynamic page service</param>
		/// <param name="start">start offset of returned results</param>
		/// <param name="limit">total count of returned results</param>
		/// <param name="sortField">sorting field name</param>
		/// <param name="sortDirection">sorting field direction</param>
		/// <param name="errorMessage">error messages response to the client.</param>
		/// <returns>returns json-serialized string from <see cref="QueryResultObject"/></returns>
		private static string ExecuteQuery(IDynamicPage dynamicPageService, int start, int limit, string sortField, string sortDirection, string errorMessage)
		{
			IEnumerable convertedQueryResults = null;
			int totalRecordCount = 0;
			string returnedErrorMessage = errorMessage;

			if (string.IsNullOrEmpty(returnedErrorMessage))
			{
				try
				{
					QueryParameter queryParameter = QueryParameter.CreateQueryParameter(dynamicPageService, HttpContext.Current.Request.QueryString, start, limit, sortField, sortDirection);

					// Execute query.
					QueryResults queryResults = dynamicPageService.Query(queryParameter);
					convertedQueryResults = ConvertQueryResults(dynamicPageService, queryResults.Results);
					totalRecordCount = queryResults.RecordCount;
				}
				catch (ValidationException exp)
				{
					returnedErrorMessage = exp.Message;
				}
				catch (Exception exp)
				{
					returnedErrorMessage = Resources.DP_UnknownErrorDetail;
					Logger.Instance(typeof(DynamicPageDataServiceHandler)).Error(exp);
				}
			}

			// Construct query result object for serialization.
			QueryResultObject queryResultObject = new QueryResultObject
			{
				Records = convertedQueryResults,
				TotalRecordCount = totalRecordCount,
				Start = start,
				Limit = limit,
				HasError = !string.IsNullOrEmpty(returnedErrorMessage),
				ErrorMessage = returnedErrorMessage
			};

			return serializer.Serialize(queryResultObject);
		}

		/// <summary>
		/// Convert QueryResults by generate decorate serializable class with transformed values.
		/// </summary>
		/// <param name="dynamicPageService"></param>
		/// <param name="results"></param>
		/// <returns></returns>
		private static IEnumerable ConvertQueryResults(IDynamicPage dynamicPageService, IEnumerable results)
		{
			IEnumerable<object> originalObjects = results.Cast<object>();
			if (originalObjects.Count() == 0) return results;

			IEnumerable<PropertyDefinition> propertyDecorateConfigs = CreatePropertyDecorateConfigs(dynamicPageService);
			IEnumerable<object> convertResults = null;

			try
			{
				convertResults = ClassDecorator.CreateDecorateObjects(originalObjects, propertyDecorateConfigs.ToArray()).Cast<object>();
			}
			catch (CompileException exp)
			{
				string loggingMessage = string.Format(CultureInfo.InvariantCulture, "There has errors to compile a dynamic class from dynamic page configuration to render UI grid of the dynamic page with object id equals to \"{0}\".", QueryStringUtility.ObjectId);
				Logger.Instance(typeof(DynamicPageDataServiceHandler)).Error(loggingMessage, exp);
				throw;
			}

			ObjectWrapper convertResultObjectWrapper = new ObjectWrapper(convertResults.First().GetType());
			for (int i = 0; i < originalObjects.Count(); i++)
			{
				object originalObject = originalObjects.ElementAt(i);
				GridRowControlBindEventArgs arg = new GridRowControlBindEventArgs(originalObject);
				dynamicPageService.OnGridRowControlsBind(arg);
				convertResultObjectWrapper.WrappedInstance = convertResults.ElementAt(i);

				// set visibility of checkbox
				bool showCheckBoxColumn = (bool)convertResultObjectWrapper.GetPropertyValue(DynamicPageDataServiceHandler.ShowCheckBoxColumnPropertyName);
				convertResultObjectWrapper.SetPropertyValue(DynamicPageDataServiceHandler.ShowCheckBoxColumnPropertyName, showCheckBoxColumn && arg.ShowCheckBoxColumn);

				// set visibility of edit button
				string permissionValue = string.Format(CultureInfo.InvariantCulture, "{0}.Update", dynamicPageService.Configuration.PermissionValue);
				bool hasUpdatePermission = permissionBridge.HasPermission(permissionValue);
				bool showEditButtonColumn = (bool)convertResultObjectWrapper.GetPropertyValue(DynamicPageDataServiceHandler.ShowEditButtonColumnPropertyName);
				convertResultObjectWrapper.SetPropertyValue(DynamicPageDataServiceHandler.ShowEditButtonColumnPropertyName, showEditButtonColumn && arg.ShowEditButton && hasUpdatePermission);

				// set visibility of view button
				bool showViewButtonColumn = (bool)convertResultObjectWrapper.GetPropertyValue(DynamicPageDataServiceHandler.ShowViewButtonColumnPropertyName);
				convertResultObjectWrapper.SetPropertyValue(DynamicPageDataServiceHandler.ShowViewButtonColumnPropertyName, showViewButtonColumn && arg.ShowViewButton);

				// set visibility of delete button
				permissionValue = string.Format(CultureInfo.InvariantCulture, "{0}.Delete", dynamicPageService.Configuration.PermissionValue);
				bool hasDeletePermission = permissionBridge.HasPermission(permissionValue);
				bool showDeleteButtonColumn = (bool)convertResultObjectWrapper.GetPropertyValue(DynamicPageDataServiceHandler.ShowDeleteButtonColumnPropertyName);
				convertResultObjectWrapper.SetPropertyValue(DynamicPageDataServiceHandler.ShowDeleteButtonColumnPropertyName, showDeleteButtonColumn && arg.ShowDeleteButton && hasDeletePermission);
			}

			return convertResults;
		}

		/// <summary>
		/// Create a collection of <see cref="PropertyDefinition"/> to configure how to create decorate class and clone objects into new types from original query results.
		/// </summary>
		/// <param name="dynamicPageService"></param>
		/// <returns></returns>
		private static IEnumerable<PropertyDefinition> CreatePropertyDecorateConfigs(IDynamicPage dynamicPageService)
		{
			GridViewPanelConfiguration gridViewPanel = dynamicPageService.Configuration.Panels.FirstOrDefault(p => p.PanelType == DynamicPagePanelTypes.GridViewPanel) as GridViewPanelConfiguration;
			if (gridViewPanel == null)
				return new PropertyDefinition[0];

			Collection<PropertyDefinition> results = new Collection<PropertyDefinition>();
			results.Add(new PropertyDefinition(typeof(string)) { PropertyName = gridViewPanel.PrimaryKeyFieldName, NewPropertyName = FieldNameTransformUtility.PrimaryKeyFieldName(gridViewPanel.PrimaryKeyFieldName) });

			// Add wrappers for original properties
			for (int index = 0; index < gridViewPanel.Fields.Count; index++)
			{
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

				results.Add(propertyDecorateConfig);
			}

			// Add wrapper for row view property.
			if (gridViewPanel.RowView != null)
			{
				results.Add(new PropertyDefinition(typeof(string))
				{
					PropertyName = gridViewPanel.RowView.FieldName,
					NewPropertyName = FieldNameTransformUtility.ViewFragmentFieldName(gridViewPanel.RowView.FieldName),
					PropertyValueConvertCallback = (arg) => { return arg != null ? arg.ToString() : ""; }
				});
			}

			// Add wrapper for ShowCheckBox
			results.Add(new PropertyDefinition(typeof(bool))
			{
				PropertyName = null,
				NewPropertyName = DynamicPageDataServiceHandler.ShowCheckBoxColumnPropertyName,
				PropertyValueConvertCallback = (arg) => { return gridViewPanel.EnabledCheckBoxField; }
			});

			// Add wrapper for EditButton
			results.Add(new PropertyDefinition(typeof(bool))
			{
				PropertyName = null,
				NewPropertyName = DynamicPageDataServiceHandler.ShowEditButtonColumnPropertyName,
				PropertyValueConvertCallback = (arg) => { return gridViewPanel.EditButton != null; }
			});

			// Add wrapper for ViewButton
			results.Add(new PropertyDefinition(typeof(bool))
			{
				PropertyName = null,
				NewPropertyName = DynamicPageDataServiceHandler.ShowViewButtonColumnPropertyName,
				PropertyValueConvertCallback = (arg) => { return gridViewPanel.ViewButton != null; }
			});

			// Add wrapper for DeleteButton
			results.Add(new PropertyDefinition(typeof(bool))
			{
				PropertyName = null,
				NewPropertyName = DynamicPageDataServiceHandler.ShowDeleteButtonColumnPropertyName,
				PropertyValueConvertCallback = (arg) => { return gridViewPanel.DeleteButton != null; }
			});

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
							Logger.Instance(typeof(DynamicPageDataServiceHandler)).Warn(exp);
						}
					}

					return "";

				case GridViewFieldValueTransformTypes.VariableString:
					if (fieldValue == null) return "";
					return WebUtility.ReplaceVariables(fieldValue.ToString()) ?? "";
			}

			return fieldValue.ToString();
		}

		private static void HandleException(HttpContext context, string exceptionMessage)
		{
			// Construct query result object for serialization.
			QueryResultObject queryResultObject = new QueryResultObject
			{
				HasError = true,
				ErrorMessage = exceptionMessage
			};

			string returnedValue = serializer.Serialize(queryResultObject);
			context.Response.Write(returnedValue);
		}
	}

	/// <summary>
	/// The class presents the query results which includes total records count and actually returned records.
	/// </summary>
	[DataContract(Namespace = ServiceNamespaces.Common)]
	public class QueryResultObject
	{
		/// <summary>
		/// Sets/gets total records count.
		/// </summary>
		[DataMember]
		public int TotalRecordCount { get; set; }

		/// <summary>
		/// Sets/gets actual returned records collection.
		/// </summary>
		[DataMember]
		public IEnumerable Records { get; set; }

		/// <summary>
		/// Sets/gets the start index of hit records to return.
		/// </summary>
		[DataMember]
		public int Start { get; set; }

		/// <summary>
		/// Sets/gets the limit of returned records for this call.
		/// </summary>
		[DataMember]
		public int Limit { get; set; }

		/// <summary>
		/// Sets/gets true if error occured in the server.
		/// </summary>
		[DataMember]
		public bool HasError { get; set; }

		/// <summary>
		/// Sets/gets error messages occured in the server.
		/// </summary>
		[DataMember]
		public string ErrorMessage { get; set; }
	}
}
