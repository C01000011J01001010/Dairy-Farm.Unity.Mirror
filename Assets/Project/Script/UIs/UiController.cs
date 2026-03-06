using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiController : MonoBehaviour, IScenedInitialize
{
    [SerializeField] private int _priority;

    [Header("Ui"), Separator]
    [Tooltip("씬 시작시 초기 Ui")]

    [SerializeField] private MyUi[] InitialUis;

    [SerializeField] private Canvas[] ScenedCanvasArr;

    public int Priority => _priority;

    UIManager uiManager;
    Dictionary<MyUi, BaseUi> UI_Dictionary = new();

    public void Exit()
    {
        foreach (BaseUi ui in UI_Dictionary.Values)
        {
            ui.Exit();
        }
    }

    public IEnumerator Initialize()
    {
        uiManager = GameManager.GetManager<UIManager>();

        // 모든 ScenedCanvas는 Ui컨트롤 자식객체로 둠
        ScenedCanvasArr = GetComponentsInChildren<Canvas>();

        // 깊이우선 탐색으로 각 캔버스 아래의 모든 Ui를 초기화
        foreach (Canvas canvas in ScenedCanvasArr)
        {
            yield return uiManager.InitChildUiByDFS(canvas.transform, UI_Dictionary);
        }
        // 로딩 후 초기화면에 보여줄 Ui만 호출
        foreach (MyUi ui in InitialUis)
        {
            ClaimUiOpen(ui);
        }
        yield return null;
    }

    public IEnumerator PostInitialize()
    {
        yield break;
    }



    protected virtual bool TryGetScenedUi(MyUi type, out BaseUi ui)
    {
        if (UI_Dictionary.ContainsKey(type))
        {
            ui = UI_Dictionary[type];
            if (ui) return true;
            else return false;
        }
        ui = null;
        return false;
    }

    public virtual BaseUi GetUi(MyUi type)
    {
        BaseUi ui = null;
        // 씬에 종속된 Ui인지 먼저 확인 후
        if (!TryGetScenedUi(type, out ui))
        {
            Debug.LogWarning($"UiController has no UserInterface of ({type.ToString()}) ");

            // 없다면 UiManager에 문의
            ui = uiManager.GetUi(type);
        }
        return ui;
    }
    public BaseUi ClaimUiOpen(MyUi type)
    {
        BaseUi ui = GetUi(type);
        if (ui is null) return null;

        ui.ClaimOpen();
        return ui;
    }
    public void ClaimUiClose(MyUi type)
    {
        BaseUi ui = GetUi(type);
        if (ui is null) return;

        ui.ClaimClose();
    }

    
}
