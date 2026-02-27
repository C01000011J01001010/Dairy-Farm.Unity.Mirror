using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

public class ItemCsvConverter : BaseCsvConverter
{
    const string target = "Item";
    protected override string ConverterTarget => target;

    [MenuItem(defaultMenu + target)]
    public static void ShowWindow() => GetWindow<ItemCsvConverter>("CSV Converter");

    protected override void ConvertData(string[] cols, ref int successCount)
    {
        int id = int.Parse(cols[0].Trim());
        string name = cols[1].Trim();
        string desc = cols[2].Trim();

        string targetAssetPath = GetTargetAssetPath(id, name);

        // 기존 파일 보존 로직
        // * 자리에 어떤 글자가 오든 문제 없음
        // ex) id = 1이면 "1_"로 시작하고 ".asset"으로 끝나는 모든 파일이 해당
        // Directory.GetFiles는 '디렉토리 + 파일 이름 + 확장자'가 모두 합쳐진 파일의 전체 경로를 배열로 반환
        string[] existingFiles = Directory.GetFiles(saveDirectory, $"{id}_*.asset");
        if (existingFiles != null && existingFiles.Length == 1)
        {
            string oldAssetPath = existingFiles[0].ToUnityPath();
            // 기존 파일이 존재하고 target이름이 파일로 없으면 기존 파일에서 이름만 바꾼다
            // 덮어쓰기 방지를 위해 같은 이름이 있으면 실행하지 않음
            // AssetDatabase.RenameAsset을 쓰려면 순수하게 이름만 있어야 해서 배제함
            if (oldAssetPath != targetAssetPath) AssetDatabase.MoveAsset(oldAssetPath, targetAssetPath);
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
        asset.ID = id;
        asset.Name = name;
        asset.Description = desc;

        // 현재 인스턴스와 실제 에셋의 데이터가 다르니 저장하라고 유니티에 요구하는 메서드
        // 이 메서드가 없으면 데이터를 바꿔도 에셋으로 저장 안됨
        EditorUtility.SetDirty(asset);
        successCount++;
    }
}
