using UnityEditor;
using UnityEngine;

namespace ExpressoBits.Inventories.Editor
{
    [CustomEditor(typeof(DropableItem))]
    public class DropableItemEditor : ItemEditor
    {
        public override void OnInspectorGUI()
        {
            base.ShowBaseItemInformation();
            SerializedProperty itemObjectProperty = serializedObject.FindProperty("itemObjectPrefab");
            EditorGUILayout.PropertyField(itemObjectProperty);
            base.ShowComponents();
            serializedObject.ApplyModifiedProperties();
        }
    }
}

