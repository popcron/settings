using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Popcron.Settings
{
    public class SettingsGenerator
    {
        private const string Indent = "    ";
        private const string InstanceName = "current";
        private const string TypeName = "Settings";
        private const string SaveMethodName = "Save";
        private const string LoadMethodName = "Load";
        private const bool PrettyPrint = true;

        public static void GenerateClass()
        {
            Settings settings = Settings.Current;
            StringBuilder fileContents = new StringBuilder();
            string template = settings.classTemplate.text;
            template = Regex.Replace(template, "\r(?=\\S)", "");

            //grab the field and property templates
            string fieldTemplate = PrefixLines(CutRegion(template, "FIELD"), Indent);
            string propertyTemplate = PrefixLines(CutRegion(template, "PROPERTY"), Indent);

            //dump the whole template into the builder
            fileContents.AppendLine("using UnityEngine;");
            fileContents.AppendLine("using System.IO;");

            //add if collections are present
            if (NeedsGenericCollections(settings))
            {
                fileContents.AppendLine("using System.Collections.Generic;");
            }

            fileContents.AppendLine("");
            fileContents.Append("public class ");
            fileContents.AppendLine(TypeName);
            fileContents.AppendLine("{");

            CreateInstanceVariable(fileContents);
            fileContents.AppendLine();

            CreatePathProperty(fileContents);
            fileContents.AppendLine();

            CreateStaticConstructor(fileContents);
            fileContents.AppendLine();

            CreateLoadMethod(fileContents);
            fileContents.AppendLine();

            CreateSaveMethod(fileContents);
            fileContents.AppendLine();

            //add all properties to the builder
            for (int i = 0; i < settings.properties.Length; i++)
            {
                ref SettingsProperty property = ref settings.properties[i];
                string propertyType = property.type;
                string description = property.description;
                bool isArray = propertyType.EndsWith("[]");
                if (isArray)
                {
                    //it array! so make it a list instead
                    propertyType = $"List<{propertyType.TrimEnd('[', ']')}>";
                }

                string defaultValue = property.defaultValue;

                //default value is a json, so convert back
                string json = JsonConvert.ToString(defaultValue);
                TypeHandler handler = TypeHandler.Find(property.type);
                if (handler != null)
                {
                    Type fakeType = handler.FakeType;
                    if (isArray)
                    {
                        fakeType = fakeType.MakeArrayType();
                    }

                    object fakeValue = JsonConvert.DeserializeObject(defaultValue, fakeType);
                    defaultValue = handler.GetLine(fakeValue);
                    propertyType = handler.GetTypeName(isArray);
                }
                else
                {
                    defaultValue = $"({propertyType})JsonConvert.DeserializeObject({json}, typeof({propertyType}))";
                }

                string propertyName = property.name.Replace(" ", "");
                string fieldName = ToFieldName(propertyName);
                string fieldString = fieldTemplate.Replace("{FieldName}", fieldName);
                fieldString = fieldString.Replace("{PropertyName}", propertyName);
                fieldString = fieldString.Replace("{PropertyType}", propertyType);
                fieldString = fieldString.Replace("{Instance}", InstanceName);
                fieldString = fieldString.Replace("{InstanceType}", TypeName);
                fieldString = fieldString.Replace("{DefaultValue}", defaultValue);
                fileContents.Append(fieldString);

                fileContents.AppendLine();
                fileContents.AppendLine();

                //add the xml comment
                if (!string.IsNullOrEmpty(description))
                {
                    fileContents.Append(Indent);
                    fileContents.AppendLine("/// <summary>");
                    fileContents.Append(Indent);
                    fileContents.AppendLine($"/// {description}");
                    fileContents.Append(Indent);
                    fileContents.AppendLine("/// </summary>");
                }

                string propertyString = propertyTemplate.Replace("{FieldName}", fieldName);
                propertyString = propertyString.Replace("{PropertyName}", propertyName);
                propertyString = propertyString.Replace("{PropertyType}", propertyType);
                propertyString = propertyString.Replace("{Instance}", InstanceName);
                propertyString = propertyString.Replace("{InstanceType}", TypeName);
                propertyString = propertyString.Replace("{DefaultValue}", defaultValue);
                propertyString = propertyString.Replace("{LoadMethod}", SaveMethodName);
                propertyString = propertyString.Replace("{SaveMethod}", LoadMethodName);
                fileContents.Append(propertyString);

                if (i != settings.properties.Length - 1)
                {
                    fileContents.AppendLine();
                    fileContents.AppendLine();
                }
            }

            fileContents.AppendLine();
            fileContents.AppendLine("}");
            File.WriteAllText(settings.pathToClass, fileContents.ToString());
            AssetDatabase.Refresh();
        }

        private static bool NeedsGenericCollections(Settings settings)
        {
            for (int i = 0; i < settings.properties.Length; i++)
            {
                ref SettingsProperty property = ref settings.properties[i];
                string propertyType = property.type;
                string description = property.description;
                bool isArray = propertyType.EndsWith("[]");
                if (isArray)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Converts this to a better field name.
        /// </summary>
        private static string ToFieldName(string propertyName)
        {
            char firstChar = propertyName[0];
            if (char.IsUpper(firstChar))
            {
                //is second char also upper?
                if (!char.IsUpper(propertyName[1]))
                {
                    firstChar = char.ToLower(firstChar);
                    propertyName = propertyName.Remove(0, 1);
                    propertyName = firstChar + propertyName;
                    return propertyName;
                }
                else
                {
                    //more uppercases, default out
                }
            }

            //add the fugly m_ prefix
            return $"m_{propertyName}";
        }

        /// <summary>
        /// Creates a static FilePath property.
        /// </summary>
        private static void CreatePathProperty(StringBuilder fileContents)
        {
            //property sig
            fileContents.Append(Indent);
            fileContents.AppendLine("private static string FilePath");

            //open method
            fileContents.Append(Indent);
            fileContents.AppendLine("{");

            //getter sig
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("get");

            //open method
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("{");

            //return the path
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("return Path.Combine(Application.dataPath, \"Settings.txt\");");

            //close method
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("}");

            //close method
            fileContents.Append(Indent);
            fileContents.AppendLine("}");
        }

        private static void CreateInstanceVariable(StringBuilder fileContents)
        {
            //insert the static instance variable
            fileContents.Append(Indent);
            fileContents.Append("private static ");
            fileContents.Append(TypeName);
            fileContents.Append(" ");
            fileContents.Append(InstanceName);
            fileContents.AppendLine(";");
        }

        private static void CreateStaticConstructor(StringBuilder fileContents)
        {
            //constructor sig
            fileContents.Append(Indent);
            fileContents.Append("static ");
            fileContents.Append(TypeName);
            fileContents.AppendLine("()");

            //open method
            fileContents.Append(Indent);
            fileContents.AppendLine("{");

            //default json object
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(LoadMethodName);
            fileContents.AppendLine("();");

            //save it
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(SaveMethodName);
            fileContents.AppendLine("();");

            //close method
            fileContents.Append(Indent);
            fileContents.AppendLine("}");
        }

        private static void CreateLoadMethod(StringBuilder fileContents)
        {
            //method sig
            fileContents.Append(Indent);
            fileContents.Append("private static void ");
            fileContents.Append(LoadMethodName);
            fileContents.AppendLine("()");

            //open method
            fileContents.Append(Indent);
            fileContents.AppendLine("{");

            //default json object
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(InstanceName);
            fileContents.Append(" = new ");
            fileContents.Append(TypeName);
            fileContents.AppendLine("();");
            fileContents.AppendLine();

            //find json data
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("if (File.Exists(FilePath))");

            //open if statement
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("{");

            //try statement
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("try");

            //open try block
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("{");

            //load json data
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("string json = File.ReadAllText(FilePath);");

            //load json data
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(InstanceName);
            fileContents.Append(" = JsonUtility.FromJson<");
            fileContents.Append(TypeName);
            fileContents.AppendLine(">(json);");

            //close try block
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("}");

            //close try and open catch
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("catch { }");

            //close if statement
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("}");

            //close method
            fileContents.Append(Indent);
            fileContents.AppendLine("}");
        }

        private static void CreateSaveMethod(StringBuilder fileContents)
        {
            //method sig
            fileContents.Append(Indent);
            fileContents.Append("private static void ");
            fileContents.Append(SaveMethodName);
            fileContents.AppendLine("()");

            //open method
            fileContents.Append(Indent);
            fileContents.AppendLine("{");
            
            //check if null
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append("if (");
            fileContents.Append(InstanceName);
            fileContents.AppendLine(" is null)");

            //open block
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("{");

            //create new
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(InstanceName);
            fileContents.Append(" = new ");
            fileContents.Append(TypeName);
            fileContents.AppendLine("();");

            //end block
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("}");
            fileContents.AppendLine();

            //get json data
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append("string json = JsonUtility.ToJson(");
            fileContents.Append(InstanceName);
            fileContents.Append(", ");
            fileContents.Append(PrettyPrint ? "true" : "false");
            fileContents.AppendLine(");");

            //write to file
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("File.WriteAllText(FilePath, json);");

            //close method
            fileContents.Append(Indent);
            fileContents.AppendLine("}");
        }

        private static string PrefixLines(string text, string prefix)
        {
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = prefix + lines[i];
            }

            return string.Join("\n", lines);
        }

        private static string CutRegion(string template, string regionName)
        {
            string beginRegionKey = $"#region {regionName}";
            string endRegionKey = "#endregion";
            int start = template.IndexOf(beginRegionKey) + beginRegionKey.Length;
            int end = start;
            for (int i = start; i < template.Length; i++)
            {
                if (template[i] == endRegionKey[0])
                {
                    string regionString = template.Substring(i);
                    if (regionString.StartsWith(endRegionKey))
                    {
                        end = i;
                        break;
                    }
                }
            }

            return template.Substring(start, end - start).TrimEnd('\n', '\r').TrimStart('\n', '\r');
        }
    }
}