using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ScenedUiManager : MonoBehaviour, IScenedManager
{
    public int Priority => 10;
    private Dictionary<Type, BaseUi> _scenedUiDict = new();

    public IEnumerator Initialize()
    {
        // 씬 하이라키에 미리 배치된 UI들 자동 등록 (DFS)
        yield return RegisterPreplacedUis(transform);
    }

    public IEnumerator PostInitialize() => null;

    private IEnumerator RegisterPreplacedUis(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.TryGetComponent(out BaseUi ui) && ui is IScenedUi)
            {
                yield return ui.Initialize();
                ui.ClaimClose();
                _scenedUiDict.TryAdd(ui.GetType(), ui);
            }
            yield return RegisterPreplacedUis(child);
        }
    }

    /// <summary>
    /// 씬 전용 UI 가져오기
    /// </summary>
    public T GetUi<T>() where T : BaseUi, IScenedUi
    {
        Type type = typeof(T);
        if (_scenedUiDict.TryGetValue(type, out BaseUi ui)) return ui as T;

        Debug.LogError($"[ScenedUiManager] {type.Name} not found in this scene!");
        return null;
    }

    public void Exit()
    {
        foreach (var ui in _scenedUiDict.Values) ui.Exit();
        _scenedUiDict.Clear();
    }
}