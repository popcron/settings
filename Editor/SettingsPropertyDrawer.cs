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
        private static FieldInfo[] surrogateFields = null;

        private static void FindAllTypes()
        {
            //add types from the supported types class
            surrogateFields = typeof(SupportedTypes).GetFields();
            displayOptions = new string[surrogateFields.Length];
            for (int i = 0; i < displayOptions.Length; i++)
            {
                displayOptions[i] = surrogateFields[i].FieldType.Name;
            }
        }

        /// <summary>
        /// Returns the surrogate field based on this type property.
        /// </summary>
        private FieldInfo GetSurrogateField(SerializedProperty typeProperty)
        {
            int selectedIndex = 0;
            for (int i = 0; i < surrogateFields.Length; i++)
            {
                if (surrogateFields[i].FieldType.FullName == typeProperty.stringValue)
                {
                    selectedIndex = i;
                    break;
                }
            }

            return surrogateFields[selectedIndex];
        }

        private SerializedObject CreateSurrogateTypeObject(SerializedProperty typeProperty, SerializedProperty defaultValueProperty)
        {
            //inefficient? i know
            //create the suty object
            SupportedTypes supportedTypes = ScriptableObject.CreateInstance<SupportedTypes>();
            SerializedObject serializedTypes = new SerializedObject(supportedTypes);
            FieldInfo supportedTypeField = GetSurrogateField(typeProperty);

            //load from json into the correct field
            object objectValue = JsonConvert.DeserializeObject(defaultValueProperty.stringValue, supportedTypeField.FieldType);
            supportedTypeField.SetValue(supportedTypes, objectValue);
            serializedTypes.Update();
            return serializedTypes;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (surrogateFields == null || displayOptions == null)
            {
                FindAllTypes();
            }

            position.height = 18;
            EditorGUI.PropertyField(position, property, label);
            if (property.isExpanded)
            {
                SerializedProperty nameProperty = property.FindPropertyRelative(nameof(Property.name));
                SerializedProperty typeProperty = property.FindPropertyRelative(nameof(Property.type));
                SerializedProperty defaultValueProperty = property.FindPropertyRelative(nameof(Property.defaultValue));

                position.y += 20;
                EditorGUI.PropertyField(position, nameProperty, new GUIContent("Name"));

                position.y += 20;
                FieldInfo fieldInfo = ShowTypeEnum(position, typeProperty);

                position.y += 20;
                SerializedObject surrogateTypesObject = CreateSurrogateTypeObject(typeProperty, defaultValueProperty);
                ShowDefaultValue(position, fieldInfo, defaultValueProperty, surrogateTypesObject);
            }
        }

        /// <summary>
        /// Returns the serialized property that can represent this field info.
        /// </summary>
        private SerializedProperty GetSurrogateProperty(FieldInfo surrogateFieldInfo, SerializedObject surrogateTypesObject)
        {
            //grab a serialized object based on this field index
            SerializedProperty next = surrogateTypesObject.FindProperty("first").Copy();
            next.Next(false);
            int index = 0;
            while (true)
            {
                if (surrogateFields[index] == surrogateFieldInfo)
                {
                    return next.Copy();
                }

                index++;
                if (index == surrogateFields.Length)
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

        private float GetHeight(FieldInfo fieldInfo, SerializedObject serializedTypes)
        {
            //grab a serialized object based on this field index
            SerializedProperty next = serializedTypes.FindProperty("first").Copy();
            next.Next(false);
            int index = 0;
            while (true)
            {
                if (surrogateFields[index] == fieldInfo)
                {
                    return EditorGUI.GetPropertyHeight(next);
                }

                index++;
                if (index == surrogateFields.Length)
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

        /// <summary>
        /// Displays a type enum and returns the surrogate field that matches this type.
        /// </summary>
        private FieldInfo ShowTypeEnum(Rect position, SerializedProperty typeProperty)
        {
            int selectedIndex = 0;
            for (int i = 0; i < surrogateFields.Length; i++)
            {
                if (surrogateFields[i].FieldType.FullName == typeProperty.stringValue)
                {
                    selectedIndex = i;
                    break;
                }
            }

            int newIndex = EditorGUI.Popup(position, "Type", selectedIndex, displayOptions);
            if (newIndex != selectedIndex)
            {
                Type systemType = surrogateFields[newIndex].FieldType;
                typeProperty.stringValue = systemType.FullName;
            }

            return surrogateFields[newIndex];
        }

        private void ShowDefaultValue(Rect position, FieldInfo surrogateField, SerializedProperty defaultValueProperty, SerializedObject surrogateTypesObject)
        {
            SerializedProperty surrogateProperty = GetSurrogateProperty(surrogateField, surrogateTypesObject);

            //show property
            EditorGUI.PropertyField(position, surrogateProperty, new GUIContent("Default"));
            surrogateProperty.serializedObject.ApplyModifiedProperties();

            //save to json by getting the value from suty
            //and then by writing back into the original property as a string
            object objectValue = surrogateField.GetValue(surrogateTypesObject.targetObject);
            defaultValueProperty.stringValue = JsonConvert.SerializeObject(objectValue);
            defaultValueProperty.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                float height = 20f * 3f;
                SerializedProperty typeProperty = property.FindPropertyRelative(nameof(Property.type));
                SerializedProperty defaultValueProperty = property.FindPropertyRelative(nameof(Property.defaultValue));
                SerializedObject surrogateTypesObject = CreateSurrogateTypeObject(typeProperty, defaultValueProperty);
                FieldInfo surrogateFieldInfo = GetSurrogateField(typeProperty);
                SerializedProperty propertyType = GetSurrogateProperty(surrogateFieldInfo, surrogateTypesObject);
                return height + EditorGUI.GetPropertyHeight(propertyType, true);
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