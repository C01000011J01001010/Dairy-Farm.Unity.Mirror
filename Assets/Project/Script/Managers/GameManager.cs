using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public delegate void DelegateUpdate();


public sealed class GameManager : MonoBehaviour, IManager
{
    public bool IsInit { get; private set; }
    private List<IGlobalManager> managerList = new(); // 초기화 순서에 사용
    private Dictionary<Type, IGlobalManager> managerDict = new(); // 접근에 사용

    #region Event_Update
    // 프레임 시작 시 데이터를 초기화하는 단계의 업데이트
    public static event DelegateUpdate UPDATE_Initial;

    // 컨트롤러의 업데이트 진행! => 파악된 상황을 통해 캐릭터에 정보 전달
    public static event DelegateUpdate UPDATE_OnController;

    // 전달된 정보를 통해서 캐릭터가 활동
    public static event DelegateUpdate UPDATE_OnCharacter;

    // 캐릭터가 오브젝트에 관여하고 나서 오브젝트가 발동
    public static event DelegateUpdate UPDATE_Object;
    #endregion

    #region Event_LateUpdate
    public static event DelegateUpdate UPDATE_Camera;

    // 프레임 종료 시 변환된 데이터를 정리하는 단계의 업데이트
    public static event DelegateUpdate UPDATE_Post;
    #endregion

    #region Event_FixedUpdate
    // 매 FixedUpdate마다 물리적 업데이트를 해주는 것
    public static event DelegateUpdate UPDATE_Physics;
    #endregion

    private bool pauseUpdate;

    IEnumerator Initializer;

    private static GameManager _instance;
    public static GameManager Inst => _instance;

    private void OnDisable()
    {
        Exit();
    }

    public void Exit()
    {
        // 강제종료시 초기화 코루틴이 있는데 아직 초기화가 완료되지 않은 경우
        if (Initializer is not null && IsInit is false)
        {
            // 중간에 정지를 시킬 수 있도록 변수로 빼둔 것!
            StopCoroutine(Initializer);
        }

        for (int i = managerList.Count - 1; i >= 0; i--)
        {
            IGlobalManager manager = managerList[i];
            if(manager != null && manager.IsInit)
            {
                managerList[i].Exit();
            }
        }
        _instance = null;
    }

    private void Awake()
    {
        if (!this.TryMakeSingleton(ref _instance))
        {
            Destroy(this);
        }
    }

    private IEnumerator Start()
    {
        yield return Initializer = Initialize();
    }

    public IEnumerator Initialize()
    {
        // UiManager를 먼저 추가하여 로딩 화면을 보여줌
        TryGetOrAddManager<UIManager>();
        IGlobalManager uiManager = GetManager<UIManager>();
        yield return uiManager?.Initialize();
        uiManager.EndInit();

        // 모든 매니저 초기화 실행
        yield return ProcessManagerLoading();
        UIManager.ClaimLoading_End();

        SceneLoadManager loadManager = GetManager<SceneLoadManager>();

#if UNITY_EDITOR
        // 테스트 씬인 경우에 사용
        if (SceneManager.sceneCount == 1)
#endif
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
            loadManager.ChangeScene(Constants.SCENE_NAME_TitleScene);
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.

#if UNITY_EDITOR
        else
        {
            UIManager.ClaimLoading_Start(-1);
            UIManager.ClaimLoading_Next("", 30.0f);
            SceneManager.SetActiveScene(SceneTester.TestScene);
            yield return WorldManager.Inst.Initialize();
            UIManager.ClaimLoading_End();
        }
#endif

        IsInit = true;
    }


    public void ClearEventUpdate()
    {
        UPDATE_Initial = null;
        UPDATE_OnController = null;
        UPDATE_OnCharacter = null;
        UPDATE_Object = null;
        UPDATE_Camera = null;
        UPDATE_Post = null;
        UPDATE_Physics = null;
    }


    private void FixedUpdate()
    {
        if (pauseUpdate) return;

        if (IsInit && WorldManager.IsInit)
        {
            UPDATE_Physics?.Invoke();
        }
    }

    private void Update()
    {
        if (pauseUpdate) return;

        //  프레임초기화 -> 컨트롤러 -> 캐릭터 -> 사물 -> 모든 상황정리
        if (IsInit && WorldManager.IsInit) // 업데이트는 이니셜라이즈가 된 뒤에만 함
        {
            UPDATE_Initial?.Invoke();
            UPDATE_OnController?.Invoke();
            UPDATE_OnCharacter?.Invoke();
            UPDATE_Object?.Invoke();
        }
    }

    private void LateUpdate()
    {
        if (pauseUpdate) return;

        if (IsInit && WorldManager.IsInit)
        {
            UPDATE_Camera?.Invoke();
            UPDATE_Post?.Invoke();
        }
    }

    private IEnumerator ProcessManagerLoading()
    {
        UIManager.ClaimLoading_Start(8);

        yield return ProcessManagerLoading<PathManager>();
        yield return ProcessManagerLoading<FileManager>();
        yield return ProcessManagerLoading<OptionManager>();
        yield return ProcessManagerLoading<AudioManager>();
        yield return ProcessManagerLoading<UserInputManager>();
        yield return ProcessManagerLoading<DragManager>();

        yield return ProcessManagerLoading<ItemManager>();
        yield return ProcessManagerLoading<SceneLoadManager>();
    }

    private IEnumerator ProcessManagerLoading<T>() where T : MonoBehaviour, IGlobalManager
    {
        // 매니저를 찾거나 생성해서 컨테이너(리스트, 딕셔너리)에 넣기
        if (TryGetOrAddManager<T>())
        {
            // 성공했으면 꺼내서 로딩 시작하고 초기화
            T manager = GetManager<T>();
            string loadingMessage = GetManagerLoadingMessage(manager);
            UIManager.ClaimLoading_Next(loadingMessage);
            yield return manager.Initialize();
            manager.EndInit();
        }
        yield return null;
    }

    private bool TryGetOrAddManager<T>() where T : MonoBehaviour, IGlobalManager
    {
        T manager = gameObject.GetOrAddComponent<T>();
        if(manager is null)
        {
            Debug.LogAssertion($"GetOrAddComponent Failed => {typeof(T).Name}");
            return false;
        }

        if (!managerDict.TryAdd(manager.GetType(), manager))
        {
            Debug.LogWarning($"Manager({typeof(T).Name}) is alreay Added");
            return false;
        }
        managerList.Add(manager);
        return true;
    }

    // 타입패턴
    public string GetManagerLoadingMessage(IGlobalManager manager) => manager switch
    {
        PathManager     => "파일 경로를 초기화 중...",
        FileManager        => "파일을 불러오는 중...",
        OptionManager      => "옵션 초기화 중...",
        AudioManager       => "오디오 초기화 중...",
        UserInputManager   => "유저 입력디바이스를 조정중...",
        SceneLoadManager   => "씬을 불러오는 중...",
        _=> "로딩중..."
    };

    public static T GetManager<T>() where T : class, IGlobalManager
    {
        Type managerType = typeof(T);
        if(Inst.managerDict.TryGetValue(managerType, out IGlobalManager manager))
        {
            return (T)manager;
        }

        Debug.LogError($"Object({managerType.Name}) is not in managerDict");
        return null;
    }

    public static bool SetPauseUpdate(bool paused) => Inst.pauseUpdate = paused;

    

}
