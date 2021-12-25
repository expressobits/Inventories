using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ExpressoBits.Inventories.Editor
{
    [CustomEditor(typeof(Item))]
    public class ItemEditor : UnityEditor.Editor
    {

        private bool changeID;
        private ushort newId;

        private SerializedProperty databaseProperty;

        #region Components Data
        private SerializedProperty componentsProperty;
        private Type elementType;
        private IList list;
        private List<ItemComponent> components;
        #endregion

        private void OnEnable()
        {
            componentsProperty = serializedObject.FindProperty("components");
            databaseProperty = serializedObject.FindProperty("database");
            list = (target as Item).components;
            components = (target as Item).components;
            elementType = Utility.GetElementType(list.GetType());
        }

        public override void OnInspectorGUI()
        {
            ShowInformation();
            serializedObject.ApplyModifiedProperties();
        }

        public void ShowInformation()
        {
            ShowDictionaryInformation();
            if (databaseProperty.objectReferenceValue != null)
            {
                ShowBaseItemInformation();
                ShowComponents();
            }
        }

        public void ShowDictionaryInformation()
        {
            Database lastDatabase = (Database)databaseProperty.objectReferenceValue;
            EditorGUI.indentLevel--;
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("ID Information");
            Item item = (Item)target;
            EditorGUILayout.PropertyField(databaseProperty);
            ChangeID(item);
            EditorGUILayout.EndVertical();
            Database actualDatabase = (Database)databaseProperty.objectReferenceValue;
            if(lastDatabase != actualDatabase)
            {
                if(lastDatabase != null) lastDatabase.Items.Remove(item);
                if(actualDatabase != null) actualDatabase.Items.Add(item);
            }
        }

        public void ShowBaseItemInformation()
        {
            Item item = (Item)target;
            
            Show(item, serializedObject);
            EditorGUILayout.Space(32);
        }

        public static void Show(Item item, SerializedObject serializedObject)
        {
            item.name = EditorGUILayout.TextField("Name", item.name);

            SerializedProperty maxStackProperty = serializedObject.FindProperty("maxStack");
            SerializedProperty iconProperty = serializedObject.FindProperty("icon");
            SerializedProperty descriptionProperty = serializedObject.FindProperty("description");
            SerializedProperty weightProperty = serializedObject.FindProperty("weight");
            SerializedProperty categoryProperty = serializedObject.FindProperty("category");

            EditorGUILayout.PropertyField(maxStackProperty);
            EditorGUILayout.PropertyField(weightProperty);
            EditorGUILayout.PropertyField(iconProperty);
            EditorGUILayout.PropertyField(descriptionProperty);
            EditorGUILayout.PropertyField(categoryProperty);

        }

        private bool HasItemId(ushort newId, Item thisItem)
        {
            List<Item> items = thisItem.Database.Items;
            foreach (var item in items)
            {
                if (item == thisItem) continue;
                if (item.ID == newId) return true;
            }
            return false;
        }

        private void ChangeID(Item item)
        {
            EditorGUILayout.BeginHorizontal();
            if (changeID)
            {
                if (newId == 0) newId = 1;
                bool validID = !HasItemId(newId, item);
                Color lastColor = GUI.color;
                if (!validID) GUI.color = Color.red;
                newId = (ushort)EditorGUILayout.IntField("ID", newId);
                GUI.color = lastColor;
                EditorGUI.BeginDisabledGroup(!validID);
                if (GUILayout.Button("OK"))
                {
                    changeID = false;
                    if (!HasItemId(newId, item))
                    {
                        serializedObject.FindProperty("id").intValue = newId;
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.LabelField("ID", databaseProperty.objectReferenceValue != null ? item.ID.ToString() : "-");
                if (GUILayout.Button("Change") && databaseProperty.objectReferenceValue != null)
                {
                    changeID = true;
                    newId = (ushort)serializedObject.FindProperty("id").intValue;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public void DeleteFromDatabase(Item item)
        {
            if (EditorUtility.DisplayDialog("Delete select item?", "Delete item name " + item.name + "\nYou cannot undo this action", "Yes", "No"))
            {
                Database database = item.Database;
                if (database) database.Items.Remove(item);
                AssetDatabase.RemoveObjectFromAsset(item);
                //string path = AssetDatabase.GetAssetPath((ScriptableObject)item);
                //AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
                Selection.activeGameObject = null;
            }
        }

        #region Components
        public void ShowComponents()
        {
            EditorGUILayout.Space(16);
            EditorGUILayout.HelpBox("Item Components is in experimental state as it is still necessary to add a \"MoveFrom\" attribute to the script if it is renamed. Renaming a script without this attribute will make it impossible to view the entire list of components that contain at least one of that script.", MessageType.Info);
            if (GUILayout.Button("See more here"))
            {
                Application.OpenURL("https://forum.unity.com/threads/serializereference-data-loss-when-class-name-is-changed.736874/");
            }
            SerializedProperty list = componentsProperty;
            if (components.Count > 0 && components[0] == null)
            {
                EditorGUILayout.HelpBox("There are invalid scripts on this list!", MessageType.Warning);
            }
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty property = list.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical();
                ItemComponent component = components[i];
                DrawComponent(i, component, ref property);
                EditorGUILayout.EndVertical();

            }
            EditorUtils.DrawSplitter(false);
            DoAddButton();
        }

        private void DrawComponent(int index, ItemComponent component, ref SerializedProperty componentProperty)
        {
            EditorUtils.DrawSplitter(false);
            if (component == null)
            {
                EditorUtils.DrawHeaderNull("Invalid script " + componentProperty.managedReferenceFullTypename, pos => OnContextClick(pos, index), null, null);
            }
            else
            {
                string name = ObjectNames.NicifyVariableName(component.GetType().Name);
                name = name.Replace(ObjectNames.NicifyVariableName(typeof(ItemComponent).Name), "");
                Texture2D icon = Utility.GetIcon(component.GetType());
                if (icon == null) icon = (Texture2D)EditorGUIUtility.ObjectContent(null, component.GetType()).image;
                if (icon == null) icon = AssetPreview.GetMiniTypeThumbnail(component.GetType());
                if (icon == null) icon = EditorGUIUtility.FindTexture("cs Script Icon");

                bool displayContent = EditorUtils.DrawHeaderToggle(name, componentProperty, icon, pos => OnContextClick(pos, index));
                if (displayContent)
                {
                    foreach (var child in componentProperty.EnumerateChildProperties())
                    {
                        EditorGUILayout.PropertyField(
                            child,
                            includeChildren: true
                        );
                    }
                }
            }

        }

        private void OnContextClick(Vector2 position, int id)
        {
            var menu = new GenericMenu();

            if (id == 0)
                menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Up"));
            else
                menu.AddItem(EditorGUIUtility.TrTextContent("Move Up"), false, () => MoveComponent(id, -1));

            if (id == componentsProperty.arraySize - 1)
                menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Down"));
            else
                menu.AddItem(EditorGUIUtility.TrTextContent("Move Down"), false, () => MoveComponent(id, 1));

            menu.AddSeparator(string.Empty);
            menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, () => RemoveComponent(id));

            menu.DropDown(new Rect(position, Vector2.zero));
        }

        private void MoveComponent(int id, int offset)
        {
            Undo.SetCurrentGroupName("Move Item Component");
            serializedObject.Update();
            componentsProperty.MoveArrayElement(id, id + offset);
            serializedObject.ApplyModifiedProperties();

            // Force save / refresh
            ForceSave();
        }

        private void ForceSave()
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        private void RemoveComponent(int id)
        {
            SerializedProperty property = componentsProperty.GetArrayElementAtIndex(id);
            property.managedReferenceValue = null;

            Undo.SetCurrentGroupName("Remove Item Component");

            componentsProperty.DeleteArrayElementAtIndex(id);
            serializedObject.ApplyModifiedProperties();

            // Force save / refresh
            ForceSave();
        }

        private void CreateScript(string scriptName)
        {

        }

        private void DoAddButton()
        {
            GUIStyle buttonStyle = new GUIStyle("AC Button");
            string nicifyName = ObjectNames.NicifyVariableName(elementType.Name);
            GUIContent buttonContent = new GUIContent("Add " + nicifyName);
            Rect buttonRect = GUILayoutUtility.GetRect(buttonContent, buttonStyle, GUILayout.ExpandWidth(true));
            buttonRect.width = buttonStyle.fixedWidth;
            buttonRect.x = EditorGUIUtility.currentViewWidth * 0.5f - buttonRect.width * 0.5f;

            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                AddObjectWindow.ShowWindow(buttonRect, elementType, Add, CreateScript);
            }
        }

        private void Add(Type type)
        {
            object value = Activator.CreateInstance(type);
            serializedObject.Update();
            componentsProperty.arraySize++;
            componentsProperty.GetArrayElementAtIndex(componentsProperty.arraySize - 1).managedReferenceValue = value;
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

    }
}