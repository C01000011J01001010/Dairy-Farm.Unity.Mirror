using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GlobalHub = BaseHub<GameManager, IGlobalManager, IGlobalGameObject>;



public delegate void DelegateUpdate();


public sealed class GameManager : GlobalHub
{
    public bool IsInit { get; private set; }
    //private List<IGlobalManager> managerList = new(); // 초기화 순서에 사용
    //private Dictionary<Type, IGlobalManager> managerDict = new(); // 접근에 사용

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

    //private static GameManager _instance;
    //public static GameManager Inst => _instance;

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

        for (int i = PreSetManagerList.Count - 1; i >= 0; i--)
        {
            IGlobalManager manager = PreSetManagerList[i];
            if(manager != null && manager.IsInit)
            {
                PreSetManagerList[i].Exit();
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

        if (!RegisterManager())
        {
            Debug.LogError("GameManager에서 일부 Manager 등록 실패");
        }
    }

    private IEnumerator Start()
    {
        yield return null; // 로딩 ui에 초기화 우선권 부여
        yield return Initializer = Initialize();
    }

    public IEnumerator Initialize()
    {
        // UiManager를 먼저 추가하여 로딩 화면을 보여줌
        //TryGetOrAddManager<UIManager>();
        //IGlobalManager uiManager = GetManager<UIManager>();
        //yield return uiManager?.Initialize();
        //uiManager.EndInit();

        // LoadingScreen의 Start에서 초기화 끝냈으니 바로 사용
        GlobalUiManager.ClaimLoading_Start(PreSetManagerList.Count);
        foreach (var manager in PreSetManagerList)
        {
            string loadingMessage = GetManagerLoadingMessage(manager);
            GlobalUiManager.ClaimLoading_Next(loadingMessage);
            yield return manager.Initialize();
            manager.EndInit();
            yield return null;

            // TODO: WorldManager처럼 Global 객체의 초기화도 추가하자
            // TODO: 
        }
        GlobalUiManager.ClaimLoading_End();

        //yield return ProcessManagerLoading();
        

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
            GlobalUiManager.ClaimLoading_Start(-1);
            GlobalUiManager.ClaimLoading_Next("", 30.0f);
            SceneManager.SetActiveScene(SceneTester.TestScene);
            yield return WorldManager.Inst.Initialize();
            GlobalUiManager.ClaimLoading_End();
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

    protected override bool RegisterManager()
    {
        bool result = true;
        result &= TryGetOrAddManager<PathManager>();
        result &= TryGetOrAddManager<FileManager>();
        result &= TryGetOrAddManager<GlobalUiManager>();
        result &= TryGetOrAddManager<OptionManager>();
        result &= TryGetOrAddManager<AudioManager>();
        result &= TryGetOrAddManager<UserInputManager>();
        result &= TryGetOrAddManager<DragManager>();
        result &= TryGetOrAddManager<ItemStaticManager>();
        result &= TryGetOrAddManager<QuestStaticManager>();
        result &= TryGetOrAddManager<CropStaticManager>();
        result &= TryGetOrAddManager<SceneLoadManager>();
        result &= TryGetOrAddManager<TimeManager>();
        return result;
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
        _=> "기타 로딩중..."
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
