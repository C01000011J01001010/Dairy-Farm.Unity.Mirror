using NUnit.Framework.Internal;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;


//public enum DataType
//{
//    ItemData,
//    QuestData,
//    SkillData
//}

public abstract class BaseCSVConverter : BaseEditorWindow<CSVConverter_Setting>
{
    protected abstract string ConverterTarget { get;}
    protected const string defaultMenu = "Tools/CSV/CSV to SO Converter -> ";

    protected string savePath { get => editorSetting.savePath; set => editorSetting.savePath = value; }
    TextAsset selectedCsv;
    bool isFileValid;
    bool isPathValid;


    //private DataType selectedType = DataType.ItemData;
    //private const string SelectedTypePrefsKey = "CSV_TO_SO_TYPE";



    private void OnEnable()
    {
        // 마지막으로 선택했던 타입 불러오기
        //selectedType = (DataType)EditorPrefs.GetInt(SelectedTypePrefsKey, 0);
        LoadSavedSettings(ConverterTarget);
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV -> ScriptableObject 변환기", EditorStyles.boldLabel);
        GUILayout.Space(10);

        //DrawDropDown();
        //GUILayout.Space(10);

        ValidateCsvFile();
        GUILayout.Space(10);

        CheckDirectory();
        GUILayout.Space(20);

        Execute();
    }

    private void DrawDropDown()
    {
        // ---데이터 타입 드롭다운 선택 ---
        //EditorGUILayout.BeginVertical(GUI.skin.box);
        //using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        //{
        //    EditorGUI.BeginChangeCheck();
        //    selectedType = (DataType)EditorGUILayout.EnumPopup("변환할 데이터 타입", selectedType);

        //    // 타입이 바뀌면 자동으로 저장 경로도 변경해줍니다!
        //    if (EditorGUI.EndChangeCheck())
        //    {
        //        EditorPrefs.SetInt(SelectedTypePrefsKey, (int)selectedType);
        //    }
        //}
        //EditorGUILayout.EndVertical();
    }

    private bool ValidateCsvFile()
    {
        // --- CSV 파일 선택(유효성 검사 포함) ---
        selectedCsv = Selection.activeObject as TextAsset;
        isFileValid = false;
        string fileName = "선택된 파일 없음";

        if (selectedCsv != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedCsv); // Assets 내에서 경로를 
            //Debug.Log($"파일 {selectedCsv.name}의 경로 : {assetPath}");
            string temp = Path.GetExtension(assetPath); // 확장자만 걸러내는 함수
            //Debug.Log($"Path.GetExtension({assetPath}) 결과 : {temp}");
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

        return isFileValid;
    }

    private bool CheckDirectory()
    {
        // --- 3. 저장 경로 (자동 설정되지만 수동으로도 변경 가능) ---
        isPathValid = Directory.Exists(savePath);
        //EditorGUILayout.BeginHorizontal();
        using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
        {
            // 직접 수정할 수 없도록 TextField 대신 LabelField
            EditorGUILayout.LabelField("저장 경로", savePath, EditorStyles.textField);
            if (GUILayout.Button("찾기", GUILayout.Width(50)))
            {
                string path = EditorUtility.OpenFolderPanel("저장 폴더 선택", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    savePath = path.ToUnityPath();
                    isPathValid = Directory.Exists(savePath);
                    UpdateSetting();
                }
            }
        }
        //EditorGUILayout.EndHorizontal();
        return isPathValid;
    }

    private void Execute()
    {
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

        ConvertData(rows, csvFile.name);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // =========================================================================
    // 아래부터는 각 데이터 타입별 전용 파싱 로직입니다.
    // =========================================================================

    private void ConvertData(string[] rows, string fileName)
    {
        int successCount = 0;
        for (int i = 1; i < rows.Length; i++)
        {
            string[] cols = rows[i].Split(',');
            if (cols.Length < 3) continue;

            try
            {
                ConvertData(cols, ref successCount);
            }
            catch (Exception e) { Debug.LogError($"[ItemData] {i + 1}번째 줄 실패: {e.Message}"); }
        }
        Debug.Log($"✅ [{fileName}] 총 {successCount}개의 아이템 데이터 갱신 완료!");
    }

    protected abstract void ConvertData(string[] cols, ref int successCount);

    protected string GetSafeFileName(string name) => string.Concat(name.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
    protected string GetTargetAssetPath(int id, string name) => $"{savePath}/{id}_{GetSafeFileName(name)}.asset";
}