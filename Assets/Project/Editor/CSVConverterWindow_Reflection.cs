using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection; // 리플렉션 사용을 위해 추가
using System.Linq;

public class CSVConverterWindow_Reflection : EditorWindow
{
    private string savePath = "Assets/Resources/ItemData";
    private string targetClassName = "ItemData"; // 생성할 SO 클래스 이름

    private const string SavePathPrefsKey = "CSV_TO_SO_SAVE_PATH";
    private const string ClassNamePrefsKey = "CSV_TO_SO_CLASS_NAME";

    [MenuItem("Tools/CSV to SO Converter (Universal)")]
    public static void ShowWindow() => GetWindow<CSVConverterWindow_Reflection>("Universal CSV Converter");

    private void OnEnable()
    {
        savePath = EditorPrefs.GetString(SavePathPrefsKey, "Assets/Resources/ItemData");
        targetClassName = EditorPrefs.GetString(ClassNamePrefsKey, "ItemData");
    }

    private void OnGUI()
    {
        GUILayout.Label("만능 CSV to ScriptableObject 변환기", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // --- 1. 타겟 클래스 이름 입력 ---
        EditorGUILayout.BeginVertical("box");
        targetClassName = EditorGUILayout.TextField("생성할 클래스 이름", targetClassName).Trim();
        Type targetType = GetTypeByName(targetClassName);

        if (targetType == null)
        {
            EditorGUILayout.HelpBox($"'{targetClassName}' 클래스를 찾을 수 없습니다. 철자를 확인해주세요.\n(ScriptableObject를 상속받은 클래스여야 합니다)", MessageType.Error);
        }
        else if (!targetType.IsSubclassOf(typeof(ScriptableObject)))
        {
            EditorGUILayout.HelpBox($"'{targetClassName}'는 ScriptableObject가 아닙니다!", MessageType.Error);
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // --- 2. CSV 파일 선택 ---
        TextAsset selectedCsv = Selection.activeObject as TextAsset;
        bool isFileValid = false;
        string fileName = "선택된 파일 없음";

        if (selectedCsv != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedCsv);
            string extension = Path.GetExtension(assetPath).ToLower();
            if (extension == ".csv" || extension == ".txt")
            {
                fileName = selectedCsv.name + extension;
                isFileValid = true;
            }
        }

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("선택된 파일:", fileName, EditorStyles.wordWrappedLabel);
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // --- 3. 저장 경로 선택 ---
        bool isPathValid = Directory.Exists(savePath);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("저장 경로", savePath, EditorStyles.textField);
        if (GUILayout.Button("찾기", GUILayout.Width(50)))
        {
            string path = EditorUtility.OpenFolderPanel("저장 폴더 선택", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                savePath = "Assets" + path.Replace(Application.dataPath, "").Replace("\\", "/");
                EditorPrefs.SetString(SavePathPrefsKey, savePath);
                isPathValid = Directory.Exists(savePath);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // --- 4. 실행 버튼 ---
        bool canExecute = isFileValid && isPathValid && targetType != null && targetType.IsSubclassOf(typeof(ScriptableObject));
        GUI.enabled = canExecute;

        if (GUILayout.Button("변환 실행 (리플렉션 기반 자동 매칭)", GUILayout.Height(40)))
        {
            ExecuteConvert(selectedCsv, targetType);
        }
        GUI.enabled = true;
    }

    private void ExecuteConvert(TextAsset csvFile, Type targetType)
    {
        EditorPrefs.SetString(SavePathPrefsKey, savePath);
        EditorPrefs.SetString(ClassNamePrefsKey, targetClassName);

        string[] rows = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (rows.Length <= 1)
        {
            EditorUtility.DisplayDialog("데이터 변환 실패", "데이터가 부족합니다. (헤더와 내용 필요)", "확인");
            return;
        }

        // 1. 헤더(변수명) 추출 및 공백 제거
        string[] headers = rows[0].Split(',').Select(h => h.Trim()).ToArray();
        int successCount = 0;

        for (int i = 1; i < rows.Length; i++)
        {
            string[] cols = rows[i].Split(',');
            if (cols.Length == 0 || string.IsNullOrWhiteSpace(cols[0])) continue;

            try
            {
                // 파일명 자동 생성을 위한 변수 (id와 name이라는 헤더를 우선적으로 찾음)
                string fileId = i.ToString();
                string fileNamePart = "Data";

                // 2. 파일명 식별 (id, name 컬럼 찾기)
                for (int j = 0; j < headers.Length; j++)
                {
                    if (j >= cols.Length) break;
                    string headerLower = headers[j].ToLower();
                    if (headerLower == "id") fileId = cols[j].Trim();
                    else if (headerLower.Contains("name")) fileNamePart = cols[j].Trim();
                }

                string safeName = string.Concat(fileNamePart.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
                string targetFileName = $"{fileId}_{safeName}.asset";
                string targetAssetPath = $"{savePath}/{targetFileName}";

                // 기존 파일 보존 로직 (ID 기준 덮어쓰기)
                string[] existingFiles = Directory.GetFiles(savePath, $"{fileId}_*.asset");
                if (existingFiles.Length > 0)
                {
                    string oldSystemPath = existingFiles[0].Replace('\\', '/');
                    string oldAssetPath = "Assets" + oldSystemPath.Replace(Application.dataPath, "");
                    if (oldAssetPath != targetAssetPath)
                    {
                        AssetDatabase.MoveAsset(oldAssetPath, targetAssetPath);
                    }
                }

                // 3. ScriptableObject 인스턴스 로드 또는 생성
                ScriptableObject asset = AssetDatabase.LoadAssetAtPath(targetAssetPath, targetType) as ScriptableObject;
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance(targetType);
                    AssetDatabase.CreateAsset(asset, targetAssetPath);
                }

                // 4. 리플렉션을 통한 데이터 자동 매칭 및 할당 (핵심)
                for (int j = 0; j < headers.Length; j++)
                {
                    if (j >= cols.Length || string.IsNullOrWhiteSpace(cols[j])) continue;

                    string headerName = headers[j];
                    string stringValue = cols[j].Trim();

                    // 해당 이름의 변수가 targetType에 존재하는지 검색
                    FieldInfo field = targetType.GetField(headerName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (field != null)
                    {
                        object convertedValue = ConvertValue(stringValue, field.FieldType);
                        if (convertedValue != null)
                        {
                            field.SetValue(asset, convertedValue);
                        }
                    }
                }

                EditorUtility.SetDirty(asset);
                successCount++;
            }
            catch (Exception e) { Debug.LogError($"{i + 1}번째 줄 변환 실패: {e.Message}"); }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ [{targetClassName}] 총 {successCount}개의 데이터 매칭 완료!");
    }

    // 문자열을 지정된 타입으로 안전하게 변환하는 헬퍼 함수
    private object ConvertValue(string value, Type type)
    {
        try
        {
            if (type == typeof(int)) return int.Parse(value);
            if (type == typeof(float)) return float.Parse(value);
            if (type == typeof(string)) return value;
            if (type == typeof(bool)) return bool.Parse(value);
            if (type.IsEnum) return Enum.Parse(type, value, true);
        }
        catch
        {
            Debug.LogWarning($"값 변환 실패: '{value}'를 {type.Name} 타입으로 바꿀 수 없습니다.");
        }
        return null; // 지원하지 않는 타입이거나 실패하면 null 반환 (기존 값 유지)
    }

    // 현재 프로젝트의 모든 어셈블리에서 이름으로 클래스 타입을 찾는 함수
    private Type GetTypeByName(string className)
    {
        if (string.IsNullOrWhiteSpace(className)) return null;

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.Name == className) return type;
            }
        }
        return null;
    }
}