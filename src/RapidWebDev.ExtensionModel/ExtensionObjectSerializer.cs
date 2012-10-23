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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel.Properties;
using System.Globalization;
using RapidWebDev.Common.Validation;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.ExtensionModel
{

	/// <summary>
	/// Support extension field's value and extension field convert operations.
	/// </summary>
	public class ExtensionObjectSerializer : IExtensionObjectSerializer
	{
		private static DataContractSerializer serializer = new DataContractSerializer(typeof(FieldCollection));
		private IMetadataApi metadataApi;


		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionObjectSerializer"/> class.
		/// </summary>
		/// <param name="metadataApi">The metadata API.</param>
		public ExtensionObjectSerializer(IMetadataApi metadataApi)
		{
			this.metadataApi = metadataApi;
		}

		/// <summary>
		/// Parse out extension field collection from extension object's extension field (ExtensionData).
		/// </summary>
		/// <param name="extensionObject"></param>
		/// <returns></returns>
		public IDictionary<string, object> Deserialize(IExtensionObject extensionObject)
		{
			if (Kit.IsEmpty(extensionObject.ExtensionData)) return new Dictionary<string, object>();
			StringBuilder output = new StringBuilder();

			XmlWriterSettings settings = new XmlWriterSettings()
			{
				Encoding = Encoding.UTF8,
				Indent = true
			};

			using (StringReader stringReader = new StringReader(extensionObject.ExtensionData))
			using (XmlReader xmlReader = XmlReader.Create(stringReader))
			{
				FieldCollection fieldValues = serializer.ReadObject(xmlReader) as FieldCollection;
				return fieldValues.ToDictionary(kvp => kvp.Name, kvp => 
					{
						// convert Utc datetime into client timezone for Date/DateTime fields.
						IFieldValue fieldValue = kvp.Value;
						DateTimeFieldValue dateTimeFieldValue = fieldValue as DateTimeFieldValue;
						if (dateTimeFieldValue == null) return fieldValue.Value;

						return LocalizationUtility.ConvertUtcTimeToClientTime(dateTimeFieldValue.Value);
					});
			}
		}

		/// <summary>
		/// Save extension field serilized into extension field (ExtensionData)
		/// </summary>
		/// <param name="extensionObject"></param>
		public void Serialize(IExtensionObject extensionObject)
		{
			FieldCollection fieldValues = new FieldCollection();
			IEnumerator<KeyValuePair<string, object>> fieldNameValueEnumerator = extensionObject.GetFieldEnumerator();
			while (fieldNameValueEnumerator.MoveNext())
			{
				KeyValuePair<string, object> fieldNameValuePair = fieldNameValueEnumerator.Current;

				using (ValidationScope validationScope = new ValidationScope())
				{
					try
					{
						IFieldValue fieldValue = ConvertToFieldValueInterface(extensionObject.ExtensionDataTypeId, fieldNameValuePair.Key, fieldNameValuePair.Value);
						if (fieldValue != null) fieldValues.Add(new FieldNameValuePair(fieldNameValuePair.Key, fieldValue));
					}
					catch (InvalidFieldValueException exp)
					{
						validationScope.Error(exp.Message);
					}
					catch (NotSupportedException exp)
					{
						validationScope.Error(exp.Message);
					}
				}
			}

			StringBuilder output = new StringBuilder();

			XmlWriterSettings settings = new XmlWriterSettings()
			{
				Encoding = Encoding.UTF8,
				Indent = true
			};

			using (XmlWriter writer = XmlWriter.Create(output, settings))
			{
				serializer.WriteObject(writer, fieldValues);
			}

			extensionObject.ExtensionData = output.ToString();
		}

		/// <summary>
		/// Convert propertyValue to IFieldValue interface
		/// </summary>
		/// <param name="extensionDataTypeId"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		/// <returns>IFieldValue interface</returns>
		/// <exception cref="NotSupportedException">when propertyValue's type is not supported</exception>
		/// <exception cref="InvalidFieldValueException">When property's value is invalid</exception>
		private IFieldValue ConvertToFieldValueInterface(Guid extensionDataTypeId, string propertyName, object propertyValue)
		{
			using (ValidationScope validationScope = new ValidationScope())
			{
				IFieldValue fieldValue = null;

				try
				{
					IFieldMetadata fieldMetadata = this.metadataApi.GetField(extensionDataTypeId, propertyName);
					if (fieldMetadata != null)
					{
						fieldValue = ConvertToFieldValueInterface(fieldMetadata, propertyName, propertyValue);
						fieldMetadata.Validate(fieldValue);
						if (fieldValue == null || fieldValue.Value == null)
							fieldValue = fieldMetadata.GetDefaultValue();
					}
					else
						fieldValue = ConvertToFieldValueInterface(null, propertyName, propertyValue);
				}
				catch (InvalidSaaSApplicationException)
				{
					fieldValue = ConvertToFieldValueInterface(null, propertyName, propertyValue);
				}

				return fieldValue;
			}
		}

		/// <summary>
		/// Convert propertyValue to IFieldValue interface
		/// </summary>
		/// <param name="fieldMetadata">The field metadata of specified property name.</param>
		/// <param name="propertyName">Property name.</param>
		/// <param name="propertyValue">Property value.</param>
		/// <returns>IFieldValue interface</returns>
		/// <exception cref="NotSupportedException">When propertyValue's type is not supported</exception>
		private static IFieldValue ConvertToFieldValueInterface(IFieldMetadata fieldMetadata, string propertyName, object propertyValue)
		{
			if (propertyValue == null) return null;

			#region If field metadata is predefined.

			if (fieldMetadata != null)
			{
				string exceptionMessage = string.Format(CultureInfo.InvariantCulture, "The property \"{0}\" with value \"{1}\" doesn't match the field metadata type \"{2}\".", propertyName, propertyValue, fieldMetadata.Type);
				switch (fieldMetadata.Type)
				{
					case FieldType.DateTime:
						if (propertyValue is DateTime)
							return LocalizationUtility.ConvertClientTimeToUtcTime((DateTime)propertyValue).FieldValue();
						else if (propertyValue is DateTime?)
						{
							DateTime? dateTimePropertyValue = (DateTime?)propertyValue;
							if (!dateTimePropertyValue.HasValue) return null;
							return LocalizationUtility.ConvertClientTimeToUtcTime(dateTimePropertyValue.Value).FieldValue();
						}
						else
							throw new NotSupportedException(exceptionMessage);

					case FieldType.Decimal:
						if (propertyValue is decimal)
							return ((decimal)propertyValue).FieldValue();
						else if (propertyValue is decimal?)
							return ((decimal?)propertyValue).FieldValue();
						else
							throw new NotSupportedException(exceptionMessage);

					case FieldType.Hierarchy:
						if (propertyValue is HierarchyNodeValueCollection)
							return (propertyValue as HierarchyNodeValueCollection).FieldValue();
						else
							return new HierarchyNodeValueCollection { propertyValue.ToString() }.FieldValue();

					case FieldType.Integer:
						if (propertyValue is int)
							return ((int)propertyValue).FieldValue();
						else if (propertyValue is int?)
							return ((int?)propertyValue).FieldValue();
						else
							throw new NotSupportedException(exceptionMessage);

					case FieldType.Enumeration:
						if (propertyValue is EnumerationValueCollection)
							return (propertyValue as EnumerationValueCollection).FieldValue();
						else
							return new EnumerationValueCollection { propertyValue.ToString() }.FieldValue();

					case FieldType.String:
						if (propertyValue is string)
							return (propertyValue as string).FieldValue();
						else
							return propertyValue.ToString().FieldValue();
				}
			}
			#endregion

			#region if field metadata is undefined, here trys to guess the property type

			if (propertyValue is string)
				return (propertyValue as string).FieldValue();

			else if (propertyValue is DateTime)
				return LocalizationUtility.ConvertClientTimeToUtcTime((DateTime)propertyValue).FieldValue();
			else if (propertyValue is DateTime?)
			{
				DateTime? dateTimePropertyValue = (DateTime?)propertyValue;
				if (!dateTimePropertyValue.HasValue) return null;
				return LocalizationUtility.ConvertClientTimeToUtcTime(dateTimePropertyValue.Value).FieldValue();
			}

			else if (propertyValue is decimal)
				return ((decimal)propertyValue).FieldValue();
			else if (propertyValue is decimal?)
				return ((decimal?)propertyValue).FieldValue();

			else if (propertyValue is int)
				return ((int)propertyValue).FieldValue();
			else if (propertyValue is int?)
				return ((int?)propertyValue).FieldValue();

			else if (propertyValue is HierarchyNodeValueCollection)
				return (propertyValue as HierarchyNodeValueCollection).FieldValue();

			else if (propertyValue is EnumerationValueCollection)
				return (propertyValue as EnumerationValueCollection).FieldValue();

			throw new NotSupportedException(string.Format(Resources.FieldNotSupportSpecifiedValueType, propertyName, propertyValue.GetType()));

			#endregion
		}
	}
}
