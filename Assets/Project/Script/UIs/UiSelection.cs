using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CoreEngine.Ui
{
    [Serializable]
    public class UiSelection
    {
        [SerializeField] private List<AssetReferenceGameObject> _uiList = new();

        // (런타임) 캡슐화된 내부 리스트를 순회하여 안전한 UI 목록 반환
        public IEnumerable<AssetReferenceGameObject> GetValidUis()
        {
            if (_uiList == null || _uiList.Count == 0) yield break;

            HashSet<string> loadedGuids = new();

            foreach (var assetRef in _uiList)
            {
                if (assetRef == null || !assetRef.RuntimeKeyIsValid()) continue;

                if (!loadedGuids.Add(assetRef.AssetGUID))
                {
                    Debug.LogWarning($"[UiSelection] 중복된 UI 감지. 로드를 스킵합니다: {assetRef.RuntimeKey}");
                    continue;
                }

                yield return assetRef;
            }
        }

#if UNITY_EDITOR
        // (에디터) 컴포넌트 본체에서 호출하여 직렬화된 _uiList의 무결성 검증
        public void Validate<TInterface>(MonoBehaviour owner, string selectionPropertyPath)
        {
            if (_uiList == null || _uiList.Count == 0) return;

            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (owner == null) return;

                var so = new UnityEditor.SerializedObject(owner);
                so.Update();

                // UiSelection 변수 내부의 _uiList 프로퍼티를 상대 경로로 추적
                var selectionProp = so.FindProperty(selectionPropertyPath);
                if (selectionProp == null) return;

                var listProp = selectionProp.FindPropertyRelative("_uiList");
                if (listProp == null) return;

                bool isChanged = false;
                var guidSet = new HashSet<string>();

                for (int i = 0; i < _uiList.Count; i++)
                {
                    if (i >= listProp.arraySize) break;

                    var ui = _uiList[i];
                    if (ui == null || string.IsNullOrEmpty(ui.AssetGUID)) continue;

                    string currentGuid = ui.AssetGUID;
                    bool isDuplicate = !guidSet.Add(currentGuid);
                    bool isInvalidType = false;

                    string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(currentGuid);
                    GameObject prefabGo = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                    if (prefabGo != null && prefabGo.GetComponent<TInterface>() == null)
                    {
                        isInvalidType = true;
                    }

                    if (isDuplicate || isInvalidType)
                    {
                        if (isDuplicate)
                            Debug.LogWarning($"[UiSelection] 중복 차단: '{prefabGo?.name}' 슬롯을 비웁니다.");
                        if (isInvalidType)
                            Debug.LogWarning($"[UiSelection] 타입 오류: '{prefabGo?.name}'은(는) {typeof(TInterface).Name}가 아닙니다!");

                        ui.SetEditorAsset(null);
                        _uiList[i] = new AssetReferenceGameObject("");

                        var elementProp = listProp.GetArrayElementAtIndex(i);
                        var guidProp = elementProp.FindPropertyRelative("m_AssetGUID");
                        if (guidProp != null) guidProp.stringValue = "";

                        isChanged = true;
                    }
                }

                if (isChanged)
                {
                    so.ApplyModifiedProperties();
                    UnityEditor.EditorUtility.SetDirty(owner);
                }
            };
        }
#endif
    }
}