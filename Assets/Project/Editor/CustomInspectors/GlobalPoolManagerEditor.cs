using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(GlobalPoolManager), true)]
public class GlobalPoolManagerEditor : BaseAddressablesEditor // 부모 상속!
{
    // 이 에디터만의 고유한 저장소 키값 설정
    protected override string PrefsKey => "GlobalPoolManager_LastLabel";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // base.OnInspectorGUI()와 동일하게 기본 UI 그림

        GlobalPoolManager manager = target as GlobalPoolManager;
        if (manager == null) return;

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("선택한 라벨이 붙은 모든 프리팹을 긁어와 리스트를 자동으로 채웁니다.", MessageType.Info);

        // 부모의 공통 UI 호출
        string selectedLabel = DrawLabelSelector();
        EditorGUILayout.Space(5);

        if (GUILayout.Button("전체 프리팹 가져오기", GUILayout.Height(40)))
        {
            if (!string.IsNullOrEmpty(selectedLabel))
            {
                AutoFillGlobalPool(manager, selectedLabel);
            }
        }
    }

    private void AutoFillGlobalPool(GlobalPoolManager manager, string targetLabel)
    {
        // 부모의 공통 탐색 기능 호출
        var entries = GetAddressableEntries(targetLabel);
        if (entries == null) return;

        if (entries.Count == 0)
        {
            Debug.LogWarning($"[{targetLabel}] 라벨을 가진 프리팹을 찾을 수 없습니다.");
            return;
        }

        int addCount = 0;
        int updateCount = 0;

        if (manager.poolSetups == null)
            manager.poolSetups = new List<BasePoolManager<GlobalPoolType>.PoolSetup>();

        foreach (var entry in entries)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.AssetPath);
            if (prefab == null) continue;

            if (Enum.TryParse(prefab.name, out GlobalPoolType matchedType))
            {
                var existingSetup = manager.poolSetups.Find(x => x.poolType.Equals(matchedType));

                if (existingSetup != null)
                {
                    if (existingSetup.prefab != prefab)
                    {
                        existingSetup.prefab = prefab;
                        updateCount++;
                    }
                }
                else
                {
                    manager.poolSetups.Add(new BasePoolManager<GlobalPoolType>.PoolSetup()
                    {
                        poolType = matchedType,
                        prefab = prefab,
                        defaultAmount = 10,
                        defaultCapacity = 20,
                        maxSize = 100
                    });
                    addCount++;
                }
            }
            else
            {
                Debug.LogWarning($"'{prefab.name}' 프리팹의 이름과 일치하는 GlobalPoolType Enum이 없습니다.");
            }
        }

        EditorUtility.SetDirty(manager);
        Debug.Log($"[GlobalPoolManager] 자동 할당 완료: {addCount}개 추가, {updateCount}개 갱신됨.");
    }
}