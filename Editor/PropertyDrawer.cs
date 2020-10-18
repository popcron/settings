using UnityEditor;
using UnityEngine;

namespace Popcron.Settings
{
    [CustomPropertyDrawer(typeof(Property))]
    public class PropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
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
                EditorGUI.PropertyField(position, type, new GUIContent("Type"));

                position.y += 20;
                EditorGUI.PropertyField(position, value, new GUIContent("Value"));
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