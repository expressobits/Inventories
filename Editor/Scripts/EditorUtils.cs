using System;
using UnityEditor;
using UnityEngine;

namespace ExpressoBits.Inventories.Editor
{
    public class EditorUtils
    {
        class Styles
        {
            static readonly Color k_Normal_AllTheme = new Color32(0, 0, 0, 0);
            //static readonly Color k_Hover_Dark = new Color32(70, 70, 70, 255);
            //static readonly Color k_Hover = new Color32(193, 193, 193, 255);
            static readonly Color k_Active_Dark = new Color32(80, 80, 80, 255);
            static readonly Color k_Active = new Color32(216, 216, 216, 255);

            static readonly int s_MoreOptionsHash = "MoreOptions".GetHashCode();

            static public GUIContent MoreOptionsLabel { get; private set; }
            static public GUIStyle MoreOptionsStyle { get; private set; }
            static public GUIStyle MoreOptionsLabelStyle { get; private set; }

            static Styles()
            {
                MoreOptionsLabel = EditorGUIUtility.TrIconContent("MoreOptions", "More Options");

                MoreOptionsStyle = new GUIStyle(GUI.skin.toggle);
                Texture2D normalColor = new Texture2D(1, 1);
                normalColor.SetPixel(1, 1, k_Normal_AllTheme);
                MoreOptionsStyle.normal.background = normalColor;
                MoreOptionsStyle.onActive.background = normalColor;
                MoreOptionsStyle.onFocused.background = normalColor;
                MoreOptionsStyle.onNormal.background = normalColor;
                MoreOptionsStyle.onHover.background = normalColor;
                MoreOptionsStyle.active.background = normalColor;
                MoreOptionsStyle.focused.background = normalColor;
                MoreOptionsStyle.hover.background = normalColor;

                MoreOptionsLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(0, 0, 0, -1)
                };
            }

            //Note:
            // - GUIStyle seams to be broken: all states have same state than normal light theme
            // - Hover with event will not be updated right when we enter the rect
            //-> Removing hover for now. Keep theme color for refactoring with UIElement later
            static public bool DrawMoreOptions(Rect rect, bool active)
            {
                int id = GUIUtility.GetControlID(s_MoreOptionsHash, FocusType.Passive, rect);
                var evt = Event.current;
                switch (evt.type)
                {
                    case EventType.Repaint:
                        Color background = k_Normal_AllTheme;
                        if (active)
                            background = EditorGUIUtility.isProSkin ? k_Active_Dark : k_Active;
                        EditorGUI.DrawRect(rect, background);
                        GUI.Label(rect, MoreOptionsLabel, MoreOptionsLabelStyle);
                        break;
                    case EventType.KeyDown:
                        bool anyModifiers = evt.alt || evt.shift || evt.command || evt.control;
                        if ((evt.keyCode == KeyCode.Space || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter) && !anyModifiers && GUIUtility.keyboardControl == id)
                        {
                            evt.Use();
                            GUI.changed = true;
                            return !active;
                        }
                        break;
                    case EventType.MouseDown:
                        if (rect.Contains(evt.mousePosition))
                        {
                            GrabMouseControl(id);
                            evt.Use();
                        }
                        break;
                    case EventType.MouseUp:
                        if (HasMouseControl(id))
                        {
                            ReleaseMouseControl();
                            evt.Use();
                            if (rect.Contains(evt.mousePosition))
                            {
                                GUI.changed = true;
                                return !active;
                            }
                        }
                        break;
                    case EventType.MouseDrag:
                        if (HasMouseControl(id))
                            evt.Use();
                        break;
                }

                return active;
            }

            static int s_GrabbedID = -1;
            static void GrabMouseControl(int id) => s_GrabbedID = id;
            static void ReleaseMouseControl() => s_GrabbedID = -1;
            static bool HasMouseControl(int id) => s_GrabbedID == id;
        }

        public static class CoreStyles
        {
            static readonly Texture2D paneOptionsIconDark;
            static readonly Texture2D paneOptionsIconLight;
            public static Texture2D PaneOptionsIcon => EditorGUIUtility.isProSkin ? paneOptionsIconDark : paneOptionsIconLight;
            static CoreStyles()
            {
                //paneOptionsIconDark = (Texture2D) EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
                paneOptionsIconDark = (Texture2D)EditorGUIUtility.TrIconContent("_Menu").image;
                paneOptionsIconLight = (Texture2D)EditorGUIUtility.TrIconContent("d__Menu").image;
                //paneOptionsIconLight = (Texture2D) EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");
            }
        }

        /// <summary>Draw a header toggle like in Volumes</summary>
        /// <param name="title"> The title of the header </param>
        /// <param name="group"> The group of the header </param>
        /// <param name="contextAction">The context action</param>
        /// <param name="hasMoreOptions">Delegate saying if we have MoreOptions</param>
        /// <param name="toggleMoreOptions">Callback called when the MoreOptions is toggled</param>
        /// <returns>return the state of the foldout header</returns>
        public static bool DrawHeaderToggle(string title, SerializedProperty group, Texture2D icon, Action<Vector2> contextAction = null, Func<bool> hasMoreOptions = null, Action toggleMoreOptions = null)
            => DrawHeaderToggle(EditorGUIUtility.TrTextContent(title), group, contextAction, icon, hasMoreOptions, toggleMoreOptions, null);

        /// <summary>Draw a header toggle like in Volumes</summary>
        /// <param name="title"> The title of the header </param>
        /// <param name="group"> The group of the header </param>
        /// <param name="contextAction">The context action</param>
        /// <param name="hasMoreOptions">Delegate saying if we have MoreOptions</param>
        /// <param name="toggleMoreOptions">Callback called when the MoreOptions is toggled</param>
        /// <returns>return the state of the foldout header</returns>
        public static bool DrawHeaderToggle(GUIContent title, SerializedProperty group, Texture2D icon, Action<Vector2> contextAction = null, Func<bool> hasMoreOptions = null, Action toggleMoreOptions = null)
         => DrawHeaderToggle(title, group, contextAction, icon, hasMoreOptions, toggleMoreOptions, null);

        /// <summary>Draw a header toggle like in Volumes</summary>
        /// <param name="title"> The title of the header </param>
        /// <param name="group"> The group of the header </param>
        /// <param name="contextAction">The context action</param>
        /// <param name="hasMoreOptions">Delegate saying if we have MoreOptions</param>
        /// <param name="toggleMoreOptions">Callback called when the MoreOptions is toggled</param>
        /// <param name="documentationURL">Documentation URL</param>
        /// <returns>return the state of the foldout header</returns>
        public static bool DrawHeaderToggle(string title, SerializedProperty group, Action<Vector2> contextAction, Texture2D icon, Func<bool> hasMoreOptions, Action toggleMoreOptions, string documentationURL)
            => DrawHeaderToggle(EditorGUIUtility.TrTextContent(title), group, contextAction, icon, hasMoreOptions, toggleMoreOptions, documentationURL);

        /// <summary>Draw a header toggle like in Volumes</summary>
        /// <param name="title"> The title of the header </param>
        /// <param name="group"> The group of the header </param>
        /// <param name="contextAction">The context action</param>
        /// <param name="hasMoreOptions">Delegate saying if we have MoreOptions</param>
        /// <param name="toggleMoreOptions">Callback called when the MoreOptions is toggled</param>
        /// <param name="documentationURL">Documentation URL</param>
        /// <returns>return the state of the foldout header</returns>
        public static bool DrawHeaderToggle(GUIContent title, SerializedProperty group, Action<Vector2> contextAction, Texture2D icon, Func<bool> hasMoreOptions, Action toggleMoreOptions, string documentationURL)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 20f);

            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f + 16 + 5;

            var foldoutRect = backgroundRect;
            foldoutRect.x = 4f;
            foldoutRect.y += 3f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var iconRect = backgroundRect;
            iconRect.y += 1f;
            iconRect.width = 18f;
            iconRect.height = 18f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.y += 4f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            // More options 1/2
            var moreOptionsRect = new Rect();
            if (hasMoreOptions != null)
            {
                moreOptionsRect = backgroundRect;

                moreOptionsRect.x += moreOptionsRect.width - 16 - 1 - 16 - 5;

                if (!string.IsNullOrEmpty(documentationURL))
                    moreOptionsRect.x -= 16 + 7;

                moreOptionsRect.height = 15;
                moreOptionsRect.width = 16;
            }

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // Icon
            GUI.Label(iconRect, icon);

            // Foldout
            // group.serializedObject.Update();
            group.isExpanded = GUI.Toggle(foldoutRect, group.isExpanded, GUIContent.none, EditorStyles.foldout);
            // group.serializedObject.ApplyModifiedProperties();

            // More options 2/2
            if (hasMoreOptions != null)
            {
                bool moreOptions = hasMoreOptions();
                bool newMoreOptions = Styles.DrawMoreOptions(moreOptionsRect, moreOptions);
                if (moreOptions ^ newMoreOptions)
                    toggleMoreOptions?.Invoke();
            }

            // Context menu
            var menuIcon = CoreStyles.PaneOptionsIcon;
            var menuRect = new Rect(labelRect.xMax + 3f + 16 + 5, labelRect.y, 18f, 18f);

            if (contextAction != null)
                GUI.DrawTexture(menuRect, menuIcon);

            // Documentation button
            if (!string.IsNullOrEmpty(documentationURL))
            {
                var documentationRect = menuRect;
                documentationRect.x -= 16 + 5;
                documentationRect.y -= 1;

                var documentationTooltip = $"Open Reference for {title.text}.";
                var documentationIcon = new GUIContent(EditorGUIUtility.TrIconContent("_Help").image, documentationTooltip);
                var documentationStyle = new GUIStyle("IconButton");

                if (GUI.Button(documentationRect, documentationIcon, documentationStyle))
                    System.Diagnostics.Process.Start(documentationURL);
            }

            // Handle events
            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (contextAction != null && menuRect.Contains(e.mousePosition))
                {
                    contextAction(new Vector2(menuRect.x, menuRect.yMax));
                    e.Use();
                }
                else if (labelRect.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                        group.isExpanded = !group.isExpanded;
                    else contextAction?.Invoke(e.mousePosition);

                    e.Use();
                }
            }

            return group.isExpanded;
        }

         /// <summary>Draw a header toggle like in Volumes</summary>
        /// <param name="title"> The title of the header </param>
        /// <param name="group"> The group of the header </param>
        /// <param name="contextAction">The context action</param>
        /// <param name="hasMoreOptions">Delegate saying if we have MoreOptions</param>
        /// <param name="toggleMoreOptions">Callback called when the MoreOptions is toggled</param>
        /// <param name="documentationURL">Documentation URL</param>
        /// <returns>return the state of the foldout header</returns>
        public static void DrawHeaderNull(string title, Action<Vector2> contextAction, Func<bool> hasMoreOptions, Action toggleMoreOptions)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 20f);

            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f + 16 + 5;

            var foldoutRect = backgroundRect;
            foldoutRect.x = 4f;
            foldoutRect.y += 3f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var iconRect = backgroundRect;
            iconRect.y += 1f;
            iconRect.width = 18f;
            iconRect.height = 18f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.y += 4f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            // More options 1/2
            var moreOptionsRect = new Rect();
            if (hasMoreOptions != null)
            {
                moreOptionsRect = backgroundRect;

                moreOptionsRect.x += moreOptionsRect.width - 16 - 1 - 16 - 5;

                moreOptionsRect.height = 15;
                moreOptionsRect.width = 16;
            }

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // More options 2/2
            if (hasMoreOptions != null)
            {
                bool moreOptions = hasMoreOptions();
                bool newMoreOptions = Styles.DrawMoreOptions(moreOptionsRect, moreOptions);
                if (moreOptions ^ newMoreOptions)
                    toggleMoreOptions?.Invoke();
            }

            // Context menu
            var menuIcon = CoreStyles.PaneOptionsIcon;
            var menuRect = new Rect(labelRect.xMax + 3f + 16 + 5, labelRect.y, 18f, 18f);

            if (contextAction != null)
                GUI.DrawTexture(menuRect, menuIcon);


            // Handle events
            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (contextAction != null && menuRect.Contains(e.mousePosition))
                {
                    contextAction(new Vector2(menuRect.x, menuRect.yMax));
                    e.Use();
                }
            }
        }

        /// <summary>Draw a splitter separator</summary>
        /// <param name="isBoxed">[Optional] add margin if the splitter is boxed</param>
        public static void DrawSplitter(bool isBoxed = false)
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f);
            float xMin = rect.xMin;

            // Splitter rect should be full-width
            rect.xMin = 0f;
            rect.width += 4f;

            if (isBoxed)
            {
                rect.xMin = xMin == 7.0 ? 4.0f : EditorGUIUtility.singleLineHeight;
                rect.width -= 1;
            }

            if (Event.current.type != EventType.Repaint)
                return;

            EditorGUI.DrawRect(rect, !EditorGUIUtility.isProSkin
                ? new Color(0.6f, 0.6f, 0.6f, 1.333f)
                : new Color(0.12f, 0.12f, 0.12f, 1.333f));
        }
    }
}