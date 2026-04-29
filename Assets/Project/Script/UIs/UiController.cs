using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UiController : MonoBehaviour, IScenedInitialize
{
    [SerializeField] private int _priority = 100; // 매니저들보다 늦게 초기화되도록 우선순위 설정
    public int Priority => _priority;

    [Header("이 씬의 초기화 UI 목록")]
    [Tooltip("씬 시작 시 자동으로 로드하고 화면에 띄울 UI 프리팹들을 할당하세요.")]
    [AssetReferenceUILabelRestriction("ScenedUi")]
    [SerializeField] private AssetReferenceUi[] _initialScenedUis;

    [Header("전역 UI 참조 (선택사항/테스트용)")]
    [AssetReferenceUILabelRestriction("GlobalUi")]
    [SerializeField] private AssetReferenceUi _someGlobalUi;

    // 매니저 참조 캐싱
    private GlobalUiManager _globalUiManager;
    private ScenedUiManager _scenedUiManager;

    public IEnumerator Initialize()
    {
        // 1. 매니저 참조 가져오기
        _globalUiManager = GameManager.GetManager<GlobalUiManager>();
        _scenedUiManager = WorldManager.GetManager<ScenedUiManager>();

        // 2. 인스펙터에 할당된 초기 UI들을 비동기로 로드하고 화면에 띄움
        foreach (AssetReferenceUi assetRef in _initialScenedUis)
        {
            if (assetRef.RuntimeKeyIsValid())
            {
                yield return LoadAndOpenInitialUi(assetRef);
            }
        }

        Debug.Log("[UiController] 초기화 완료");
    }

    public IEnumerator PostInitialize()
    {
        yield break;
    }

    /// <summary>
    /// Addressables를 이용해 초기 UI를 인스턴스화하고 엽니다.
    /// </summary>
    private IEnumerator LoadAndOpenInitialUi(AssetReferenceUi assetRef)
    {
        // UiController의 자식으로 UI를 생성 (원한다면 다른 Canvas Transform을 넘겨도 됨)
        var handle = assetRef.InstantiateAsync(transform);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            if (handle.Result.TryGetComponent(out BaseUi ui))
            {
                // 생성된 UI 초기화 및 열기
                yield return ui.Initialize();
                ui.ClaimOpen();

                // (선택) 만약 생성한 UI를 ScenedUiManager에서도 검색(GetUi)되게 하고 싶다면 등록
                // _scenedUiManager.RegisterDynamicUi(ui); 
            }
        }
        else
        {
            Debug.LogError($"[UiController] 초기 UI 로드 실패: {assetRef.RuntimeKey}");
        }
    }

    // ====================================================================
    // 비즈니스 로직에서 UI를 조작할 때 사용하는 단일 창구 (Facade) 메서드들
    // ====================================================================

    /// <summary>
    /// 전역(Global) UI를 화면에 엽니다.
    /// 사용 예: OpenGlobalUi<InventoryUi>();
    /// </summary>
    public T OpenGlobalUi<T>() where T : BaseUi, IGlobalUi
    {
        T ui = _globalUiManager.GetUi<T>();
        if (ui != null)
        {
            ui.ClaimOpen();
        }
        else
        {
            Debug.LogWarning($"[UiController] 전역 UI ({typeof(T).Name}) 가 매니저에 없습니다.");
        }
        return ui;
    }

    /// <summary>
    /// 씬(Scened) 전용 UI를 화면에 엽니다.
    /// 사용 예: OpenScenedUi<SmithyUi>();
    /// </summary>
    public T OpenScenedUi<T>() where T : BaseUi, IScenedUi
    {
        T ui = _scenedUiManager.GetUi<T>();
        if (ui != null)
        {
            ui.ClaimOpen();
        }
        else
        {
            Debug.LogWarning($"[UiController] 씬 UI ({typeof(T).Name}) 가 매니저에 없습니다.");
        }
        return ui;
    }

    /// <summary>
    /// 열려있는 UI를 닫습니다.
    /// </summary>
    public void CloseUi(BaseUi ui)
    {
        if (ui != null)
        {
            ui.ClaimClose();
        }
    }

    public void Exit()
    {
        // 실제 UI 파괴와 메모리 해제는 ScenedUiManager의 Exit()에서 일괄 처리되므로
        // Controller에서는 별도의 파괴 로직을 신경 쓰지 않아도 됩니다. (관심사 분리)
        Debug.Log("[UiController] Exit 처리됨");
    }
}