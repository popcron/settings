using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Popcron.Settings
{
    [CustomEditor(typeof(Settings))]
    public class SettingsInspector : Editor
    {
        public static void Show(SerializedObject serializedObject)
        {
            SerializedProperty pathToClass = serializedObject.FindProperty("pathToClass");
            SerializedProperty properties = serializedObject.FindProperty("properties");

            EditorGUILayout.PropertyField(pathToClass, new GUIContent("Path to Class File"));
            EditorGUILayout.PropertyField(properties, true);

            if (GUILayout.Button("Generate"))
            {
                GenerateClass();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void GenerateClass()
        {
            List<string> lines = new List<string>();
            lines.Add("public class Settings");
            lines.Add("{");
            lines.Add("");
            lines.Add("}");

            File.WriteAllLines(Settings.Current.pathToClass, lines);
        }

        public override void OnInspectorGUI()
        {
            Show(serializedObject);
        }
    }
}
