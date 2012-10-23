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
using System.Data;
using System.Data.Linq;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using Microsoft.CSharp;
using RapidWebDev.Common.CodeDom;
using System.Data.Linq.Mapping;

namespace RapidWebDev.Common.Data
{
	/// <summary>
	/// DataContext proxy factory.
	/// </summary>
	internal static class DataContextProxyFactory
	{
		private static readonly object syncObject = new object();
		private static readonly Dictionary<Type, Type> dataContextProxyTypes = new Dictionary<Type, Type>();

		/// <summary>
		/// Create an instance of the generic data context type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		internal static T Create<T>(IDbConnection connection) where T: DataContext
		{
			Type dataContextType = typeof(T);
			if (!dataContextProxyTypes.ContainsKey(dataContextType))
			{
				lock (syncObject)
				{
					if (!dataContextProxyTypes.ContainsKey(dataContextType))
					{
						Type dataContextProxyType = CreateProxyType<T>();
						dataContextProxyTypes.Add(dataContextType, dataContextProxyType);
					}
				}
			}

			return Activator.CreateInstance(dataContextProxyTypes[dataContextType], connection) as T;
		}

		/// <summary>
		/// Create proxy type which inherited from specified data context type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static Type CreateProxyType<T>() where T : DataContext
		{
			Type dataContextType = typeof(T);
			string className = string.Format(CultureInfo.InvariantCulture, "{0}__Proxy", dataContextType.Name);

			// create proxy type of data context
			CodeTypeDeclaration dataContextProxyTypeDeclaration = new CodeTypeDeclaration(className);
			dataContextProxyTypeDeclaration.BaseTypes.Add(new CodeTypeReference(dataContextType));
			dataContextProxyTypeDeclaration.TypeAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed;
			AddConstructors(dataContextProxyTypeDeclaration);

			// create override method Dispose(bool disposing).
			CodeMemberMethod disposeMethod = new CodeMemberMethod
			{
				Name = "Dispose",
				Attributes = MemberAttributes.Override | MemberAttributes.Family
			};

			disposeMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "disposing"));

			// invoke base.Dispose(disposing);
			CodeMethodReferenceExpression baseDisposeMethodReferenceExpression = new CodeMethodReferenceExpression(new CodeBaseReferenceExpression(), "Dispose");
			CodeMethodInvokeExpression baseDisposeInvokeExpression = new CodeMethodInvokeExpression(baseDisposeMethodReferenceExpression, new CodeArgumentReferenceExpression("disposing"));
			CodeExpressionStatement baseDisposeInvokeStatement = new CodeExpressionStatement(baseDisposeInvokeExpression);

			// invoke DataContextProxyFactory.OnDisposed(this);
			CodeTypeReferenceExpression dataContextProxyFactoryTypeReference = new CodeTypeReferenceExpression(typeof(DataContextFactory));
			CodeMethodReferenceExpression onDisposedReferenceExpression = new CodeMethodReferenceExpression(dataContextProxyFactoryTypeReference, "Dispose");
			CodeMethodInvokeExpression onDisposedInvokeExpression = new CodeMethodInvokeExpression(onDisposedReferenceExpression, new CodeThisReferenceExpression());
			CodeExpressionStatement onDisposedInvokeStatement = new CodeExpressionStatement(onDisposedInvokeExpression);

			// add try...catch... wraps "base.Dispose(disposing);" and "DataContextProxyFactory.OnDisposed(this);"
			CodeTryCatchFinallyStatement tryCatchFinallyStatement = new CodeTryCatchFinallyStatement(new[] { baseDisposeInvokeStatement }, new CodeCatchClause[0], new[] { onDisposedInvokeStatement });
			disposeMethod.Statements.Add(tryCatchFinallyStatement);

			// add Dispose(bool disposing) to proxy class.
			dataContextProxyTypeDeclaration.Members.Add(disposeMethod);

			// add proxy type to namespace.
			CodeNamespace dataContextProxyTypeNamespace = new CodeNamespace(dataContextType.Namespace);
			dataContextProxyTypeNamespace.Types.Add(dataContextProxyTypeDeclaration);

			Assembly compiledAssembly = CompileCodeTypeDeclaration<T>(dataContextProxyTypeNamespace);
			return (from type in compiledAssembly.GetTypes()
					where type.Name == dataContextProxyTypeDeclaration.Name
					select type).FirstOrDefault();
		}

		/// <summary>
		/// Create 4 constructors for DataContext.
		/// </summary>
		/// <param name="dataContextProxyTypeDeclaration"></param>
		private static void AddConstructors(CodeTypeDeclaration dataContextProxyTypeDeclaration)
		{
			// ctor()
			CodeConstructor constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
			dataContextProxyTypeDeclaration.Members.Add(constructor);

			// ctor(IDbConnection connection)
			constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
			constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection"));
			constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("connection"));
			dataContextProxyTypeDeclaration.Members.Add(constructor);

			// ctor(IDbConnection connection, MappingSource mapping)
			constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
			constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection"));
			constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(MappingSource), "mapping"));
			constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("connection"));
			constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("mapping"));
			dataContextProxyTypeDeclaration.Members.Add(constructor);

			// ctor(string fileOrServerOrConnection)
			constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
			constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "fileOrServerOrConnection"));
			constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("fileOrServerOrConnection"));
			dataContextProxyTypeDeclaration.Members.Add(constructor);
		}

		/// <summary>
		/// Compile CodeTypeDeclaration into runtime C# type.
		/// </summary>
		/// <param name="dataContextNamespace"></param>
		/// <returns></returns>
		private static Assembly CompileCodeTypeDeclaration<T>(CodeNamespace dataContextNamespace) where T : DataContext
		{
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			codeCompileUnit.Namespaces.Add(dataContextNamespace);
			codeCompileUnit.ReferencedAssemblies.AddRange(ResolveReferenceAssemblyNames<T>());

			string privateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath ?? AppDomain.CurrentDomain.BaseDirectory;
			CompilerParameters compilerParameters = new CompilerParameters
			{
				GenerateExecutable = false,
				GenerateInMemory = true,
				IncludeDebugInformation = false,
				CompilerOptions = string.Format(CultureInfo.InvariantCulture, @"/lib:""{0}""", privateBinPath)
			};

			CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");
			CompilerResults compilerResults = compiler.CompileAssemblyFromDom(compilerParameters, codeCompileUnit);

			if (compilerResults.Errors.HasErrors || compilerResults.Errors.HasWarnings)
			{
				StringBuilder compilerErrorBuilder = new StringBuilder();
				foreach (CompilerError error in compilerResults.Errors)
					compilerErrorBuilder.AppendFormat("Compile {0} with Error Number {1}, {2}\r\n\r\n", error.IsWarning ? "Warning" : "Error", error.ErrorNumber, error.ErrorText);

				Logger.Instance(typeof(DataContextProxyFactory)).Error(compilerErrorBuilder.ToString());
				throw new CompileException(compilerErrorBuilder.ToString());
			}

			return compilerResults.CompiledAssembly;
		}

		private static string[] ResolveReferenceAssemblyNames<T>()
		{
			HashSet<string> referencedAssemblyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			IEnumerable<AssemblyName> referencedAssemblies = new List<AssemblyName> { typeof(T).Assembly.GetName(), typeof(DataContextProxyFactory).Assembly.GetName() };
			referencedAssemblies = referencedAssemblies.Concat(typeof(DataContextProxyFactory).Assembly.GetReferencedAssemblies());
			referencedAssemblies = referencedAssemblies.Concat(typeof(T).Assembly.GetReferencedAssemblies());
			foreach (AssemblyName referencedAssemblyName in referencedAssemblies)
			{
				string assemblyName = string.Format(CultureInfo.InvariantCulture, "{0}.dll", referencedAssemblyName.Name);
				if (!referencedAssemblyNames.Contains(assemblyName))
					referencedAssemblyNames.Add(assemblyName);

				if (assemblyName.StartsWith("RapidWebDev.", StringComparison.OrdinalIgnoreCase))
					ResolveReferenceAssemblyNames(referencedAssemblyName, referencedAssemblyNames);
			}

			return referencedAssemblyNames.ToArray();
		}

		private static void ResolveReferenceAssemblyNames(AssemblyName targetAssemblyName, HashSet<string> referencedAssemblyNames)
		{
			try
			{
				Assembly targetAssembly = Assembly.Load(targetAssemblyName);
				foreach (AssemblyName referencedAssemblyName in targetAssembly.GetReferencedAssemblies())
				{
					string assemblyName = string.Format(CultureInfo.InvariantCulture, "{0}.dll", referencedAssemblyName.Name);
					if (!referencedAssemblyNames.Contains(assemblyName))
						referencedAssemblyNames.Add(assemblyName);

					if (assemblyName.StartsWith("RapidWebDev.", StringComparison.OrdinalIgnoreCase))
						ResolveReferenceAssemblyNames(referencedAssemblyName, referencedAssemblyNames);
				}
			}
			catch
			{
			}
		}
	}
}