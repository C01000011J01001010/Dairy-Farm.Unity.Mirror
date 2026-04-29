using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.Collections.Generic;

public abstract class BaseAddressablesEditor : Editor
{
    protected abstract string Label { get; }
    protected abstract string Description { get; }
    protected abstract string ButtonTooltip { get; }

    //protected string[] availableLabels;
    //protected int selectedLabelIndex = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // base.OnInspectorGUI()와 동일하게 기본 UI 그림

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(Description, MessageType.Info);

        // 부모의 공통 UI 호출
        //string selectedLabel = DrawLabelSelector();
        EditorGUILayout.Space(5);

        if (GUILayout.Button(ButtonTooltip, GUILayout.Height(40)))
        {
            if (!string.IsNullOrEmpty(Label))
            {
                OnButtonClick(Label);
            }
        }
    }

    protected abstract void OnButtonClick(string targetLabel);

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