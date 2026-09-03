using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


[RequireComponent(typeof(baseCharacterAnim))]
[RequireComponent(typeof(CharacterStateController))]
[RequireComponent(typeof(Rigidbody2D))]
public class BaseCharacter : MonoBehaviour, IScenedInitialize
{
    [SerializeField] protected int _priority = 11;
    public int Priority => _priority;


    public baseCharacterAnim anim {  get; protected set; }
    protected Rigidbody2D rigidBody;

    public Vector2 inputMove;
    public bool isMove;
    public bool isSprint;
    public float moveSpeed = 2.5f;
    public float SprintMul = 2f;

    protected bool isReady;

    private List<IActorFeature> modules = new(); // 모듈 초기화, 업데이트 순서용
    private Dictionary<Type, IActorFeature> moduleDict = new();// 중복방지, Get용


    private void OnDisable()
    {
        if (isReady)
        {
            Unsubscribe();
        }
    }

    private void OnEnable()
    {
        if (isReady)
        {
            Subscribe();
        }
    }

    public virtual void Exit()
    {
        // 초기화 순서와 반대로 종료
        for(int i = modules.Count-1; i >= 0 ; i--)
        {
            modules[i].Exit();
        }

        if (isReady)
        {
            Unsubscribe();

            // OnDisable에서 다시 Unsubscribe하지 않도록 플래그를 끄기
            isReady = false; 
        }
    }

    public virtual IEnumerator Initialize()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<baseCharacterAnim>();
        TryAddCharacterModule<CharacterStateController>();
        TryAddCharacterModule<CharacterInventory>();
        TryAddCharacterModule<CharacterCropDataSheet>();

        
        foreach(IActorFeature module in modules)
        {
            module.Initialize(this);
        }
        yield return null;
    }
    public virtual IEnumerator PostInitialize()
    {
        foreach (IActorFeature module in modules)
        {
            module.PostInitialize();

            // 후처리까지 끝났으니 Tick 시작
            module.SetActive(true);
        }
        yield return null;
        isReady = true;
        Subscribe();
    }

    protected void TryAddCharacterModule<Module>() where Module : IActorFeature
    {
        IActorFeature module = GetComponent<Module>();
        if(module == null)
        {
            Debug.LogError($"이 모듈({typeof(Module).Name})은 캐릭터에 준비되지 않음 모듈");
        }
        Type moduleType = module.GetType();

        // 모듈 중복 방지
        if(module != null && !moduleDict.ContainsKey(moduleType))
        {
            modules.Add(module);
            moduleDict.Add(moduleType, module);
        }
        else
        {
            Debug.LogError($"{typeof(Module).Name}을 캐릭터에 준비하지 않았음");
        }
    }

    public T GetFeature<T>() where T : class, IActorFeature
    {
        Type moduleType = typeof(T);
        if (moduleDict.TryGetValue(moduleType, out IActorFeature manager))
        {
            return (T)manager;
        }

        Debug.LogError($"Object({moduleType.Name}) is not in moduleDict");
        return null;
    }

    protected void Subscribe()
    {
        CharacterManager mng = WorldManager.GetManager<CharacterManager>();
        mng?.AddList(this);
    }

    protected void Unsubscribe()
    {
        CharacterManager mng = WorldManager.GetManager<CharacterManager>();
        mng?.RemoveList(this);
    }

    public virtual void Move(Vector2 input)
    {
        inputMove = input;
        isMove = input.sqrMagnitude > 0.01f;

        // 3. Scale -1을 이용한 좌우 반전 로직
        // x값이 0일 때는 마지막 방향을 유지하기 위해 '0이 아닐 때만' 업데이트
        if (input.x != 0)
        {
            float direction = input.x > 0 ? 1f : -1f;

            // 부모의 Scale을 뒤집어 하위 무기, 이펙트 위치까지 한꺼번에 반전
            transform.localScale = new Vector3(direction, 1f, 1f);
        }
        // 위 아래 일 시 정상 scale로 변경
        else if(input.y != 0)
        {
            transform.localScale = Vector3.one;
        }
    }

    public void SprintHold(bool value) => isSprint = value;

    //public void SprintToggle() => isRun = !isRun;

    public virtual void Tick() 
    {
        foreach(IActorFeature module in modules)
        {
            if (module.IsActive)
            {
                module.Tick(Time.deltaTime);
            }
        }
    }

    public virtual void FixedTick()
    {
        foreach (IActorFeature module in modules)
        {
            if (module.IsActive)
            {
                module.FixedTick(Time.fixedDeltaTime);
            }
        }

        if(isMove)
        {
            Physics_Move();
        }
    }

    private void Physics_Move()
    {
        Vector2 nextVec = inputMove.normalized * moveSpeed * Time.fixedDeltaTime;
        if (isSprint) nextVec *= SprintMul;
        rigidBody.MovePosition(rigidBody.position + nextVec);
    }
}
