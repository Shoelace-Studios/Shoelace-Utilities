#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ShoelaceStudios.Randomization.Editor
{
    public static class FoldoutHelper
    {
        public static bool GetFoldout(Dictionary<string, bool> foldouts, SerializedProperty property)
        {
            string key = property.propertyPath;
            return foldouts.ContainsKey(key) && foldouts[key];
        }

        public static bool DrawFoldout(
            Dictionary<string, bool> foldouts, SerializedProperty property,
            Rect position, GUIContent label, out float y, float padding)
        {
            string key = property.propertyPath;
            if (!foldouts.ContainsKey(key))
                foldouts[key] = true;

            Rect foldoutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            bool expanded = EditorGUI.Foldout(foldoutRect, foldouts[key], label, true);
            foldouts[key] = expanded;

            y = position.y + EditorGUIUtility.singleLineHeight + padding;
            return expanded;
        }
    }
}
#endif
