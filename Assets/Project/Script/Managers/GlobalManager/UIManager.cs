using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;


public enum MyUi
{
    #region GlobalScene
    LoadingScreen = 0,
    InventoryScreen,
    TargetMarker,
    
    // HUD
    QuickSlot = 50,
    #endregion

    #region TitleScene
    TitleScreen = 100,
    TitleSelection,
    #endregion
}

/*
 * Action, Func
 * 매개변수 숫자가 달라지는 것을 오버로딩으로 만들어놓았음
 * 매개변수 이름을 만들거나 주석을 달 수 없음
 * 임시로만 써야함 -> 웬만하면 delegate로 사용하는 것을 습관화 하는게 좋다
 * 다른 데에도 유용하다 -> 협업시 함수에 대한 이름을 알 수 있음
 * 보통 delegate는 보통 1개의 파일에 몰아서 사용함
 */
// ui에 요청하는 양식
public delegate void DelegateLoading_Call(int processAmount);
//public delegate void DelegateLoading_CallPercent(); // 자동으로 최대는 100퍼센트
public delegate void DelegateLoading_Next(string loadingContext, int skipAmount);
public delegate void DelegateLoading_NextPercent(string loadingContext, float percent);
public delegate void DelegateLoading_End();



public class UIManager : BaseGlobalManager, IGlobalManager
{
    private const string canvasFileName = "CanvasPreset";

    public static event DelegateLoading_Call        OnLoadingCall;
    public static event DelegateLoading_Next        OnLoadingNext;
    public static event DelegateLoading_NextPercent OnLoadingNextPercent;
    public static event DelegateLoading_End         OnLoadingEnd;

    private Transform uiRoot;
    private Canvas Canvas_HUD; // 0, 화면에 자동적으로 띄워지는 영역
    private Canvas Canvas_PopUp; // 10, 선택적으로 띄워지고 조작할 수 있는 영역
    private Canvas Canvas_Loading; // 100
    

    //RectTransform       _mainCanvasRectTransform;
    //CanvasScaler        _mainCanvasScaler;
    //GraphicRaycaster    _mainCanvasRaycaster;

    // 전체 ui 관리
    Dictionary<MyUi, BaseUi> UI_Dictionary = new();

    // HUD를 제외한 활성화된 ui 관리
    Stack<BaseUi> UI_Stack = new();

    public void Exit()
    {
        foreach(BaseUi ui in UI_Dictionary.Values)
        {
            ui.Exit();
        }
    }

    public IEnumerator Initialize()
    {
        GameObject uiRootObj = new GameObject("UI");
#if UNITY_EDITOR
        // 테스터 용도
        Scene GlobalScene = SceneManager.GetSceneByName(Constants.SCENE_NAME_GlobalScene);
        SceneManager.MoveGameObjectToScene(uiRootObj, GlobalScene);
#endif
        uiRoot = uiRootObj.transform;

        yield return Initialize_LoadingCanvas();
        yield return Initialize_HUDCanvas();
        yield return Initialize_PopUpCanvas();
        yield return null;
    }

    public IEnumerator Initialize_LoadingCanvas()
    {
        Canvas_Loading = GetNewCanvas("Loading Canvas", 100);

        // 로딩화면
        yield return InsertUI_ToLoadingCanvas(MyUi.LoadingScreen);
    }
    public IEnumerator Initialize_HUDCanvas()
    {
        Canvas_HUD = GetNewCanvas("HUD Canvas", 0);

        // 타겟팅 표시 Ui
        //yield return InsertUI_ToGlobalCanvas(MyUi.TargetMarker);
        yield break;
    }
    public IEnumerator Initialize_PopUpCanvas()
    {
        Canvas_PopUp = GetNewCanvas("PopUp Canvas", 10);

        // 인벤토리 화면
        //yield return InsertUI_ToMenuCanvas(MyUi.InventoryScreen);
        yield break;

    }

    private Canvas GetNewCanvas(string canvasName, int sortingOrder)
    {
        // UI 는 파일매니저 초기화보다 먼저 초기화되니 별개로 프리팹을 로드해야함
        GameObject prefab = FileManager.GetPrefab(GetUiPath(canvasFileName));

        if (prefab is null)
        {
            Debug.LogError($"prefab({canvasFileName}) does not exist");
        }

        GameObject newObject = Instantiate(prefab, uiRoot);

        if (!newObject)
        {
            Debug.LogWarning($"Instantiate failed of ({prefab.name})");
            return null;
        }

        // 캔버스가 잘 만들어졌다면
        if (newObject.TryGetComponent(out Canvas asCanvas))
        {
            // 캔버스 이름을 초기화
            newObject.name = canvasName;

             //화면에 그리는 순서를 초기화
            asCanvas.sortingOrder = sortingOrder;
            return asCanvas;
        }

        Debug.LogError($"{newObject.name} has no Canvas Component");
        return null;
    }

    private IEnumerator InsertUI_ToGlobalCanvas(MyUi type) { yield return InsertUI(type, Canvas_HUD.transform); }
    private IEnumerator InsertUI_ToMenuCanvas(MyUi type) { yield return InsertUI(type, Canvas_PopUp.transform);}
    private IEnumerator InsertUI_ToLoadingCanvas(MyUi type) { yield return InsertUI(type, Canvas_Loading.transform);}

    private IEnumerator InsertUI(MyUi uiType, Transform canvasTransform)
    {
        // 이미 존재하는 ui인지 확인
        if (UI_Dictionary.TryGetValue(uiType, out BaseUi originUi))
        {
            originUi.transform.SetParent(canvasTransform);
            Debug.LogWarning("UIManager -> Ui already exist");
            yield break;
        }

        // 없으면 추가
        GameObject Instance = MakeFromPrefab(uiType, canvasTransform);
        if (Instance is null)
        {
            Debug.LogError($"UIManager -> Instantiation of {uiType.ToString()} is failed");
            yield break;
        }

        BaseUi newUi = Instance.GetComponent<BaseUi>();
        if (newUi is null)
        {
            Debug.LogError($"UIManager -> Instance {Instance.name} has no [Baseui]");
            yield break;
        }

        newUi.gameObject.SetActive(false);
        yield return newUi.Initialize();
        yield return null;
    }

    // 깊이우선 탐색하며 초기화
    public IEnumerator InitChildUiByDFS(Transform parent, Dictionary<MyUi, BaseUi> uiContainer)
    {
        foreach (Transform child in parent)
        {
            // BaseUserInterface를 구현한 클래스를 갖고 있으면
            if (child.TryGetComponent(out BaseUi asUserInterface))
            {
                // Ui 초기화 후 딕셔너리에 삽입
                yield return asUserInterface.Initialize();
                InitChildUiByDFS(uiContainer, asUserInterface.UiType, asUserInterface);
            }
            // 클래스 없이 객체만 있는 경우 이름으로 타입변환
            else if (Enum.TryParse(child.name, out MyUi UiType))
            {
                Debug.LogError($"{child.name} has no BaseUi");
            }

            // child.gameObject.SetActive(false);를 if문 바깥으로 빼면 Ui의 모든 자식객체가 비활성화되어버림
            yield return InitChildUiByDFS(child, uiContainer);
        }
    }

    private void InitChildUiByDFS(Dictionary<MyUi, BaseUi> uiContainer, MyUi type, BaseUi ui)
    {
        ui.gameObject.SetActive(false);

        // Ui가 이미 초기화된 경우
        if (!uiContainer.TryAdd(type, ui))
        {
            Debug.LogAssertion($"{ui} is alreay Initilaize");
        }
    }

    public BaseUi GetUi(MyUi type)
    {
        if(UI_Dictionary.ContainsKey(type))
        {
            return UI_Dictionary[type];
        }
        Debug.LogWarning($"UiManager has no UserInterface of ({type.ToString()}) ");
        return null;
    }

    // UI프리팹 가져와서 인스턴시 후 메인캔버스에 연결
    public GameObject MakeFromPrefab(MyUi type, Transform parentTransform)
    {
        var prefab = GetUIPrefab(type);
        if(prefab is null)
        {
            Debug.LogError($"UIManager -> prefab({type.ToString()}) is null");
            return null;
        }
        return Instantiate(prefab, parentTransform);
    }


    public static GameObject GetUIPrefab(MyUi type)
    {
        string path = GetUiPath(type);
        return FileManager.GetPrefab(path);
    }


    public static void ClaimLoading_Start(int processAmount)
    {
        OnLoadingCall?.Invoke(processAmount);
        
    }

    public static void ClaimLoading_Next(string loadingContext, int skipAmount = 1)
    {
        OnLoadingNext?.Invoke(loadingContext, skipAmount);
    }
    public static void ClaimLoading_Next(string loadingContext, float percent)
    {
        OnLoadingNextPercent?.Invoke(loadingContext, percent);
    }
    public static void ClaimLoading_End()
    {
        OnLoadingEnd?.Invoke();
    }

    private static string GetUiPath(MyUi type)
        => GetUiPath(type.ToString());

    private static string GetUiPath(string PrefabName)
    {
        return Constants.DIRECTORY_Prefabs_Uis + "/" + PrefabName;
    }
}
