using System.Collections.Generic;
using ShoelaceStudios.Utilities;
using UnityEditor;
using UnityEngine;

namespace ShoelaceStudios.Randomization.Editor
{
	#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(WeightedRandom<>), true)]
    public class WeightedRandomDrawer : PropertyDrawer
    {
        private const float BarHeight = 20f;
        private const float RowHeight = 22f;
        private const float Padding = 2f;
        private const float FieldWidth = 100f;
        private const float ButtonWidth = 60f;

        private readonly Dictionary<string, bool> foldouts = new();

        #region --- Main Drawer ---

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!TryGetEntries(property, out SerializedProperty entriesProp) || entriesProp.arraySize == 0)
                // No entries: warning + Add button
                return EditorGUIUtility.singleLineHeight + Padding + RowHeight + Padding;

            bool expanded = FoldoutHelper.GetFoldout(foldouts, property);
            float height = EditorGUIUtility.singleLineHeight + Padding; // foldout header
            height += BarHeight + Padding; // cumulative bar

            if (expanded)
            {
                height += entriesProp.arraySize * (RowHeight + Padding); // entry rows
                height += RowHeight + Padding; // Add button
                height += RowHeight + Padding; // Normalize button
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            property.serializedObject.Update();

            if (!TryGetEntries(property, out SerializedProperty entriesProp) || entriesProp.arraySize == 0)
            {
                // No entries: warning + Add button
                Rect warningRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(warningRect, label.text + " has no entries! Add at least one.");

                Rect addRect = new(position.x, warningRect.yMax + Padding, ButtonWidth, RowHeight);
                if (GUI.Button(addRect, "Add") && entriesProp != null)
                    EntryHelper.AddEntry(entriesProp);

                property.serializedObject.ApplyModifiedProperties();
                EditorGUI.EndProperty();
                return;
            }

            // Draw foldout header
            bool expanded = FoldoutHelper.DrawFoldout(foldouts, property, position, label, out float y, Padding);

            // Draw cumulative bar
            Rect barRect = new(position.x, y, position.width, BarHeight);
            if (expanded)
                BarHelper.DrawDraggableBar(entriesProp, barRect);
            else
                BarHelper.DrawCollapsedBarWithLabels(entriesProp, barRect);

            y = barRect.yMax + Padding;

            if (expanded)
            {
                // Draw all entries
                for (int i = 0; i < entriesProp.arraySize; i++)
                {
                    float bonusY = EntryHelper.DrawEntryRow(
                        entriesProp.GetArrayElementAtIndex(i), position.x, y, FieldWidth,
                        RowHeight, Padding, i, entriesProp);
                    y += RowHeight + Padding + bonusY;
                }

                // Draw Add + Normalize buttons
                Rect addButtonRect = new(position.x, y, ButtonWidth, RowHeight);
                if (GUI.Button(addButtonRect, "Add"))
                    EntryHelper.AddEntry(entriesProp);
                y += RowHeight + Padding;

                Rect normRect = new(position.x, y, FieldWidth * 3 + Padding * 2, RowHeight);
                if (GUI.Button(normRect, "Normalize Weights"))
                    EntryHelper.NormalizeEntries(entriesProp);
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        #endregion

        #region --- Foldout Helper ---

        #endregion

        private bool TryGetEntries(SerializedProperty property, out SerializedProperty entriesProp)
        {
            entriesProp = property.FindPropertyRelative("entries");
            return entriesProp != null;
        }
    }

	#endif
}
