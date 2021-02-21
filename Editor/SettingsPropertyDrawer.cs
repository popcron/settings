using Newtonsoft.Json;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Popcron.Settings
{
    [CustomPropertyDrawer(typeof(SettingsProperty))]
    public class SettingsPropertyDrawer : PropertyDrawer
    {
        private const float Gap = 2f;
        private const float IndentOffset = 18f;

        private static string[] displayOptions = null;
        private static FieldInfo[] surrogateFields = null;
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings();

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
            if (surrogateFields is null)
            {
                FindAllTypes();
            }

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

        private SerializedObject CreateSurrogateTypeObject(SerializedProperty property)
        {
            SerializedProperty typeProperty = property.FindPropertyRelative(nameof(SettingsProperty.type));
            SerializedProperty defaultValueProperty = property.FindPropertyRelative(nameof(SettingsProperty.defaultValue));
            SerializedProperty isExpandedProperty = property.FindPropertyRelative(nameof(SettingsProperty.isExpanded));

            //inefficient? i know
            //create the suty object
            SupportedTypes supportedTypes = ScriptableObject.CreateInstance<SupportedTypes>();
            SerializedObject serializedTypes = new SerializedObject(supportedTypes);
            FieldInfo supportedTypeField = GetSurrogateField(typeProperty);
            TypeHandler surrogateType = TypeHandler.Find(supportedTypeField.FieldType);

            //load from json into the correct field
            object objectValue = null;
            try
            {
                if (surrogateType != null)
                {
                    //if the expectated type is an array, then make it so
                    Type fakeType = surrogateType.FakeType;
                    if (supportedTypeField.FieldType.IsArray)
                    {
                        fakeType = fakeType.MakeArrayType();
                    }

                    objectValue = JsonConvert.DeserializeObject(defaultValueProperty.stringValue, fakeType);
                    objectValue = surrogateType.ConvertIntoReal(objectValue);
                }
                else
                {
                    objectValue = JsonConvert.DeserializeObject(defaultValueProperty.stringValue, supportedTypeField.FieldType);
                }
            }
            catch
            {
                //couldnt convert for sum reeson
                if (supportedTypeField.FieldType.IsValueType)
                {
                    objectValue = Activator.CreateInstance(supportedTypeField.FieldType);
                }
                else
                {
                    objectValue = null;
                }
            }

            supportedTypeField.SetValue(supportedTypes, objectValue);
            serializedTypes.Update();

            //set the expanded value on all kids
            SerializedProperty next = serializedTypes.FindProperty("first").Copy();
            next.isExpanded = isExpandedProperty.boolValue;
            while (next.Next(false))
            {
                next.isExpanded = isExpandedProperty.boolValue;
            }

            return serializedTypes;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (surrogateFields == null || displayOptions == null)
            {
                FindAllTypes();
            }

            SerializedProperty nameProperty = property.FindPropertyRelative(nameof(SettingsProperty.name));
            SerializedProperty descriptionProperty = property.FindPropertyRelative(nameof(SettingsProperty.description));
            SerializedProperty typeProperty = property.FindPropertyRelative(nameof(SettingsProperty.type));

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, new GUIContent($"{nameProperty.stringValue} ({typeProperty.stringValue})"), true);
            if (property.isExpanded)
            {
                position.x += IndentOffset;
                position.width -= IndentOffset;

                position.y += EditorGUIUtility.singleLineHeight + Gap;
                EditorGUI.PropertyField(position, nameProperty, new GUIContent("Name"));

                position.y += EditorGUIUtility.singleLineHeight + Gap;
                EditorGUI.PropertyField(position, descriptionProperty, new GUIContent("Description"));

                position.y += EditorGUIUtility.singleLineHeight + Gap;
                FieldInfo fieldInfo = ShowTypeEnum(position, typeProperty);

                position.y += EditorGUIUtility.singleLineHeight + Gap;
                SerializedObject surrogateTypesObject = CreateSurrogateTypeObject(property);
                ShowDefaultValue(position, fieldInfo, property, surrogateTypesObject);
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

        private void ShowDefaultValue(Rect position, FieldInfo surrogateField, SerializedProperty property, SerializedObject surrogateTypesObject)
        {
            SerializedProperty defaultValueProperty = property.FindPropertyRelative(nameof(SettingsProperty.defaultValue));
            SerializedProperty isExpandedProperty = property.FindPropertyRelative(nameof(SettingsProperty.isExpanded));
            SerializedProperty surrogateProperty = GetSurrogateProperty(surrogateField, surrogateTypesObject);

            //show property
            EditorGUI.PropertyField(position, surrogateProperty, new GUIContent("Default"));
            surrogateProperty.serializedObject.ApplyModifiedProperties();

            jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            //save to json by getting the value from suty
            //and then by writing back into the original property as a string
            object objectValue = surrogateField.GetValue(surrogateTypesObject.targetObject);

            //find a surrogate type handler for this, cause json.net is annoying
            if (objectValue != null)
            {
                TypeHandler surrogateType = TypeHandler.Find(surrogateField.FieldType);
                if (surrogateType != null)
                {
                    objectValue = surrogateType.ConvertAwayFromReal(objectValue);
                }
            }

            defaultValueProperty.stringValue = JsonConvert.SerializeObject(objectValue, jsonSettings);
            isExpandedProperty.boolValue = surrogateProperty.isExpanded;
            defaultValueProperty.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float elementHeight = EditorGUIUtility.singleLineHeight + Gap;
            float nameHeight = 0;
            float descriptionHeight = 0;
            float typeHeight = 0;
            float valueHeight = 0;
            if (property.isExpanded)
            {
                nameHeight = EditorGUIUtility.singleLineHeight + Gap;
                descriptionHeight = EditorGUIUtility.singleLineHeight + Gap;
                typeHeight = EditorGUIUtility.singleLineHeight + Gap;

                SerializedProperty typeProperty = property.FindPropertyRelative(nameof(SettingsProperty.type));
                SerializedObject surrogateTypesObject = CreateSurrogateTypeObject(property);
                FieldInfo surrogateFieldInfo = GetSurrogateField(typeProperty);
                SerializedProperty propertyType = GetSurrogateProperty(surrogateFieldInfo, surrogateTypesObject);
                valueHeight = EditorGUI.GetPropertyHeight(propertyType, true) + Gap;
            }

            return elementHeight + nameHeight + descriptionHeight + typeHeight + valueHeight;
        }
    }
}