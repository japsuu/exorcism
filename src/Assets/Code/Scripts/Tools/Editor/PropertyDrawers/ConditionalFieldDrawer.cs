using UnityEditor;
using UnityEngine;

namespace Tools.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalFieldAttribute conditionalAttribute = (ConditionalFieldAttribute)attribute;

            SerializedProperty conditionalProperty = property.serializedObject.FindProperty(conditionalAttribute.ConditionalFieldName);

            // Check if the conditional field is a bool.
            if (conditionalProperty != null && conditionalProperty.propertyType == SerializedPropertyType.Boolean)
            {
                bool showField = conditionalAttribute.VisibleWhenTrue ? conditionalProperty.boolValue : !conditionalProperty.boolValue;

                // Only show the field if the conditional field is true.
                if (showField)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalFieldAttribute conditionalAttribute = (ConditionalFieldAttribute)attribute;

            SerializedProperty conditionalProperty = property.serializedObject.FindProperty(conditionalAttribute.ConditionalFieldName);

            // Check if the conditional field is a bool.
            if (conditionalProperty != null && conditionalProperty.propertyType == SerializedPropertyType.Boolean)
            {
                bool showField = conditionalAttribute.VisibleWhenTrue ? conditionalProperty.boolValue : !conditionalProperty.boolValue;

                // Only show the field if the conditional field is true.
                // Return zero height to hide the field.
                return showField ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}