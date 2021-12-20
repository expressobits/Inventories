using UnityEditor;
using UnityEngine;

namespace ExpressoBits.Inventories.Editor
{
    [CustomEditor(typeof(Container))]
    public class ContainerEditor : UnityEditor.Editor
    {

        SerializedProperty databaseSerializedProperty;
        SerializedProperty slotsSerializedProperty;
        SerializedProperty limitedSlotsSerializedProperty;
        SerializedProperty limitedAmountOfSlotsSerializedProperty;

        public override void OnInspectorGUI()
        {
            databaseSerializedProperty = serializedObject.FindProperty("database");
            slotsSerializedProperty = serializedObject.FindProperty("slots");
            limitedSlotsSerializedProperty = serializedObject.FindProperty("limitedSlots");
            limitedAmountOfSlotsSerializedProperty = serializedObject.FindProperty("limitedAmountOfSlots");

            EditorGUILayout.PropertyField(databaseSerializedProperty);
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