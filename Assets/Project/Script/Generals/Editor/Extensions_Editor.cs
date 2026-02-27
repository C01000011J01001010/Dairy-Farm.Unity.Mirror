
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Extensions_Editor
{
    public static EditorSetting LoadSavedEditorSetting<EditorSetting>(this EditorWindow target, string fileName = null)
        where EditorSetting : ScriptableObject
    {
        // 지정된 파일 이름이 없으면 클래스 이름을 기본으로 사용
        string editorSettingPath;
        string editorSettingDirectory = target.GetCsDirectory();

        // 파일 이름과 확장자 지정
        if (string.IsNullOrEmpty(fileName))
        {
            editorSettingPath = $"{editorSettingDirectory}/{target.GetType().Name}_SetData.asset";
        }
        else
        {
            fileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            editorSettingPath = $"{editorSettingDirectory}/{fileName}.asset";
        }
        
        EditorSetting setting = editorSettingPath.LoadSavedData<EditorSetting>();
        return setting;
    }

    public static WantType LoadSavedData<WantType>(this string path)
        where WantType : ScriptableObject
    {
        // 경로에 저장된 데이터가 있으면 그대로 가져오고 없으면 만들어서 가져오기
        WantType savedData = AssetDatabase.LoadAssetAtPath<WantType>(path);
        if (savedData == null)
        {
            // 인스턴스로 만들고 해당 인스턴스를 에셋으로 저장
            savedData = ScriptableObject.CreateInstance<WantType>();
            AssetDatabase.CreateAsset(savedData, path);
            Debug.LogWarning($"경로({path})에 파일이 없음\n" +
                $"저장파일({typeof(WantType).Name}.asset)을 생성함\n");
        }
        return savedData;
    }


    public static string GetCsDirectory(this ScriptableObject target)
    {
        // Editor와 EditorWindow는 ScriptableObject를 상속받은 클래스
        MonoScript scriptAsset = MonoScript.FromScriptableObject(target);

        // 파일 위치만 가져오기
        string editorPath = AssetDatabase.GetAssetPath(scriptAsset);
        return Path.GetDirectoryName(editorPath).Replace("\\", "/");
    }

    public static string GetCsDirectory(this MonoBehaviour target)
    {
        MonoScript scriptAsset = MonoScript.FromMonoBehaviour(target);

        // 파일 위치만 가져오기
        string editorPath = AssetDatabase.GetAssetPath(scriptAsset);
        return Path.GetDirectoryName(editorPath).Replace("\\", "/");
    }

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
