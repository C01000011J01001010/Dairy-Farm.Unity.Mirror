using UnityEngine;
using UnityEditor;
using System.IO;
using System;

// 1. 여기서 게임에 필요한 데이터 타입들을 정의합니다.
public enum DataType
{
    ItemData,
    QuestData,
    SkillData
}

public class CSVConverterWindow_Generic : EditorWindow
{
    private DataType selectedType = DataType.ItemData;
    private string savePath = "Assets/Resources/ItemData"; // 기본 경로

    private const string SelectedTypePrefsKey = "CSV_TO_SO_TYPE";

    [MenuItem("Tools/CSV to SO Converter (Dropdown)")]
    public static void ShowWindow() => GetWindow<CSVConverterWindow_Generic>("CSV Converter");

    private void OnEnable()
    {
        // 마지막으로 선택했던 타입 불러오기
        selectedType = (DataType)EditorPrefs.GetInt(SelectedTypePrefsKey, 0);
        UpdateSavePath();
    }

    private void OnGUI()
    {
        GUILayout.Label("드롭다운 선택형 CSV 변환기", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // --- 1. 데이터 타입 드롭다운 선택 ---
        EditorGUILayout.BeginVertical("box");
        EditorGUI.BeginChangeCheck();
        selectedType = (DataType)EditorGUILayout.EnumPopup("변환할 데이터 타입", selectedType);

        // 타입이 바뀌면 자동으로 저장 경로도 변경해줍니다!
        if (EditorGUI.EndChangeCheck())
        {
            UpdateSavePath();
            EditorPrefs.SetInt(SelectedTypePrefsKey, (int)selectedType);
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

        // --- 3. 저장 경로 (자동 설정되지만 수동으로도 변경 가능) ---
        bool isPathValid = Directory.Exists(savePath);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("저장 경로", savePath, EditorStyles.textField);
        if (GUILayout.Button("찾기", GUILayout.Width(50)))
        {
            string path = EditorUtility.OpenFolderPanel("저장 폴더 선택", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                savePath = "Assets" + path.Replace(Application.dataPath, "").Replace("\\", "/");
                isPathValid = Directory.Exists(savePath);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // --- 4. 실행 버튼 ---
        GUI.enabled = isFileValid && isPathValid;

        if (GUILayout.Button($"{selectedType} 변환 실행", GUILayout.Height(40)))
        {
            ExecuteConvert(selectedCsv);
        }
        GUI.enabled = true;
    }

    private void UpdateSavePath()
    {
        // 타입에 따라 자동으로 Resources 하위 폴더를 지정합니다.
        savePath = $"Assets/Resources/{selectedType}";
    }

    private void ExecuteConvert(TextAsset csvFile)
    {
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        string[] rows = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (rows.Length <= 1)
        {
            EditorUtility.DisplayDialog("데이터 변환 실패", "데이터가 부족합니다. (헤더와 내용 필요)", "확인");
            return;
        }

        // --- 5. 선택된 타입에 따라 다른 함수로 분기 (핵심) ---
        switch (selectedType)
        {
            case DataType.ItemData:
                ConvertItemData(rows, csvFile.name);
                break;
            case DataType.QuestData:
                ConvertQuestData(rows, csvFile.name);
                break;
            case DataType.SkillData:
                ConvertSkillData(rows, csvFile.name);
                break;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // =========================================================================
    // 아래부터는 각 데이터 타입별 전용 파싱 로직입니다.
    // =========================================================================

    private void ConvertItemData(string[] rows, string fileName)
    {
        int successCount = 0;
        for (int i = 1; i < rows.Length; i++)
        {
            string[] cols = rows[i].Split(',');
            if (cols.Length < 3) continue;

            try
            {
                int id = int.Parse(cols[0].Trim());
                string name = cols[1].Trim();
                string desc = cols[2].Trim();

                string safeName = string.Concat(name.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
                string targetAssetPath = $"{savePath}/{id}_{safeName}.asset";

                // 기존 파일 보존 로직
                string[] existingFiles = Directory.GetFiles(savePath, $"{id}_*.asset");
                if (existingFiles.Length > 0)
                {
                    string oldAssetPath = "Assets" + existingFiles[0].Replace('\\', '/').Replace(Application.dataPath, "");
                    if (oldAssetPath != targetAssetPath) AssetDatabase.MoveAsset(oldAssetPath, targetAssetPath);
                }

                ItemData asset = AssetDatabase.LoadAssetAtPath<ItemData>(targetAssetPath);
                if (asset == null)
                {
                    asset = CreateInstance<ItemData>();
                    AssetDatabase.CreateAsset(asset, targetAssetPath);
                }

                asset.id = id;
                asset.itemName = name;
                asset.description = desc;
                // 아이템 전용 추가 데이터가 있다면 여기서 파싱 (예: asset.maxStack = int.Parse(cols[3]);)

                EditorUtility.SetDirty(asset);
                successCount++;
            }
            catch (Exception e) { Debug.LogError($"[ItemData] {i + 1}번째 줄 실패: {e.Message}"); }
        }
        Debug.Log($"✅ [{fileName}] 총 {successCount}개의 아이템 데이터 갱신 완료!");
    }

    private void ConvertQuestData(string[] rows, string fileName)
    {
        // TODO: 나중에 QuestData 클래스를 만들면 여기에 파싱 로직을 작성합니다.
        // ItemData와 구조는 비슷하지만, "보상 리스트 분리", "목표 수치 파싱" 등 
        // 퀘스트만의 독특한 문자열 처리 로직을 여기에 마음껏 짤 수 있습니다.
        Debug.LogWarning("QuestData 변환 로직은 아직 구현되지 않았습니다!");
    }

    private void ConvertSkillData(string[] rows, string fileName)
    {
        // TODO: 나중에 SkillData 클래스를 만들면 여기에 파싱 로직을 작성합니다.
        Debug.LogWarning("SkillData 변환 로직은 아직 구현되지 않았습니다!");
    }
}