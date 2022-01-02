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

        SerializedProperty OnItemAddUnityEventSerializedProperty;
        SerializedProperty OnItemRemoveUnityEventSerializedProperty;
        SerializedProperty OnAddUnityEventSerializedProperty;
        SerializedProperty OnRemoveUnityEventSerializedProperty;
        SerializedProperty OnRemoveAtUnityEventSerializedProperty;
        SerializedProperty OnUpdateUnityEventSerializedProperty;
        SerializedProperty OnChangedUnityEventSerializedProperty;
        SerializedProperty OnOpenUnityEventSerializedProperty;
        SerializedProperty OnCloseUnityEventSerializedProperty;

        private void OnEnable()
        {
            OnItemAddUnityEventSerializedProperty = serializedObject.FindProperty("OnItemAddUnityEvent");
            OnItemRemoveUnityEventSerializedProperty = serializedObject.FindProperty("OnItemRemoveUnityEvent");
            OnAddUnityEventSerializedProperty = serializedObject.FindProperty("OnAddUnityEvent");
            OnRemoveUnityEventSerializedProperty = serializedObject.FindProperty("OnRemoveUnityEvent");
            OnRemoveAtUnityEventSerializedProperty = serializedObject.FindProperty("OnRemoveAtUnityEvent");
            OnUpdateUnityEventSerializedProperty = serializedObject.FindProperty("OnUpdateUnityEvent");
            OnChangedUnityEventSerializedProperty = serializedObject.FindProperty("OnChangedUnityEvent");
            OnOpenUnityEventSerializedProperty = serializedObject.FindProperty("OnOpenUnityEvent");
            OnCloseUnityEventSerializedProperty = serializedObject.FindProperty("OnCloseUnityEvent");
        }

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
            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(OnItemAddUnityEventSerializedProperty);
            EditorGUILayout.PropertyField(OnItemRemoveUnityEventSerializedProperty);
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(OnAddUnityEventSerializedProperty);
            EditorGUILayout.PropertyField(OnRemoveUnityEventSerializedProperty);
            EditorGUILayout.PropertyField(OnRemoveAtUnityEventSerializedProperty);
            EditorGUILayout.PropertyField(OnUpdateUnityEventSerializedProperty);
            EditorGUILayout.PropertyField(OnChangedUnityEventSerializedProperty);
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(OnOpenUnityEventSerializedProperty);
            EditorGUILayout.PropertyField(OnCloseUnityEventSerializedProperty);
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

        }
    }
}