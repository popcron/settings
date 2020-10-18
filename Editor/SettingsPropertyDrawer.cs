using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Popcron.Settings
{
    [CustomPropertyDrawer(typeof(Property))]
    public class SettingsPropertyDrawer : PropertyDrawer
    {
        private static List<Type> settingsTypes = null;
        private static string[] displayOptions = null;

        private static void FindAllTypes()
        {
            List<string> displayOptions = new List<string>();
            settingsTypes = new List<Type>();

            void Add(Type newType, string displayName = null)
            {
                displayOptions.Add(displayName ?? newType.Name);
                settingsTypes.Add(newType);
            }

            //add primitives
            Add(typeof(string), "string");
            Add(typeof(int), "int");
            Add(typeof(float), "float");
            Add(typeof(double), "double");
            Add(typeof(bool), "bool");
            Add(typeof(char), "char");

            SettingsPropertyDrawer.displayOptions = displayOptions.ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (settingsTypes == null || displayOptions == null)
            {
                FindAllTypes();
            }

            position.height = 18;
            EditorGUI.PropertyField(position, property, label);
            if (property.isExpanded)
            {
                SerializedProperty name = property.FindPropertyRelative("name");
                SerializedProperty type = property.FindPropertyRelative("type");
                SerializedProperty value = property.FindPropertyRelative("value");

                position.y += 20;
                EditorGUI.PropertyField(position, name, new GUIContent("Name"));

                position.y += 20;
                ShowTypeEnum(position, type);

                position.y += 20;
                EditorGUI.PropertyField(position, value, new GUIContent("Value"));
            }
        }

        private void ShowTypeEnum(Rect position, SerializedProperty type)
        {
            int selectedIndex = -1;
            for (int i = 0; i < settingsTypes.Count; i++)
            {
                if (settingsTypes[i].FullName == type.stringValue)
                {
                    selectedIndex = i;
                    break;
                }
            }

            int newIndex = EditorGUI.Popup(position, "Type", selectedIndex, displayOptions);
            if (newIndex != selectedIndex)
            {
                type.stringValue = settingsTypes[newIndex].FullName;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            if (property.isExpanded)
            {
                height = 80;
            }

            return height;
        }
    }
}