#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace ShoelaceStudios.Randomization.Editor
{
    public static class BarHelper
    {
        public static void DrawCollapsedBarWithLabels(SerializedProperty entriesProp, Rect rect)
        {
            float total = TotalWeight(entriesProp);
            float x = rect.x;
            float minLabelWidth = 15f;

            for (int i = 0; i < entriesProp.arraySize; i++)
            {
                SerializedProperty entry = entriesProp.GetArrayElementAtIndex(i);
                SerializedProperty weightProp = entry.FindPropertyRelative("Weight");
                SerializedProperty colorProp = entry.FindPropertyRelative("Color");
                SerializedProperty valueProp = entry.FindPropertyRelative("Value");

                float width = weightProp.floatValue / total * rect.width;
                Color bgColor = colorProp?.colorValue ?? DefaultColor(i);
                EditorGUI.DrawRect(new Rect(x, rect.y, width, rect.height), bgColor);

                if (width > minLabelWidth)
                {
                    Color textColor = bgColor.grayscale > 0.35f ? Color.black : Color.white;

                    GUIStyle style = new(GUI.skin.label)
                    {
                        normal =
                        {
                            textColor = textColor
                        },
                        alignment = TextAnchor.MiddleLeft,
                        clipping = TextClipping.Clip,
                        fontSize = 12
                    };

                    float percent = weightProp.floatValue / total * 100f;
                    string labelText = $"{EntryHelper.GetPropertyValueString(valueProp)} ({percent:0.#}%)";

                    GUI.Label(new Rect(x + 2, rect.y, width, rect.height), labelText, style);
                }

                x += width;
            }
        }

        public static void DrawDraggableBar(SerializedProperty entriesProp, Rect rect)
        {
            float total = TotalWeight(entriesProp);
            float x = rect.x;

            for (int i = 0; i < entriesProp.arraySize; i++)
            {
                SerializedProperty entry = entriesProp.GetArrayElementAtIndex(i);
                SerializedProperty weightProp = entry.FindPropertyRelative("Weight");
                SerializedProperty colorProp = entry.FindPropertyRelative("Color");

                float width = weightProp.floatValue / total * rect.width;
                Rect segmentRect = new(x, rect.y, width, rect.height);
                EditorGUI.DrawRect(segmentRect, colorProp?.colorValue ?? DefaultColor(i));

                EditorGUIUtility.AddCursorRect(segmentRect, MouseCursor.ResizeHorizontal);
                if (Event.current.type == EventType.MouseDrag && segmentRect.Contains(Event.current.mousePosition))
                {
                    float delta = Event.current.delta.x / rect.width * total;
                    weightProp.floatValue = Mathf.Max(0f, weightProp.floatValue + delta * 2f);
                    Event.current.Use();
                }

                x += width;
            }
        }

        private static float TotalWeight(SerializedProperty entriesProp)
        {
            float total = 0f;
            for (int i = 0; i < entriesProp.arraySize; i++)
                total += Mathf.Max(0f, entriesProp.GetArrayElementAtIndex(i).FindPropertyRelative("Weight").floatValue);
            if (total <= 0f) total = entriesProp.arraySize;
            return total;
        }

        public static Color DefaultColor(int index)
        {
            Color[] palette =
            {
                Color.red, Color.green,
                Color.blue, Color.yellow,
                Color.cyan, Color.magenta,
                Color.gray
            };
            return palette[index % palette.Length];
        }
    }
}
#endif
