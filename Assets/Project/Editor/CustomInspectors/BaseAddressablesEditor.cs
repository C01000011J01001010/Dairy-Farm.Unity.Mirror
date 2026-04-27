using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.Collections.Generic;
using System;

public abstract class BaseAddressablesEditor : Editor
{
    // 자식 클래스에서 자신만의 저장 키값(PlayerPrefs Key)을 명시하도록 강제함
    protected abstract string PrefsKey { get; }

    protected string[] availableLabels;
    protected int selectedLabelIndex = 0;

    protected virtual void OnEnable()
    {
        // 1. 에디터가 켜질 때 라벨 목록 동기화
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings != null)
        {
            availableLabels = settings.GetLabels().ToArray();

            // 2. 자식 클래스가 지정한 키값을 통해 마지막 선택 라벨 복구
            string lastLabel = EditorPrefs.GetString(PrefsKey, "default");
            selectedLabelIndex = Array.IndexOf(availableLabels, lastLabel);
            if (selectedLabelIndex < 0) selectedLabelIndex = 0;
        }
        else
        {
            availableLabels = new string[] { "설정 파일 없음" };
        }
    }

    // [공통 UI] 드롭다운을 그리고 선택된 라벨 문자열을 반환하는 함수
    protected string DrawLabelSelector()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("검색할 Addressables 라벨");

        if (availableLabels != null && availableLabels.Length > 0)
        {
            selectedLabelIndex = EditorGUILayout.Popup(selectedLabelIndex, availableLabels);
            EditorPrefs.SetString(PrefsKey, availableLabels[selectedLabelIndex]);
            EditorGUILayout.EndHorizontal();

            return availableLabels[selectedLabelIndex];
        }

        EditorGUILayout.EndHorizontal();
        return null;
    }

    // [공통 기능] 선택된 라벨을 가진 모든 에셋(Entry)을 찾아주는 함수
    protected List<AddressableAssetEntry> GetAddressableEntries(string targetLabel)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable Asset Settings를 찾을 수 없습니다.");
            return null;
        }

        var entries = new List<AddressableAssetEntry>();
        settings.GetAllAssets(entries, false, group => true, entry => entry.labels.Contains(targetLabel));
        return entries;
    }
}