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

namespace RapidWebDev.UI.DynamicPages
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Configuration;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls.WebParts;
	using RapidWebDev.Common;
	using RapidWebDev.Common.Validation;
	using RapidWebDev.UI.Controls;
	using RapidWebDev.UI.DynamicPages.Configurations;
	using RapidWebDev.UI.DynamicPages.Resolvers;
	using RapidWebDev.UI.Properties;
	using ListItem = System.Web.UI.WebControls.ListItem;
	using RapidWebDev.Common.Globalization;

	/// <summary>
	/// Factory to produce QueryField controls.
	/// </summary>
	public static class QueryFieldControlFactory
	{
		/// <summary>
		/// The query field control types which use Between as default query operator.
		/// </summary>
		private static readonly HashSet<ControlConfigurationTypes> TypesUseDefaultOperatorBetween = new HashSet<ControlConfigurationTypes> 
		{
			ControlConfigurationTypes.Date,
			ControlConfigurationTypes.DateTime,
			ControlConfigurationTypes.Integer,
			ControlConfigurationTypes.Decimal
		};

		/// <summary>
		/// The query field control types occupied the whole line.
		/// </summary>
		private static readonly HashSet<ControlConfigurationTypes> TypesOccupiedWholeLine = new HashSet<ControlConfigurationTypes> 
		{
			ControlConfigurationTypes.CheckBoxGroup,
			ControlConfigurationTypes.RadioGroup
		};

		/// <summary>
		/// Create Query controls from configuration.
		/// </summary>
		/// <param name="queryFieldConfiguration"></param>
		/// <returns></returns>
		public static IEnumerable<IQueryFieldControl> CreateQueryControls(QueryFieldConfiguration queryFieldConfiguration)
		{
			bool useBetween = false;
			if (queryFieldConfiguration.Operator == QueryFieldOperators.Between)
				useBetween = true;

			if (queryFieldConfiguration.Operator == QueryFieldOperators.Auto && TypesUseDefaultOperatorBetween.Contains(queryFieldConfiguration.Control.ControlType))
				useBetween = true;

			IQueryFieldControl[] queryFieldControls;
			if (!useBetween)
				queryFieldControls = new[] { CreateQueryFieldControl(queryFieldConfiguration) };
			else
				queryFieldControls = new[] { CreateQueryFieldControl(queryFieldConfiguration), CreateQueryFieldControl(queryFieldConfiguration) };

			return queryFieldControls;
		}

		/// <summary>
		/// Calculate how many cells the query field control occupied in rendering UI.
		/// </summary>
		/// <param name="queryFieldConfiguration"></param>
		/// <returns></returns>
		public static int CalculateQueryControlOccupation(QueryFieldConfiguration queryFieldConfiguration)
		{
			if (queryFieldConfiguration.Occupation > 0)
				return queryFieldConfiguration.Occupation;

			switch (queryFieldConfiguration.Operator)
			{
				case QueryFieldOperators.Between:
					return 2;
				case QueryFieldOperators.In:
				case QueryFieldOperators.NotIn:
					return int.MaxValue;
				case QueryFieldOperators.Auto:
					if (TypesUseDefaultOperatorBetween.Contains(queryFieldConfiguration.Control.ControlType)) return 2;
					else if (TypesOccupiedWholeLine.Contains(queryFieldConfiguration.Control.ControlType)) return 999;
					else return 1;
				default:
					return 1;
			}
		}

		/// <summary>
		/// Build <seealso cref="QueryParameterExpressionCollection"/> from collection of queryFieldConfigurations and posted values.
		/// </summary>
		/// <param name="queryFieldConfigurations"></param>
		/// <param name="postedValuesByControlIndex"></param>
		/// <returns></returns>
		public static QueryParameterExpressionCollection BuildQueryParameterExpressions(IEnumerable<QueryFieldConfiguration> queryFieldConfigurations, IEnumerable<KeyValuePair<int, string>> postedValuesByControlIndex)
		{
			QueryParameterExpressionCollection results = new QueryParameterExpressionCollection();
			IDictionary<int, QueryFieldConfiguration> queryFieldConfigurationByControlIndex = BuildQueryFieldConfiguration(queryFieldConfigurations);
			IControlValueResolverFactory controlValueResolverFactory = SpringContext.Current.GetObject<IControlValueResolverFactory>();
			foreach (KeyValuePair<int, string> postedVaule in postedValuesByControlIndex)
			{
				if (postedVaule.Key >= queryFieldConfigurationByControlIndex.Count)
					throw new ValidationException(string.Format(CultureInfo.InvariantCulture, Resources.DPCtrl_PostedValueControlIndexInvalid, postedVaule.Key));

				QueryFieldConfiguration queryFieldConfiguration = queryFieldConfigurationByControlIndex[postedVaule.Key];
				object fieldValue = controlValueResolverFactory.Resolve(queryFieldConfiguration, postedVaule.Value);
				results.Add(new QueryParameterExpression(queryFieldConfiguration.FieldName, queryFieldConfiguration.Operator, fieldValue, queryFieldConfiguration));
			}

			return results;
		}

		/// <summary>
		/// Assign ID and index to input enumerable query field controls.
		/// </summary>
		/// <param name="queryFieldControls"></param>
		public static void AssignQueryFieldControlIDAndIndex(IEnumerable<IQueryFieldControl> queryFieldControls)
		{
			int controlCount = 0;
			foreach (IQueryFieldControl queryFieldControl in queryFieldControls)
			{
				queryFieldControl.ControlIndex = controlCount++;
				queryFieldControl.ID = "QueryFieldControl" + queryFieldControl.ControlIndex.ToString();
			}
		}

		#region Private methods to create QueryField controls

		private static IQueryFieldControl CreateQueryFieldControl(QueryFieldConfiguration queryFieldConfiguration)
		{
			switch (queryFieldConfiguration.Control.ControlType)
			{
				case ControlConfigurationTypes.CheckBox:
					return CreateCheckBox(queryFieldConfiguration.Control as CheckBoxConfiguration);

				case ControlConfigurationTypes.CheckBoxGroup:
					return CreateCheckBoxGroup(queryFieldConfiguration.Control as CheckBoxGroupConfiguration);

				case ControlConfigurationTypes.ComboBox:
					return CreateComboBox(queryFieldConfiguration.Control as ComboBoxConfiguration);

				case ControlConfigurationTypes.Date:
					return CreateDatePicker(queryFieldConfiguration.Control as DateConfiguration);
					
				case ControlConfigurationTypes.DateTime:
					return CreateDateTimePicker(queryFieldConfiguration.Control as DateTimeConfiguration);

				case ControlConfigurationTypes.Decimal:
					return CreateDecimalTextBox(queryFieldConfiguration.Control as DecimalConfiguration);

				case ControlConfigurationTypes.Integer:
					return CreateIntegerTextBox(queryFieldConfiguration.Control as IntegerConfiguration);

				case ControlConfigurationTypes.RadioGroup:
					return CreateRadioGroup(queryFieldConfiguration.Control as RadioGroupConfiguration);

				case ControlConfigurationTypes.TextBox:
					return CreateTextBox(queryFieldConfiguration.Control as TextBoxConfiguration);

				case ControlConfigurationTypes.HierarchySelector:
					return CreateHierarchySelector(queryFieldConfiguration.Control as HierarchySelectorConfiguration);

				case ControlConfigurationTypes.Custom:
					return CreateCustomControl(queryFieldConfiguration.Control as CustomConfiguration);
			}

			throw new NotSupportedException(Resources.DPCtrl_QueryFieldControlNotSupport);
		}

		private static CheckBox CreateCheckBox(CheckBoxConfiguration controlConfiguration)
		{
			return new CheckBox { Text = controlConfiguration.Text, Checked = controlConfiguration.Checked };
		}

		private static CheckBoxGroup CreateCheckBoxGroup(CheckBoxGroupConfiguration controlConfiguration)
		{
			CheckBoxGroup checkBoxGroup = new CheckBoxGroup();
			foreach (CheckBoxGroupItemConfiguration checkBoxGroupItemConfiguration in controlConfiguration.Items)
			{
				ListItem listItem = new ListItem(checkBoxGroupItemConfiguration.Text, checkBoxGroupItemConfiguration.Value) { Selected = checkBoxGroupItemConfiguration.Checked };
				checkBoxGroup.Items.Add(listItem);
			}

			return checkBoxGroup;
		}

		private static ComboBox CreateComboBox(ComboBoxConfiguration controlConfiguration)
		{
			ComboBox combobox = new ComboBox
			{
				Editable = controlConfiguration.Editable,
				ForceSelection = controlConfiguration.ForceSelection,
				MinChars = controlConfiguration.MinChars,
				SelectedItemChangedCallback = controlConfiguration.OnSelectedIndexChanged
			};

			if (controlConfiguration.DynamicDataSource != null)
			{
				combobox.Mode = ComboBoxDataSourceModes.Remote;
				combobox.Url = Kit.ResolveAbsoluteUrl(controlConfiguration.DynamicDataSource.Url);
				combobox.Method = controlConfiguration.DynamicDataSource.Method;
				combobox.Proxy = controlConfiguration.DynamicDataSource.Proxy;
				combobox.ItemSelector = controlConfiguration.DynamicDataSource.ItemSelector;
				combobox.QueryParam = controlConfiguration.DynamicDataSource.QueryParam;
				combobox.Root = controlConfiguration.DynamicDataSource.Root;
				combobox.TextField = controlConfiguration.DynamicDataSource.TextField;
				combobox.ValueField = controlConfiguration.DynamicDataSource.ValueField;
				combobox.XTemplate = controlConfiguration.DynamicDataSource.XTemplate;

				foreach (ComboBoxDynamicDataSourceParamConfiguration parameter in controlConfiguration.DynamicDataSource.Params)
					combobox.Params.Add(new ComboBoxDynamicDataSourceParamConfiguration(parameter.Name) { Value = parameter.Value });
			}
			else if (controlConfiguration.StaticDataSource != null)
			{
				foreach (ComboBoxItemConfiguration itemConfiguration in controlConfiguration.StaticDataSource)
					combobox.Items.Add(new ListItem(itemConfiguration.Text, itemConfiguration.Value) { Selected = itemConfiguration.Checked });
			}

			return combobox;
		}

		private static DatePicker CreateDatePicker(DateConfiguration controlConfiguration)
		{
			DateTime? defaultDate = ResolveDefaultDateTime(controlConfiguration.DefaultDate);
			string defaultDateString = "";
			if (defaultDate.HasValue)
				defaultDateString = defaultDate.Value.ToString("yyyy-MM-dd");

			return new DatePicker 
			{
				MinValue = controlConfiguration.MinValue,
				MaxValue = controlConfiguration.MaxValue,
				Text = defaultDateString,
				Width = new System.Web.UI.WebControls.Unit("100px")
			};
		}

		private static DateTimePicker CreateDateTimePicker(DateTimeConfiguration controlConfiguration)
		{
			DateTime? defaultDate = ResolveDefaultDateTime(controlConfiguration.DefaultDate, controlConfiguration.DefaultTime);
			string defaultDateString = "";
			if (defaultDate.HasValue)
				defaultDateString = defaultDate.Value.ToString("yyyy-MM-dd HH:mm");

			return new DateTimePicker
			{
				MinValue = controlConfiguration.MinValue,
				MaxValue = controlConfiguration.MaxValue,
				Text = defaultDateString,
				Width = new System.Web.UI.WebControls.Unit("130px")
			};
		}

		private static DecimalTextBox CreateDecimalTextBox(DecimalConfiguration controlConfiguration)
		{
			return new DecimalTextBox
			{
				AllowNegative = controlConfiguration.AllowNegative,
				DecimalPrecision = controlConfiguration.DecimalPrecision,
			};
		}

		private static IntegerTextBox CreateIntegerTextBox(IntegerConfiguration controlConfiguration)
		{
			return new IntegerTextBox
			{
				AllowNegative = controlConfiguration.AllowNegative
			};
		}

		private static RadioGroup CreateRadioGroup(RadioGroupConfiguration controlConfiguration)
		{
			RadioGroup radioGroup = new RadioGroup();
			foreach (RadioGroupItemConfiguration radioGroupItemConfiguration in controlConfiguration.Items)
				radioGroup.Items.Add(new ListItem(radioGroupItemConfiguration.Text, radioGroupItemConfiguration.Value) { Selected = radioGroupItemConfiguration.Checked });

			return radioGroup;
		}

		private static TextBox CreateTextBox(TextBoxConfiguration controlConfiguration)
		{
			return new TextBox();
		}

		private static IQueryFieldControl CreateCustomControl(CustomConfiguration controlConfiguration)
		{
			try
			{
				IQueryFieldControl queryFieldControl = Activator.CreateInstance(controlConfiguration.Type) as IQueryFieldControl;
				if (queryFieldControl == null)
					throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, Resources.DPCtrl_InvalidCustomControlType, controlConfiguration.Type));

				return queryFieldControl;
			}
			catch (NotImplementedException)
			{
				throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, Resources.DPCtrl_CustomControlTypeNotFound, controlConfiguration.Type));
			}
		}

		private static DateTime? ResolveDefaultDateTime(DefaultDateValues defaultDateValue)
		{
			DateTime now = LocalizationUtility.ConvertUtcTimeToClientTime(DateTime.UtcNow);
			switch (defaultDateValue)
			{
				case DefaultDateValues.FirstDayOfMonth:
					return new DateTime(now.Year, now.Month, 1);

				case DefaultDateValues.FirstDayOfWeek:
					int dayOfWeek = (int)now.DayOfWeek;
					if (dayOfWeek == 0) return now.AddDays(-6d);
					return now.AddDays(dayOfWeek - 1);

				case DefaultDateValues.FirstDayOfYeay:
					return new DateTime(now.Year, 1, 1);

				case DefaultDateValues.Today:
					return now;

				default:
					return (DateTime?)null;
			}
		}

		private static DateTime? ResolveDefaultDateTime(DefaultDateValues defaultDateValue, DefaultTimeValues defaultTimeValue)
		{
			DateTime returnedValue;
			DateTime now = LocalizationUtility.ConvertUtcTimeToClientTime(DateTime.UtcNow);
			switch (defaultDateValue)
			{
				case DefaultDateValues.FirstDayOfMonth:
					returnedValue = new DateTime(now.Year, now.Month, 1);
					break;

				case DefaultDateValues.FirstDayOfWeek:
					int dayOfWeek = (int)now.DayOfWeek;
					if (dayOfWeek == 0) return now.AddDays(-6d);
					returnedValue = now.AddDays(dayOfWeek - 1);
					break;

				case DefaultDateValues.FirstDayOfYeay:
					returnedValue = new DateTime(now.Year, 1, 1);
					break;

				case DefaultDateValues.Today:
					returnedValue = now;
					break;

				default:
					return (DateTime?)null;
			}

			switch (defaultTimeValue)
			{
				case DefaultTimeValues.None:
					return returnedValue;

				case DefaultTimeValues.Now:
					return returnedValue.Add(new TimeSpan(0, now.Hour, now.Minute, now.Second, now.Millisecond));

				default:
					string s = defaultTimeValue.ToString().Substring(1);
					int hourNumber;
					if (int.TryParse(s, out hourNumber))
						returnedValue = returnedValue.AddHours(hourNumber);

					return returnedValue;
			}
		}

		private static HierarchySelector CreateHierarchySelector(HierarchySelectorConfiguration controlConfiguration)
		{
			return new HierarchySelector
			{
				ServiceUrl = Kit.ResolveAbsoluteUrl(controlConfiguration.ServiceUrl),
				TextField = controlConfiguration.TextField,
				ValueField = controlConfiguration.ValueField,
				ParentValueField = controlConfiguration.ParentValueField,
				Title = controlConfiguration.Title,
				Cascading = controlConfiguration.Cascading
			};
		}

		#endregion

		#region Private methods to BuildQueryParameterExpressions

		private static IDictionary<int, QueryFieldConfiguration> BuildQueryFieldConfiguration(IEnumerable<QueryFieldConfiguration> queryFieldConfigurations)
		{
			Dictionary<int, QueryFieldConfiguration> queryFieldConfigurationByControlIndex = new Dictionary<int, QueryFieldConfiguration>();
			foreach (QueryFieldConfiguration queryFieldConfiguration in queryFieldConfigurations)
			{
				int controlCount = CalculateQueryFieldControlCount(queryFieldConfiguration);
				if (controlCount == 1)
				{
					queryFieldConfigurationByControlIndex.Add(queryFieldConfigurationByControlIndex.Count, queryFieldConfiguration.Clone());
				}
				else if (controlCount == 2)
				{
					QueryFieldConfiguration queryFieldConfigurationStart = queryFieldConfiguration.Clone();
					queryFieldConfigurationStart.Operator = QueryFieldOperators.GreaterThan;
					queryFieldConfigurationByControlIndex.Add(queryFieldConfigurationByControlIndex.Count, queryFieldConfigurationStart);

					QueryFieldConfiguration queryFieldConfigurationEnd = queryFieldConfiguration.Clone();
					queryFieldConfigurationEnd.Operator = QueryFieldOperators.LessEqualThan;
					queryFieldConfigurationByControlIndex.Add(queryFieldConfigurationByControlIndex.Count, queryFieldConfigurationEnd);
				}
			}

			return queryFieldConfigurationByControlIndex;
		}

		private static int CalculateQueryFieldControlCount(QueryFieldConfiguration queryFieldConfiguration)
		{
			switch (queryFieldConfiguration.Operator)
			{
				case QueryFieldOperators.Between:
					return 2;
				case QueryFieldOperators.Auto:
					if (TypesUseDefaultOperatorBetween.Contains(queryFieldConfiguration.Control.ControlType)) return 2;
					else return 1;
				default:
					return 1;
			}
		}

		#endregion
	}
}

