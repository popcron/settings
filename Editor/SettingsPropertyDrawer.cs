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
            Add(typeof(byte), "byte");
            Add(typeof(sbyte), "sbyte");
            Add(typeof(short), "short");
            Add(typeof(ushort), "ushort");
            Add(typeof(int), "int");
            Add(typeof(uint), "uint");
            Add(typeof(long), "long");
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
                SerializedProperty name = property.FindPropertyRelative(nameof(Property.name));
                SerializedProperty type = property.FindPropertyRelative(nameof(Property.type));
                SerializedProperty value = property.FindPropertyRelative(nameof(Property.defaultValue));

                position.y += 20;
                EditorGUI.PropertyField(position, name, new GUIContent("Name"));

                position.y += 20;
                Type systemType = ShowTypeEnum(position, type);

                position.y += 20;
                ShowDefaultValue(position, systemType, value);
            }
        }

        private Type ShowTypeEnum(Rect position, SerializedProperty type)
        {
            int selectedIndex = 0;
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

            return settingsTypes[newIndex];
        }

        private void ShowDefaultValue(Rect position, Type systemType, SerializedProperty value)
        {
            GUIContent label = new GUIContent("Default");
            if (systemType == typeof(string))
            {
                value.stringValue = EditorGUI.TextField(position, label, value.stringValue);
            }
            else if (systemType == typeof(byte))
            {
                if (!byte.TryParse(value.stringValue, out byte byteValue))
                {
                    byteValue = 0;
                }

                value.stringValue = Mathf.Clamp(EditorGUI.LongField(position, label, byteValue), byte.MinValue, byte.MaxValue).ToString();
            }
            else if (systemType == typeof(sbyte))
            {
                if (!sbyte.TryParse(value.stringValue, out sbyte sbyteValue))
                {
                    sbyteValue = 0;
                }

                value.stringValue = Mathf.Clamp(EditorGUI.LongField(position, label, sbyteValue), sbyte.MinValue, sbyte.MaxValue).ToString();
            }
            else if (systemType == typeof(short))
            {
                if (!short.TryParse(value.stringValue, out short shortValue))
                {
                    shortValue = 0;
                }

                value.stringValue = Mathf.Clamp(EditorGUI.LongField(position, label, shortValue), short.MinValue, short.MaxValue).ToString();
            }
            else if (systemType == typeof(ushort))
            {
                if (!ushort.TryParse(value.stringValue, out ushort ushortValue))
                {
                    ushortValue = 0;
                }

                value.stringValue = Mathf.Clamp(EditorGUI.LongField(position, label, ushortValue), ushort.MinValue, ushort.MaxValue).ToString();
            }
            else if (systemType == typeof(int))
            {
                if (!int.TryParse(value.stringValue, out int intValue))
                {
                    intValue = 0;
                }

                value.stringValue = Mathf.Clamp(EditorGUI.LongField(position, label, intValue), int.MinValue, int.MaxValue).ToString();
            }
            else if (systemType == typeof(uint))
            {
                if (!uint.TryParse(value.stringValue, out uint uintValue))
                {
                    uintValue = 0;
                }

                value.stringValue = Mathf.Clamp(EditorGUI.LongField(position, label, uintValue), uint.MinValue, uint.MaxValue).ToString();
            }
            else if (systemType == typeof(long))
            {
                if (!long.TryParse(value.stringValue, out long longValue))
                {
                    longValue = 0;
                }

                value.stringValue = Mathf.Clamp(EditorGUI.LongField(position, label, longValue), long.MinValue, long.MaxValue).ToString();
            }
            else if (systemType == typeof(float))
            {
                if (!float.TryParse(value.stringValue, out float floatValue))
                {
                    floatValue = 0;
                }

                value.stringValue = EditorGUI.FloatField(position, label, floatValue).ToString() + "f";
            }
            else if (systemType == typeof(double))
            {
                if (!double.TryParse(value.stringValue, out double doubleValue))
                {
                    doubleValue = 0;
                }

                value.stringValue = EditorGUI.DoubleField(position, label, doubleValue).ToString() + "d";
            }
            else if (systemType == typeof(bool))
            {
                value.stringValue = EditorGUI.Toggle(position, label, value.stringValue.Equals("true", StringComparison.OrdinalIgnoreCase)).ToString().ToLower();
            }
            else
            {
                value.stringValue = EditorGUI.TextField(position, label, value.stringValue);
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