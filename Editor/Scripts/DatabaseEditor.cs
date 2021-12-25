// using System;
// using UnityEditor;
// using UnityEngine;

// namespace ExpressoBits.Inventories.Editor
// {
//     [CustomEditor(typeof(Database))]
//     public class DatabaseEditor : UnityEditor.Editor
//     {

//         private Database database;

//         public override void OnInspectorGUI()
//         {
//             database = (Database)target;
//             // TODO show all items with search bar?
//             // base.OnInspectorGUI();
//             DoAddButton();
//         }

//         private void CreateScript(string scriptName)
//         {

//         }

//         private void DoAddButton()
//         {
//             GUIStyle buttonStyle = new GUIStyle("AC Button");
//             string nicifyName = ObjectNames.NicifyVariableName(typeof(Item).Name);
//             GUIContent buttonContent = new GUIContent("Add " + nicifyName);
//             Rect buttonRect = GUILayoutUtility.GetRect(buttonContent, buttonStyle, GUILayout.ExpandWidth(true));
//             buttonRect.width = buttonStyle.fixedWidth;
//             buttonRect.x = EditorGUIUtility.currentViewWidth * 0.5f - buttonRect.width * 0.5f;

//             if (GUI.Button(buttonRect, buttonContent, buttonStyle))
//             {
//                 AddItemWindow.ShowWindow(buttonRect, this, CreateScript);
//             }
//         }

//         internal void Add(Type type)
//         {
//             ScriptableObject newInstance = CreateInstance(type);
//             newInstance.name = "new item";
//             serializedObject.Update();
//             if (newInstance is Item newItem)
//             {
//                 database.Add(newItem, database.GetNewItemId());
//                 AssetDatabase.AddObjectToAsset(newItem, database);
//                 AssetDatabase.SaveAssets();
//             }

//         }
//     }
// }

