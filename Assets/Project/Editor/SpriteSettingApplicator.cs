using UnityEditor;
using UnityEngine;

public class SpriteSettingApplicator : EditorWindow
{
    private int ppu = 16;
    private FilterMode filterMode = FilterMode.Point;
    private TextureImporterCompression compression = TextureImporterCompression.Uncompressed;

    [MenuItem("Tools/2D/Sprite Setting Applicator")]
    public static void ShowWindow()
    {
        // 새로운 이름의 윈도우 생성
        GetWindow<SpriteSettingApplicator>("Sprite Setter");
    }

    private void OnGUI()
    {
        GUILayout.Label("스프라이트 인스펙터 일괄 설정", EditorStyles.boldLabel);
        GUILayout.Space(5);

        // 설정 값 입력 필드
        ppu = EditorGUILayout.IntField("Pixels Per Unit", ppu);
        filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", filterMode);
        compression = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compression", compression);

        GUILayout.Space(15);

        // 실행 버튼
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("선택한 스프라이트에 설정 적용", GUILayout.Height(40)))
        {
            ApplySpriteSettings();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.HelpBox("1. 프로젝트 창에서 이미지(Texture)들을 선택하세요.\n2. 버튼을 누르면 인스펙터 설정이 일괄 변경됩니다.", MessageType.Info);
    }

    private void ApplySpriteSettings()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("선택된 파일이 없습니다! 프로젝트 창에서 이미지를 선택해주세요.");
            return;
        }

        int count = 0;
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                // 인스펙터 설정값들 변경
                importer.spritePixelsPerUnit = ppu;
                importer.filterMode = filterMode;
                importer.textureCompression = compression;

                // 도트 그래픽 최적화: 밉맵 끄고 안티앨리어싱 방지
                importer.mipmapEnabled = false;

                // 변경사항 저장 및 리임포트
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
                count++;
            }
        }

        Debug.Log($"{count}개의 스프라이트 설정이 성공적으로 변경되었습니다!");
    }
}