#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

[InitializeOnLoad]
public class AutoAddressableLabeler
{
    static AutoAddressableLabeler()
    {
        EditorApplication.delayCall += () =>
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                settings.OnModification -= OnSettingsModified;
                settings.OnModification += OnSettingsModified;
            }
        };
    }

    private static void OnSettingsModified(AddressableAssetSettings settings, AddressableAssetSettings.ModificationEvent e, object eventData)
    {
        if (e == AddressableAssetSettings.ModificationEvent.EntryAdded ||
            e == AddressableAssetSettings.ModificationEvent.EntryMoved)
        {
            if (eventData is AddressableAssetEntry singleEntry)
            {
                ProcessEntry(settings, singleEntry);
            }
            else if (eventData is IEnumerable<AddressableAssetEntry> entries)
            {
                foreach (var entry in entries)
                {
                    ProcessEntry(settings, entry);
                }
            }
        }
    }

    private static void ProcessEntry(AddressableAssetSettings settings, AddressableAssetEntry entry)
    {
        if (entry == null || entry.parentGroup == null) return;

        string groupName = entry.parentGroup.Name;

        // 시스템 기본 그룹들은 제외
        if (groupName == "Default Local Group" || groupName == "Built In Data") return;

        SetExclusiveLabel(settings, entry, groupName);
    }

    // 🌟 핵심 변경 부분: 이전 라벨을 청소하고 새 라벨만 독점(Exclusive)으로 세팅합니다.
    private static void SetExclusiveLabel(AddressableAssetSettings settings, AddressableAssetEntry entry, string labelName)
    {
        // 1. 전체 설정에 라벨이 없다면 생성
        if (!settings.GetLabels().Contains(labelName))
        {
            settings.AddLabel(labelName);
        }

        bool isChanged = false;

        // 2. 현재 에셋에 붙어있는 라벨 목록을 복사해옴 (HashSet 사용)
        var existingLabels = new HashSet<string>(entry.labels);

        // 3. 현재 그룹 이름과 '다른' 라벨이 붙어있다면 모두 제거 (False)
        foreach (var oldLabel in existingLabels)
        {
            if (oldLabel != labelName)
            {
                entry.SetLabel(oldLabel, false);
                isChanged = true;
            }
        }

        // 4. 새로운 라벨(현재 그룹 이름) 활성화 (True)
        if (!entry.labels.Contains(labelName))
        {
            entry.SetLabel(labelName, true);
            isChanged = true;
        }

        if (isChanged)
        {
            Debug.Log($"✨ [AutoLabeler] '{entry.address}' 에셋의 라벨이 '{labelName}'(으)로 깔끔하게 갱신되었습니다!");
        }
    }
}
#endif