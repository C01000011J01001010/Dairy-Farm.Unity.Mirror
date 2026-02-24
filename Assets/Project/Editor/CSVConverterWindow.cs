using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq; // 추가: 파일 검색을 위해

public class CSVConverterWindow : EditorWindow
{
    private string savePath = "Assets/Resources/ItemData";
    private const string SavePathPrefsKey = "CSV_TO_SO_SAVE_PATH";

    [MenuItem("Tools/CSV/CSV to SO Converter Window")]
    public static void ShowWindow() => GetWindow<CSVConverterWindow>("CSV Converter");

    private void OnEnable() => savePath = EditorPrefs.GetString(SavePathPrefsKey, "Assets/Resources/ItemData");

    private void OnGUI()
    {
        GUILayout.Label("CSV to ScriptableObject 변환기 (데이터 보존형)", EditorStyles.boldLabel);
        GUILayout.Space(10);

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
        GUI.enabled = isFileValid && isPathValid;
        if (GUILayout.Button("변환 실행 (기존 데이터 유지)", GUILayout.Height(40))) ExecuteConvert(selectedCsv);
        GUI.enabled = true;
    }

    private void ExecuteConvert(TextAsset csvFile)
    {
        EditorPrefs.SetString(SavePathPrefsKey, savePath);
        string[] rows = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (rows.Length <= 1)
        {
            EditorUtility.DisplayDialog("데이터 변환 실패", "데이터가 부족합니다. 헤더와 내용을 확인해주세요.", "확인");
            return;
        }

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
                string targetFileName = $"{id}_{safeName}.asset";
                string targetAssetPath = $"{savePath}/{targetFileName}";

                // --- [핵심] 기존 데이터 마이그레이션 로직 ---
                // 1. 해당 폴더에서 같은 ID로 시작하는 파일이 있는지 먼저 찾습니다.
                string[] existingFiles = Directory.GetFiles(savePath, $"{id}_*.asset");

                if (existingFiles.Length > 0)
                {
                    // 시스템 경로를 유니티 에셋 경로로 변환
                    string oldSystemPath = existingFiles[0].Replace('\\', '/');
                    string oldAssetPath = "Assets" + oldSystemPath.Replace(Application.dataPath, "");

                    // 2. 파일명이 바뀌었다면 기존 파일을 새 이름으로 변경(Move)합니다.
                    if (oldAssetPath != targetAssetPath)
                    {
                        AssetDatabase.MoveAsset(oldAssetPath, targetAssetPath);
                        Debug.Log($"이름 변경 감지: {Path.GetFileName(oldAssetPath)} -> {targetFileName} (기존 데이터 보존됨)");
                    }
                }

                // 3. 이제 파일을 불러옵니다 (이름을 바꿨다면 바뀐 이름으로, 같다면 원래 이름으로 로드됨)
                ItemData asset = AssetDatabase.LoadAssetAtPath<ItemData>(targetAssetPath);

                if (asset == null)
                {
                    asset = CreateInstance<ItemData>();
                    AssetDatabase.CreateAsset(asset, targetAssetPath);
                }

                // 4. CSV에 있는 데이터만 덮어씁니다. (인스펙터에서 넣은 리스트 등은 그대로 유지됨)
                asset.id = id;
                asset.itemName = name;
                asset.description = desc;

                EditorUtility.SetDirty(asset);
                successCount++;
            }
            catch (Exception e) { Debug.LogError($"{i + 1}번째 줄 실패: {e.Message}"); }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ 변환 완료! 총 {successCount}개의 데이터가 업데이트되었습니다.");
    }
}