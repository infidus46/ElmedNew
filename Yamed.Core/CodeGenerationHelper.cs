using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;

namespace Yamed.Core
{
    static class CodeGenerationHelper
    {
        public static string GetCodeString(string namespaceName, string className, string[] keys, Type[] types)
        {
            // Create compile unit.
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            // Create namespace.
            CodeNamespace dynamicNamespace = new CodeNamespace(namespaceName);
            dynamicNamespace.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));

            // Create class.
            CodeTypeDeclaration dynamicClass = new CodeTypeDeclaration(className);
            dynamicClass.IsClass = true;
            dynamicClass.BaseTypes.Add(new CodeTypeReference("System.ComponentModel.INotifyPropertyChanged"));

            // Create PropertyChanged event; implement INotifyPropertyChanged.
            CodeMemberEvent propertyChangedEvent = new CodeMemberEvent();
            propertyChangedEvent.Name = "PropertyChanged";
            propertyChangedEvent.Type = new CodeTypeReference("System.ComponentModel.PropertyChangedEventHandler");
            propertyChangedEvent.Attributes = MemberAttributes.Public;
            dynamicClass.Members.Add(propertyChangedEvent);

            // Create RaisePropertyChanged method, and add to class.
            CodeMemberMethod raisePropertyChangedMethod = new CodeMemberMethod();
            raisePropertyChangedMethod.Attributes = MemberAttributes.Family;
            raisePropertyChangedMethod.Name = "RaisePropertyChanged";
            raisePropertyChangedMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.String", "propertyName"));
            raisePropertyChangedMethod.Statements.Add(new CodeSnippetStatement("if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }"));
            dynamicClass.Members.Add(raisePropertyChangedMethod);

            for (int i = 0; i < keys.Length; i++)
            {
                // Construct field and property names.
                string fieldName = string.Format("_{0}", keys[i].ToString());
                string propertyName = keys[i].ToString();

                // Create field.
                CodeMemberField dynamicField = new CodeMemberField(types[i], fieldName);
                //dynamicField.InitExpression = new CodeDefaultValueExpression(new CodeTypeReference(types[i]));
                dynamicClass.Members.Add(dynamicField);

                // Create property.
                CodeMemberProperty dynamicProperty = new CodeMemberProperty();
                dynamicProperty.Attributes = MemberAttributes.Public;
                dynamicProperty.Name = keys[i].ToString();
                dynamicProperty.Type = new CodeTypeReference(types[i]);

                // Create property - get statements.
                dynamicProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));

                // Create property - set statements.
                // Assign value to field.
                dynamicProperty.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));

                // Call RaisePropertyChanged method.
                CodeMethodInvokeExpression raisePropertyChangedMethodInvoke
                    = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "RaisePropertyChanged",
                        new CodeExpression[] { new CodeSnippetExpression("\"" + propertyName + "\"") });
                dynamicProperty.SetStatements.Add(raisePropertyChangedMethodInvoke);

                // Add property to class.
                dynamicClass.Members.Add(dynamicProperty);
            }

            // Add class to namespace.
            dynamicNamespace.Types.Add(dynamicClass);

            // Add namespace to compile unit.
            compileUnit.Namespaces.Add(dynamicNamespace);

            // Generate CSharp code from compile unit.
            StringWriter stringWriter = new StringWriter();
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            provider.GenerateCodeFromCompileUnit(compileUnit, stringWriter, new CodeGeneratorOptions());
            stringWriter.Close();
            return stringWriter.ToString();
        }

        public static object InstantiateClass(string codeString, string fullyQualifiedTypeName)
        {
            CSharpCodeProvider compiler = new CSharpCodeProvider();
            //CompilerParameters compilerParams = new CompilerParameters(new string[] { "System.dll" });
            CompilerParameters compilerParams = new CompilerParameters(new string[] { "System.dll" })
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                IncludeDebugInformation = false
            };
            CompilerResults results = compiler.CompileAssemblyFromSource(compilerParams, new string[] { codeString });
            return results.CompiledAssembly.CreateInstance(fullyQualifiedTypeName);
        }
    }
}