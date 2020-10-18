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

            CreateSaveMethod(fileContents);
            fileContents.AppendLine();

            //add all properties to the builder
            for (int i = 0; i < settings.properties.Length; i++)
            {
                ref Property property = ref settings.properties[i];

                string fieldString = fieldTemplate.Replace("{PropertyName}", property.name);
                fieldString = fieldString.Replace("{PropertyType}", property.type);
                fieldString = fieldString.Replace("{Instance}", InstanceName);
                fieldString = fieldString.Replace("{DefaultValue}", property.defaultValue);
                fileContents.Append(fieldString);

                fileContents.AppendLine();

                string propertyString = propertyTemplate.Replace("{PropertyName}", property.name);
                propertyString = propertyString.Replace("{PropertyType}", property.type);
                propertyString = propertyString.Replace("{Instance}", InstanceName);
                propertyString = propertyString.Replace("{DefaultValue}", property.defaultValue);
                fileContents.Append(propertyString);

                if (i != settings.properties.Length - 1)
                {
                    fileContents.AppendLine();
                }
            }

            fileContents.AppendLine();
            fileContents.AppendLine("}");
            File.WriteAllText(settings.pathToClass, fileContents.ToString());
            AssetDatabase.Refresh();
        }

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
            fileContents.Append("current = new ");
            fileContents.Append(TypeName);
            fileContents.AppendLine("();");
            fileContents.AppendLine();

            //find json data
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("if (File.Exists(FilePath))");

            //open method
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("{");

            //load json data
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.AppendLine("string json = File.ReadAllText(FilePath);");

            //load json data
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append("current = JsonUtility.FromJson<");
            fileContents.Append(TypeName);
            fileContents.AppendLine(">(json);");

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

        private static void CreateSaveMethod(StringBuilder fileContents)
        {
            //method sig
            fileContents.Append(Indent);
            fileContents.AppendLine("private static void Save()");

            //open method
            fileContents.Append(Indent);
            fileContents.AppendLine("{");

            //get json data
            fileContents.Append(Indent);
            fileContents.Append(Indent);
            fileContents.Append("string json = JsonUtility.ToJson(current, ");
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

            return template.Substring(start, end - start).TrimEnd('\n').TrimStart('\n');
        }
    }
}