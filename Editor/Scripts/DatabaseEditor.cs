using UnityEditor;
using UnityEngine;

namespace ExpressoBits.Inventories.Editor
{
    [CustomEditor(typeof(Database))]
    public class DatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            NewItemButton();
        }

        public void MakeNewItem(Database database, ushort newId, string newName)
        {
            Item newItem = CreateInstance<Item>();
            newItem.name = newName;
            database.Add(newItem, newId);
            AssetDatabase.AddObjectToAsset(newItem, database);
            AssetDatabase.SaveAssets();
        }

        private ushort newId;
        private string newName = string.Empty;

        private void NewItemButton()
        {
            var origFontStyle = EditorStyles.label.fontStyle;
            var database = (Database)target;
            EditorGUILayout.BeginVertical("box");
            EditorStyles.label.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("New Item");
            EditorStyles.label.fontStyle = origFontStyle;
            ushort id = (byte)EditorGUILayout.IntField("Item ID", newId);
            if(id == 0) id = 1;
            id = database.HasItem(id) ? database.GetNewItemId() : id;
            newId = id;
            newName = EditorGUILayout.TextField("Name", newName);
            EditorGUI.BeginDisabledGroup(newName.Length == 0);
            if (GUILayout.Button("Add New Item"))
            {
                MakeNewItem(database, newId, newName);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }
}

