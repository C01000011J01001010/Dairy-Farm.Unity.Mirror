using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// 1. 여기서 게임에 필요한 데이터 타입들을 정의합니다.
public enum DataType
{
    ItemData,
    QuestData,
    SkillData
}

public class CSVConverterWindow_Generic : EditorWindow
{
    const string Assets = "Assets";
    private DataType selectedType = DataType.ItemData;
    private string savePath = "Assets/Resources/ItemData"; // 기본 경로

    private const string SelectedTypePrefsKey = "CSV_TO_SO_TYPE";

    [MenuItem("Tools/CSV/CSV to SO Converter (Dropdown)")]
    public static void ShowWindow() => GetWindow<CSVConverterWindow_Generic>("CSV Converter");

    private void OnEnable()
    {
        // 마지막으로 선택했던 타입 불러오기
        selectedType = (DataType)EditorPrefs.GetInt(SelectedTypePrefsKey, 0);
        UpdateSavePath();
    }

    private void OnGUI()
    {
        // Editor와 EditorWindow는 ScriptableObject를 상속받은 클래스
        MonoScript scriptAsset = MonoScript.FromScriptableObject(this);
        // MonoBehaviour에 쓰고싶다면 MonoScript.FromMonoBehaviour(this); 사용
        Debug.Log(AssetDatabase.GetAssetPath(scriptAsset));

        GUILayout.Label("드롭다운 선택형 CSV 변환기", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // ---데이터 타입 드롭다운 선택 ---
        //EditorGUILayout.BeginVertical(GUI.skin.box);
        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            EditorGUI.BeginChangeCheck();
            selectedType = (DataType)EditorGUILayout.EnumPopup("변환할 데이터 타입", selectedType);

            // 타입이 바뀌면 자동으로 저장 경로도 변경해줍니다!
            if (EditorGUI.EndChangeCheck())
            {
                UpdateSavePath();
                EditorPrefs.SetInt(SelectedTypePrefsKey, (int)selectedType);
            }
        }
        //EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // --- CSV 파일 선택(유효성 검사 포함) ---
        TextAsset selectedCsv = Selection.activeObject as TextAsset;
        bool isFileValid = false;
        string fileName = "선택된 파일 없음";

        if (selectedCsv != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedCsv); // Assets 내에서 경로를 
            Debug.Log($"파일 {selectedCsv.name}의 경로 : {assetPath}");
            string temp = Path.GetExtension(assetPath); // 확장자만 걸러내는 함수
            Debug.Log($"Path.GetExtension({assetPath}) 결과 : {temp}");
            string extension = Path.GetExtension(assetPath).ToLower();

            // 확장자로 걸러내기 (csv 파일 확장자는 둘중 하나)
            if (extension == ".csv" || extension == ".txt")
            {
                // 파일 이름 초기화, 유효성 검사 확인
                fileName = selectedCsv.name + extension;
                isFileValid = true;
            }
        }

        //EditorGUILayout.BeginVertical("box");
        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            EditorGUILayout.LabelField("선택된 파일:", fileName, EditorStyles.wordWrappedLabel);
        }
        //EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // --- 3. 저장 경로 (자동 설정되지만 수동으로도 변경 가능) ---
        bool isPathValid = Directory.Exists(savePath);
        //EditorGUILayout.BeginHorizontal();
        using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
        {
            // 직접 수정할 수 없도록 TextField 대신 LabelField
            EditorGUILayout.LabelField("저장 경로", savePath, EditorStyles.textField);
            if (GUILayout.Button("찾기", GUILayout.Width(50)))
            {
                string path = EditorUtility.OpenFolderPanel("저장 폴더 선택", Assets, "");
                if (!string.IsNullOrEmpty(path))
                {
                    savePath = path.ToUnityPath();
                    isPathValid = Directory.Exists(savePath);
                }
            }
        }
        //EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // --- 실행 버튼 ---
        bool valid = isFileValid && isPathValid;
        //GUI.enabled = valid;
        using (new EditorGUI.DisabledGroupScope(!valid))
        {
            if (GUILayout.Button("변환 실행", GUILayout.Height(40)))
            {
                ExecuteConvert(selectedCsv);
            }
        } // <--- 중괄호가 끝나는 순간, 알아서 원래 상태(true)로 돌아감
        // 만약 이전 상태가 false라면 그대로 false
        //GUI.enabled = true;
    }

    private void UpdateSavePath()
    {
        // 타입에 따라 자동으로 Resources 하위 폴더를 지정합니다.
        savePath = $"Assets/Resources/{selectedType}";
    }

    private void ExecuteConvert(TextAsset csvFile)
    {
        // 디렉토리가 없으면 생성
        // 그런데 이전 내용대로면 디렉토리가 반드시 존재함
        // 안전하게 하려고 넣어둠
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        string[] rows = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // 스키마만 존재하는 경우
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
                // * 자리에 어떤 글자가 오든 문제 없음
                // ex) id = 1이면 "1_"로 시작하고 ".asset"으로 끝나는 모든 파일이 해당
                // Directory.GetFiles는 '디렉토리 + 파일 이름 + 확장자'가 모두 합쳐진 파일의 전체 경로를 배열로 반환
                string[] existingFiles = Directory.GetFiles(savePath, $"{id}_*.asset"); 
                if (existingFiles.Length == 1)
                {
                    string oldAssetPath = existingFiles[0].ToUnityPath();
                    // 기존 파일이 존재하고 target이름이 파일로 없으면 기존 파일에서 이름만 바꾼다
                    // 덮어쓰기 방지를 위해 같은 이름이 있으면 실행하지 않음
                    // AssetDatabase.RenameAsset을 쓰려면 순수하게 이름만 있어야 해서 배제함
                    if (oldAssetPath != targetAssetPath) AssetDatabase.MoveAsset(oldAssetPath, targetAssetPath);
                }
                else if (existingFiles.Length > 1)
                {
                    string errorMessage = $"파일변환 실패 -> Primary key로 지정된 id({id})가 유일하지 않음";
                    Debug.LogError(errorMessage);
                    continue;
                }

                // 저장장치에서 메모리로 불러오기
                ItemData asset = AssetDatabase.LoadAssetAtPath<ItemData>(targetAssetPath);

                if (asset == null)
                {
                    // 인스턴스로 만들고 해당 인스턴스를 에셋으로 저장
                    asset = CreateInstance<ItemData>();
                    AssetDatabase.CreateAsset(asset, targetAssetPath);
                }

                // 메모리에 있는 인스턴스의 데이터를 바꾸고
                asset.id = id;
                asset.itemName = name;
                asset.description = desc;
                
                // 현재 인스턴스와 실제 에셋의 데이터가 다르니 저장하라고 유니티에 요구하는 메서드
                // 이 메서드가 없으면 데이터를 바꿔도 에셋으로 저장 안됨
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