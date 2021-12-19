using UnityEditor;
using UnityEngine;

namespace ExpressoBits.Inventories.Editor
{
    [CustomEditor(typeof(Container))]
    public class ContainerEditor : UnityEditor.Editor
    {

        SerializedProperty slotsSerializedProperty;
        SerializedProperty limitedSlotsSerializedProperty;
        SerializedProperty limitedAmountOfSlotsSerializedProperty;

        public override void OnInspectorGUI()
        {
            slotsSerializedProperty = serializedObject.FindProperty("slots");
            limitedSlotsSerializedProperty = serializedObject.FindProperty("limitedSlots");
            limitedAmountOfSlotsSerializedProperty = serializedObject.FindProperty("limitedAmountOfSlots");

            EditorGUILayout.PropertyField(slotsSerializedProperty);
            EditorGUILayout.PropertyField(limitedSlotsSerializedProperty);
            if(limitedSlotsSerializedProperty.boolValue)
            {
                EditorGUILayout.PropertyField(limitedAmountOfSlotsSerializedProperty);
                slotsSerializedProperty.arraySize = Mathf.Min(limitedAmountOfSlotsSerializedProperty.intValue,slotsSerializedProperty.arraySize);
            }
            serializedObject.ApplyModifiedProperties();

        }
    }
}