using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(ScenedPoolManager), true)]
public class ScenedPoolManagerEditor : BaseAddressablesEditor // 부모 상속!
{
    // 이 에디터만의 고유한 저장소 키값 설정
    protected override string PrefsKey => "ScenedPoolManager_LastLabel";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ScenedPoolManager manager = target as ScenedPoolManager;
        if (manager == null) return;

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("리스트에 추가된 빈 항목의 이름과 일치하는 프리팹만 찾아 연결합니다.", MessageType.Info);

        // 부모의 공통 UI 호출
        string selectedLabel = DrawLabelSelector();
        EditorGUILayout.Space(5);

        if (GUILayout.Button("비어있는 프리팹 자동 할당", GUILayout.Height(40)))
        {
            if (!string.IsNullOrEmpty(selectedLabel))
            {
                FillEmptyPrefabs(manager, selectedLabel);
            }
        }
    }

    private void FillEmptyPrefabs(ScenedPoolManager manager, string targetLabel)
    {
        if (manager.poolSetups == null || manager.poolSetups.Count == 0)
        {
            Debug.LogWarning("[ScenedPoolManager] 먼저 리스트에 항목을 추가하고 Enum을 선택해주세요.");
            return;
        }

        // 부모의 공통 탐색 기능 호출
        var entries = GetAddressableEntries(targetLabel);
        if (entries == null) return;

        int updateCount = 0;

        foreach (var setup in manager.poolSetups)
        {
            if (setup.prefab != null) continue;

            string targetName = setup.poolType.ToString();
            var matchedEntry = entries.Find(e => Path.GetFileNameWithoutExtension(e.AssetPath) == targetName);

            if (matchedEntry != null)
            {
                GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(matchedEntry.AssetPath);
                if (loadedPrefab != null)
                {
                    setup.prefab = loadedPrefab;
                    updateCount++;
                }
            }
            else
            {
                Debug.LogWarning($"[{targetName}] 이름과 일치하는 프리팹을 Addressables(라벨: {targetLabel})에서 찾을 수 없습니다.");
            }
        }

        if (updateCount > 0)
        {
            EditorUtility.SetDirty(manager);
            Debug.Log($"[ScenedPoolManager] 프리팹 자동 할당 완료: {updateCount}개 채워짐.");
        }
        else
        {
            Debug.Log("[ScenedPoolManager] 새로 채워 넣을 비어있는 프리팹이 없습니다.");
        }
    }
}