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
            SerializedProperty classTemplate = serializedObject.FindProperty("classTemplate");
            SerializedProperty properties = serializedObject.FindProperty("properties");

            try
            {
                EditorGUILayout.PropertyField(pathToClass, new GUIContent("Path to Class File"));
                EditorGUILayout.PropertyField(classTemplate);
                EditorGUILayout.PropertyField(properties, true);

                if (GUILayout.Button("Generate"))
                {
                    SettingsGenerator.GenerateClass();
                }

                serializedObject.ApplyModifiedProperties();
            }
            catch { }
        }

        public override void OnInspectorGUI()
        {
            Show(serializedObject);
        }
    }
}
