using Newtonsoft.Json;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Popcron.Settings
{
    [CustomPropertyDrawer(typeof(Property))]
    public class SettingsPropertyDrawer : PropertyDrawer
    {
        private static string[] displayOptions = null;
        private static FieldInfo[] supportedFields = null;
        private static SerializedObject[] supportedTypesObjects = null;

        private static void FindAllTypes()
        {
            //add types from the supported types class
            supportedFields = typeof(SupportedTypes).GetFields();
            displayOptions = new string[supportedFields.Length];
            supportedTypesObjects = new SerializedObject[supportedFields.Length];
            for (int i = 0; i < displayOptions.Length; i++)
            {
                SupportedTypes supportedTypes = ScriptableObject.CreateInstance<SupportedTypes>();
                supportedTypesObjects[i] = new SerializedObject(supportedTypes);
                displayOptions[i] = supportedFields[i].FieldType.Name;
            }
        }

        private FieldInfo GetField(SerializedProperty typeProperty)
        {
            int selectedIndex = 0;
            for (int i = 0; i < supportedFields.Length; i++)
            {
                if (supportedFields[i].FieldType.FullName == typeProperty.stringValue)
                {
                    selectedIndex = i;
                    break;
                }
            }

            return supportedFields[selectedIndex];
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (supportedFields == null || displayOptions == null)
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
                FieldInfo fieldInfo = ShowTypeEnum(position, type);

                position.y += 20;
                ShowDefaultValue(position, fieldInfo, value);

                SettingsInspector.propertyIndex++;
            }
        }

        private SerializedProperty GetProperty(FieldInfo fieldInfo)
        {
            //grab a serialized object based on this field index
            SerializedObject supportedTypesObject = supportedTypesObjects[SettingsInspector.propertyIndex];
            SerializedProperty next = supportedTypesObject.FindProperty("first").Copy();
            next.Next(false);
            int index = 0;
            while (true)
            {
                if (supportedFields[index] == fieldInfo)
                {
                    return next.Copy();
                }

                index++;
                if (index == supportedFields.Length)
                {
                    break;
                }
                else
                {
                    next.NextVisible(false);
                }
            }

            return null;
        }

        private float GetHeight(FieldInfo fieldInfo)
        {
            //grab a serialized object based on this field index
            SerializedObject supportedTypesObject = supportedTypesObjects[0];
            SerializedProperty next = supportedTypesObject.FindProperty("first").Copy();
            next.Next(false);
            int index = 0;
            while (true)
            {
                if (supportedFields[index] == fieldInfo)
                {
                    return EditorGUI.GetPropertyHeight(next);
                }

                index++;
                if (index == supportedFields.Length)
                {
                    break;
                }
                else
                {
                    next.NextVisible(false);
                }
            }

            return 0f;
        }

        private FieldInfo ShowTypeEnum(Rect position, SerializedProperty typeProperty)
        {
            int selectedIndex = 0;
            for (int i = 0; i < supportedFields.Length; i++)
            {
                if (supportedFields[i].FieldType.FullName == typeProperty.stringValue)
                {
                    selectedIndex = i;
                    break;
                }
            }

            int newIndex = EditorGUI.Popup(position, "Type", selectedIndex, displayOptions);
            if (newIndex != selectedIndex)
            {
                Type systemType = supportedFields[newIndex].FieldType;
                typeProperty.stringValue = systemType.FullName;
            }

            return supportedFields[newIndex];
        }

        private void ShowDefaultValue(Rect position, FieldInfo fieldInfo, SerializedProperty value)
        {
            SerializedProperty property = GetProperty(fieldInfo);
            SupportedTypes targetObject = (SupportedTypes)property.serializedObject.targetObject;
            FieldInfo originalField = targetObject.GetType().GetField(property.name);

            //load from json
            object objectValue = JsonConvert.DeserializeObject(value.stringValue, originalField.FieldType);
            originalField.SetValue(targetObject, objectValue);
            property.serializedObject.Update();

            //show property
            EditorGUI.PropertyField(position, property, new GUIContent("Default"));
            property.serializedObject.ApplyModifiedProperties();

            //save to json
            objectValue = originalField.GetValue(targetObject);
            value.stringValue = JsonConvert.SerializeObject(objectValue);
            value.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                float height = 20f * 3f;
                SerializedProperty typeProperty = property.FindPropertyRelative(nameof(Property.type));
                FieldInfo propertyType = GetField(typeProperty);
                return height + GetHeight(propertyType);
            }
            else
            {
                return 20f;
            }
        }

        public class SupportedTypes : ScriptableObject
        {
            [SerializeField]
            private byte first;

            public byte byteValue;
            public sbyte sbyteValue;
            public short shortValue;
            public ushort ushortValue;
            public int intValue;
            public uint uintValue;
            public long longValue;
            public bool boolValue;

            public string stringValue;
            public string[] stringListValue;

            public char charValue;
            public float floatValue;
            public double doubleValue;
        }
    }
}