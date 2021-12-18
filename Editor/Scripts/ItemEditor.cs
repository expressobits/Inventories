using System;
using UnityEditor;
using UnityEngine;

namespace ExpressoBits.Inventories.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Item))]
    public class ItemEditor : UnityEditor.Editor
    {

        private bool changeID;
        private ushort newId;

        public override void OnInspectorGUI()
        {
            Item item = (Item)target;
            ChangeID(item);
            Show(item, serializedObject);
        }

        public static void Show(Item item, SerializedObject serializedObject)
        {
            item.name = EditorGUILayout.TextField("Name", item.name);

            //SerializedProperty idProperty = serializedObject.FindProperty("id");
            SerializedProperty weightProperty = serializedObject.FindProperty("weight");
            SerializedProperty maxStackProperty = serializedObject.FindProperty("maxStack");
            SerializedProperty iconProperty = serializedObject.FindProperty("icon");
            SerializedProperty descriptionProperty = serializedObject.FindProperty("description");
            //SerializedProperty itemObjectPrefabProperty = serializedObject.FindProperty("itemObjectPrefab");
            SerializedProperty categoryProperty = serializedObject.FindProperty("category");

            EditorGUILayout.PropertyField(weightProperty);
            EditorGUILayout.PropertyField(maxStackProperty);
            EditorGUILayout.PropertyField(iconProperty);
            EditorGUILayout.PropertyField(descriptionProperty);
            //EditorGUILayout.PropertyField(itemObjectPrefabProperty);
            EditorGUILayout.PropertyField(categoryProperty);

            serializedObject.ApplyModifiedProperties();

        }

        private bool HasItemId(ushort newId, Item thisItem)
        {
            Item[] items = Resources.FindObjectsOfTypeAll<Item>();
            foreach (var item in items)
            {
                if(item == thisItem) continue;
                if(item.ID == newId) return true;
            }
            return false;
        }

        private void ChangeID(Item item)
        {
            EditorGUILayout.BeginHorizontal();
            if (changeID)
            {
                bool validID = !HasItemId(newId,item);
                Color lastColor = GUI.color;
                if(!validID) GUI.color = Color.red;
                newId = (ushort)EditorGUILayout.IntField("ID", newId);
                GUI.color = lastColor;
                EditorGUI.BeginDisabledGroup(!validID);
                if (GUILayout.Button("OK"))
                {
                    changeID = false;
                    if (!HasItemId(newId,item))
                    {
                        serializedObject.FindProperty("id").intValue = newId;
                    }
                    // TODO items change id
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.LabelField("ID", item.ID.ToString());
                if (GUILayout.Button("Change"))
                {
                    changeID = true;
                    newId = (ushort)serializedObject.FindProperty("id").intValue;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

    }
}