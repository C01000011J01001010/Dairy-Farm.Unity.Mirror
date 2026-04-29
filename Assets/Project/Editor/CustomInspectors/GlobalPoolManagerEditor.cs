using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(GlobalPoolManager), true)]
public class GlobalPoolManagerEditor : BaseAddressablesEditor // 부모 상속!
{
    protected override string Label => Constants.LABEL_GlobalPoolObjects;

    protected override string Description => "선택한 라벨이 붙은 모든 프리팹을 긁어와 리스트를 자동으로 채웁니다.";

    protected override string ButtonTooltip => "전체 프리팹 가져오기";

    protected override void OnButtonClick(string targetLabel)
    {
        GlobalPoolManager manager = target as GlobalPoolManager;
        if (manager == null) return;
        AutoFillGlobalPool(manager, targetLabel);
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
                BasePoolManager<GlobalPoolType>.PoolSetup existingSetup 
                    = manager.poolSetups.Find(x => x.poolType.Equals(matchedType));

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
                    BasePoolManager<GlobalPoolType>.PoolSetup newSetup = new()
                    {
                        poolType = matchedType,
                        prefab = prefab,
                    };
                    manager.poolSetups.Add(newSetup);
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