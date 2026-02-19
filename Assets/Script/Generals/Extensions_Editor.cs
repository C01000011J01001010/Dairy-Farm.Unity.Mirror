
using System.Collections;
using UnityEditor;
using UnityEngine;

public static class Extensions_Editor
{
    public static bool TryDrawScriptOpenButton(this Object target)
    {
        MonoBehaviour asMono = target as MonoBehaviour;
        if (asMono == null)
        {
            Debug.LogWarning($"{target.GetType().Name} is not MonoBehaviour");
            return false;
        }
        asMono.DrawScriptOpenButton();
        return true;
    }

    public static void DrawScriptOpenButton(this MonoBehaviour target)
    {
        MonoScript script = MonoScript.FromMonoBehaviour(target);
        Rect rect = EditorGUILayout.GetControlRect();
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13,
            //alignment = TextAnchor.MiddleRight,
        };

        // 배경 영역 클릭 감지
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            // 더블클릭 감지
            if (Event.current.clickCount == 2)
            {
                AssetDatabase.OpenAsset(script);
                Event.current.Use(); // 이벤트 소비
            }
        }

        EditorGUI.LabelField(rect, $"📜 {target.GetType().Name} (Double-click to open)", style);
    }
}
