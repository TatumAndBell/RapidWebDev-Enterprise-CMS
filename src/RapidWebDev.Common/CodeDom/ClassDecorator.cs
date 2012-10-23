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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using Microsoft.CSharp;

namespace RapidWebDev.Common.CodeDom
{
	/// <summary>
	/// Utility class to create dynamic decorate class at runtime for any classes (especially anonymous classes).
	/// </summary>
	public static class ClassDecorator
	{
		private static object syncObject = new object();
		private static Regex regexToMatchAlphabet = new Regex(@"[A-Za-z0-9_\u4e00-\u9fa5]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static readonly string DYNAMIC_CLASS_NAME_SPACE = "RapidWebDev.Common.CodeDom.DynamicClasses";
		private static Dictionary<string, Type> dynamicDecorateTypes = new Dictionary<string, Type>();
		private static Dictionary<Type, IEnumerable<PropertyInfo>> propertyInfoByType = new Dictionary<Type, IEnumerable<PropertyInfo>>();

		/// <summary>
		/// Create decorate type for input type on specified properties or all properties.
		/// If no properties specified, there creates all properties of original class into decorate class.
		/// The method is useful to create WCF data contract type dynamically with properties supporting both SET/GET, DataContract attribute on class level and DataMember attribute on property level.
		/// </summary>
		/// <param name="newTypeName"></param>
		/// <param name="propertiesToDecorate"></param>
		/// <returns></returns>
		/// <exception cref="CompileException">Exception when the specified properties configuration is incorrect.</exception>
		public static Type CreateDecorateType(string newTypeName, IEnumerable<PropertyDefinition> propertiesToDecorate)
		{
			IEnumerable<string> propertyNamesToDecorate = propertiesToDecorate.Select(p => p.PropertyName);
			string decorateTypeKey = GenerateDecorateTypeKey(newTypeName, propertyNamesToDecorate);
			if (!dynamicDecorateTypes.ContainsKey(decorateTypeKey))
			{
				lock (syncObject)
				{
					if (!dynamicDecorateTypes.ContainsKey(decorateTypeKey))
					{
						// create decorate class type
						string decorateTypeName = GenerateDecorateClassName(newTypeName, propertyNamesToDecorate);
						CodeTypeDeclaration decorateTypeDeclaration = new CodeTypeDeclaration(decorateTypeName);

						// add DataContract attribute to the decorate class
						CodeTypeReference dataContractTypeReference = new CodeTypeReference(typeof(DataContractAttribute));
						CodeAttributeArgument namespaceArgument = new CodeAttributeArgument("Namespace", new CodePrimitiveExpression(ServiceNamespaces.Common));
						CodeAttributeArgument nameArgument = new CodeAttributeArgument("Name", new CodePrimitiveExpression(FilterIllegalCharacters(newTypeName)));
						CodeAttributeDeclaration dataContractAttribute = new CodeAttributeDeclaration(dataContractTypeReference, namespaceArgument, nameArgument);
						decorateTypeDeclaration.CustomAttributes.Add(dataContractAttribute);

						// create properties
						HashSet<string> propertyNames = new HashSet<string>();
						foreach (PropertyDefinition propertyConfig in propertiesToDecorate)
						{
							Type newPropertyType = propertyConfig.NewPropertyType;
							if (newPropertyType == null)
								throw new InvalidProgramException(string.Format(CultureInfo.InvariantCulture, "NewPropertyType is not specified to decorate the type {0}.", newTypeName));

							string newPropertyName = propertyConfig.NewPropertyName ?? propertyConfig.PropertyName;
							if (!propertyNames.Contains(newPropertyName))
							{
								AddDecorateProperty(decorateTypeDeclaration, newPropertyName, newPropertyType);
								propertyNames.Add(newPropertyName);
							}
						}

						Type decorateType = CompileCodeTypeDeclaration(decorateTypeDeclaration);
						dynamicDecorateTypes[decorateTypeKey] = decorateType;
					}
				}
			}

			return dynamicDecorateTypes[decorateTypeKey];
		}

		/// <summary>
		/// Create decorate object for input object by CodeDom. 
		/// The decorate object has both SET/GET accessors on all properties with WCF attributes. 
		/// It's quite helpful on changing/convert/serialize property values for anonymous object. 
		/// </summary>
		/// <param name="inputObjects"></param>
		/// <param name="propertiesToDecorate"></param>
		/// <returns></returns>
		/// <exception cref="CompileException">Exception when the specified properties configuration is incorrect.</exception>
		public static IEnumerable CreateDecorateObjects(IEnumerable inputObjects, IEnumerable<PropertyDefinition> propertiesToDecorate)
		{
			Kit.NotNull(inputObjects, "inputObjects");

			IEnumerable<object> enumerableInputObjects = inputObjects.Cast<object>();
			int inputObjectCount = enumerableInputObjects.Count();
			if (inputObjectCount == 0) return inputObjects;

			object typicalInputObject = enumerableInputObjects.First();
			Type decorateType = CreateDecorateType(typicalInputObject.GetType().Name, propertiesToDecorate);

			Type genericListType = typeof(List<>).MakeGenericType(decorateType);
			IList returnValues = Activator.CreateInstance(genericListType) as IList;
			for (int index = 0; index < inputObjectCount; index++)
				returnValues.Add(Activator.CreateInstance(decorateType));

			IEnumerable<PropertyInfo> newProperties = GetPropertyInfoByType(decorateType);

			foreach (PropertyInfo newProperty in newProperties)
			{
				PropertyDefinition propertyDecorateConfig = propertiesToDecorate.FirstOrDefault(config => config.NewPropertyName == newProperty.Name);
				string originalPropertyName = newProperty.Name;
				bool convertPropertyType = false;
				if (propertyDecorateConfig != null)
				{
					originalPropertyName = propertyDecorateConfig.PropertyName;
					convertPropertyType = propertyDecorateConfig.NewPropertyType != null;
				}

				for (int index = 0; index < inputObjectCount; index++)
				{
					object inputObject = enumerableInputObjects.ElementAt(index);
					object propertyValue = null;

					try
					{
						propertyValue = DataBinder.Eval(inputObject, originalPropertyName);
					}
					catch
					{
						// Here has potential that the user create a new property but not replied on a existed property of the object.
						// In this case, DataBinder.Eval will throw an exception.
					}

					if (propertyDecorateConfig != null && propertyDecorateConfig.PropertyValueConvertCallback != null)
						propertyValue = propertyDecorateConfig.PropertyValueConvertCallback(propertyValue);

					if (convertPropertyType && propertyValue != null)
						propertyValue = Kit.ConvertType(propertyValue, propertyDecorateConfig.NewPropertyType);

					if (propertyValue == null)
						propertyValue = (propertyDecorateConfig.NewPropertyType != null && propertyDecorateConfig.NewPropertyType.IsValueType) ? Activator.CreateInstance(propertyDecorateConfig.NewPropertyType) : null;

					newProperty.SetValue(returnValues[index], propertyValue, null);
				}
			}

			return returnValues;
		}

		/// <summary>
		/// Get legal name by removing non-alphabet/chinese characters from input name. 
		/// Here only allows characters in [0-9] | [a-z] | [A-Z] | _ | \u4e00-\u9fa5 (Chinese Characters)
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string FilterIllegalCharacters(string name)
		{
			MatchCollection matchCollection = regexToMatchAlphabet.Matches(name);
			return matchCollection.Cast<Match>().Select(match => match.Value.Trim()).Concat("_");
		}

		/// <summary>
		/// Compile CodeTypeDeclaration into runtime C# type.
		/// </summary>
		/// <param name="decorateTypeDeclaration"></param>
		/// <returns></returns>
		private static Type CompileCodeTypeDeclaration(CodeTypeDeclaration decorateTypeDeclaration)
		{
			CodeNamespace codeNamespace = new CodeNamespace(DYNAMIC_CLASS_NAME_SPACE);
			codeNamespace.Types.Add(decorateTypeDeclaration);

			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			codeCompileUnit.Namespaces.Add(codeNamespace);
			codeCompileUnit.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");

			CompilerParameters compilerParameters = new CompilerParameters
			{
				GenerateExecutable = false,
				GenerateInMemory = true,
				IncludeDebugInformation = false
			};

			CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");
			CompilerResults compilerResults = compiler.CompileAssemblyFromDom(compilerParameters, codeCompileUnit);

			if (compilerResults.Errors.HasErrors || compilerResults.Errors.HasWarnings)
			{
				StringBuilder compilerErrorBuilder = new StringBuilder();
				foreach (CompilerError error in compilerResults.Errors)
					compilerErrorBuilder.AppendFormat("Compile {0} with Error Number {1}, {2}\r\n\r\n", error.IsWarning ? "Warning" : "Error", error.ErrorNumber, error.ErrorText);

				Logger.Instance(typeof(ClassDecorator)).Error(compilerErrorBuilder.ToString());
				throw new CompileException(compilerErrorBuilder.ToString());
			}

			Type decorateType = (from type in compilerResults.CompiledAssembly.GetTypes()
								 where type.Name == decorateTypeDeclaration.Name
								 select type).FirstOrDefault();

			return decorateType;
		}

		/// <summary>
		/// Create a dynamic property mapping to original PropertyInfo into decorate type with WCF DataMember attribute.
		/// </summary>
		/// <param name="decorateType"></param>
		/// <param name="propertyName"></param>
		/// <param name="originalPropertyType"></param>
		private static void AddDecorateProperty(CodeTypeDeclaration decorateType, string propertyName, Type originalPropertyType)
		{
			string newPropertyName = propertyName;

			// create the field which the property value stored.
			string fieldName = "_" + newPropertyName;
			CodeMemberField field = new CodeMemberField(originalPropertyType, fieldName);
			decorateType.Members.Add(field);

			// create the property
			CodeMemberProperty property = new CodeMemberProperty
			{
				Name = newPropertyName,
				Type = new CodeTypeReference(originalPropertyType),
				HasGet = true,
				HasSet = true,
				Attributes = MemberAttributes.Public | MemberAttributes.Final
			};

			// create DataMember attribute on the property
			CodeTypeReference dataMemberTypeReference = new CodeTypeReference(typeof(DataMemberAttribute));
			property.CustomAttributes.Add(new CodeAttributeDeclaration(dataMemberTypeReference));

			// create GETTER for the property
			CodeFieldReferenceExpression codeFieldReferenceExpression = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName);
			CodeMethodReturnStatement methodReturnStatement = new CodeMethodReturnStatement(codeFieldReferenceExpression);
			property.GetStatements.Add(methodReturnStatement);

			// create SETTER for the property
			CodeAssignStatement codeAssignStatement = new CodeAssignStatement(codeFieldReferenceExpression, new CodePropertySetValueReferenceExpression());
			property.SetStatements.Add(codeAssignStatement);

			decorateType.Members.Add(property);
		}

		/// <summary>
		/// Generate key for dynamic created decorate class type.
		/// </summary>
		/// <param name="inputType"></param>
		/// <param name="propertiesToDecorate"></param>
		/// <returns></returns>
		private static string GenerateDecorateTypeKey(Type inputType, IEnumerable<string> propertiesToDecorate)
		{
			string inputTypeName = null;
			if (inputType != null)
				inputTypeName = inputType.FullName;

			string propertiesIdentifier = null;
			if (propertiesToDecorate != null && propertiesToDecorate.Count() > 0)
				propertiesIdentifier = propertiesToDecorate.OrderBy(propertyName => propertyName).Concat(", ");

			return string.Format(CultureInfo.InvariantCulture, "{0} With {1}", inputTypeName, propertiesIdentifier);
		}

		/// <summary>
		/// Generate key for dynamic created decorate class type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="propertiesToDecorate"></param>
		/// <returns></returns>
		private static string GenerateDecorateTypeKey(string typeName, IEnumerable<string> propertiesToDecorate)
		{
			string inputTypeName = typeName;
			string propertiesIdentifier = null;
			if (propertiesToDecorate != null && propertiesToDecorate.Count() > 0)
				propertiesIdentifier = propertiesToDecorate.OrderBy(propertyName => propertyName).Concat(", ");

			return string.Format(CultureInfo.InvariantCulture, "{0} With {1}", inputTypeName, propertiesIdentifier);
		}

		/// <summary>
		/// Generate class name for decorate class by input type and decorating properties.
		/// </summary>
		/// <param name="inputType"></param>
		/// <param name="propertiesToDecorate"></param>
		/// <returns></returns>
		private static string GenerateDecorateClassName(Type inputType, IEnumerable<string> propertiesToDecorate)
		{
			string inputTypeName = null;
			if (inputType != null)
				inputTypeName = FilterIllegalCharacters(inputType.Name);

			string propertiesIdentifier = null;
			if (propertiesToDecorate != null && propertiesToDecorate.Count() > 0)
				propertiesIdentifier = propertiesToDecorate.OrderBy(propertyName => propertyName).Select(propertyName => GeneratePropertySnapshot(propertyName)).Concat("_");

			return string.Format(CultureInfo.InvariantCulture, "{0}_", inputTypeName, propertiesIdentifier);
		}

		/// <summary>
		/// Generate class name for decorate class by input type and decorating properties.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="propertiesToDecorate"></param>
		/// <returns></returns>
		private static string GenerateDecorateClassName(string typeName, IEnumerable<string> propertiesToDecorate)
		{
			string inputTypeName = FilterIllegalCharacters(typeName);
			string propertiesIdentifier = null;
			if (propertiesToDecorate != null && propertiesToDecorate.Count() > 0)
				propertiesIdentifier = propertiesToDecorate.OrderBy(propertyName => propertyName).Select(propertyName => GeneratePropertySnapshot(propertyName)).Concat("_");

			return string.Format(CultureInfo.InvariantCulture, "{0}_", inputTypeName, propertiesIdentifier);
		}

		/// <summary>
		/// Generate snapshot for the property.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		private static string GeneratePropertySnapshot(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName)) return "";

			string naturePropertyName = propertyName.Split(new char[] { '[', '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
			if (naturePropertyName.Length > 1)
				return naturePropertyName.Substring(0, 2) + naturePropertyName.Length;
			else
				return naturePropertyName + "1";
		}

		/// <summary>
		/// Get all property info of specified type using caching.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static IEnumerable<PropertyInfo> GetPropertyInfoByType(Type type)
		{
			if (!propertyInfoByType.ContainsKey(type))
			{
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				propertyInfoByType[type] = properties;
			}

			return propertyInfoByType[type];
		}
	}
}

