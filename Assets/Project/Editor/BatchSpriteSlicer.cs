using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;

public class BatchSpriteSlicer : EditorWindow
{
    private int sliceWidth = 16;
    private int sliceHeight = 16;

    // 메뉴 아이템을 누르면 창이 생성됨
    [MenuItem("Tools/2D/Custom Batch Slice")]
    public static void ShowWindow()
    {
        GetWindow<BatchSpriteSlicer>("Batch Slicer");
    }

    private void OnGUI()
    {
        GUILayout.Label("슬라이스 설정", EditorStyles.boldLabel);

        // 가로, 세로 크기 입력 필드
        sliceWidth = EditorGUILayout.IntField("가로 픽셀 (Width)", sliceWidth);
        sliceHeight = EditorGUILayout.IntField("세로 픽셀 (Height)", sliceHeight);

        GUILayout.Space(10);

        if (GUILayout.Button("선택한 파일들 슬라이스 시작"))
        {
            ExecuteSlice();
        }

        EditorGUILayout.HelpBox("프로젝트 창에서 이미지를 먼저 선택한 후 버튼을 누르세요.", MessageType.Info);
    }

    private void ExecuteSlice()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("선택된 파일이 없습니다!");
            return;
        }

        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(obj);

            if (dataProvider != null)
            {
                dataProvider.InitSpriteEditorDataProvider();

                // 텍스트 임포터 설정
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;

                var spriteRects = new List<SpriteRect>();
                Texture2D tex = obj as Texture2D;

                if (tex == null) continue;

                int index = 0;
                // 위에서 아래로 슬라이스 (유니티 좌표계 기준)
                for (int y = tex.height; y >= sliceHeight; y -= sliceHeight)
                {
                    for (int x = 0; x < tex.width; x += sliceWidth)
                    {
                        var rect = new SpriteRect
                        {
                            name = $"{obj.name}_{index++}",
                            rect = new Rect(x, y - sliceHeight, sliceWidth, sliceHeight),
                            alignment = SpriteAlignment.Center,
                            pivot = new Vector2(0.5f, 0.5f)
                        };
                        spriteRects.Add(rect);
                    }
                }

                dataProvider.SetSpriteRects(spriteRects.ToArray());
                dataProvider.Apply();
                importer.SaveAndReimport();
            }
        }
        Debug.Log($"일괄 슬라이스 완료: {sliceWidth}x{sliceHeight}");
    }
}