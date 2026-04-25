using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolRequest<T_Type>
{
    public T_Type wantType;
    public int amount;
}


public delegate GameObject DelegateInstantiate(string key, Transform parent,
        Vector3 pos, Quaternion rot, Vector3 localScale, Space coordinate);
public delegate void DelegateRelease(GameObject target);

public class Disabled_PoolManager : MonoBehaviour, IScenedManager
{
    [SerializeField] private int initialPriority = -1;
    public int Priority => initialPriority;

    [SerializeField] PoolRequest<Type_Character>[] requestCharacter;
    //[SerializeField] PoolRequest<Type_Effect>[] requestEffect;
    //[SerializeField] PoolRequest<Type_Weapon>[] requestWeapon;

    #region 쓸 수도 있으니 지우지 마셈
    private event DelegateInstantiate _OnSpawn;
    private event DelegateRelease _OnDespawn;

    public event DelegateInstantiate OnSpawn
    {
        add
        {
            _OnSpawn -= value;
            _OnSpawn += value;
        }
        remove => _OnSpawn -= value;
    }
    public event DelegateRelease OnDespawn
    {
        add
        {
            _OnDespawn -= value;
            _OnDespawn += value;
        }
        remove => _OnDespawn -= value;
    }
    #endregion

    // Queue를 쓰면 => 정해진 입구 정해진 출구 한번만 실행을 하면 추가/제거가 쉽다
    // 선입선출 -> pooling을 위해서 미리 만들어놓은 오브젝트를 준비
    // 먼저 들어왔다...의 의미 => 여기에서 가장 오래된 데이터다
    // stack을 쓰는 경우 => 한 프레임에 추가와 삭제가 동시에 되는 경우 주의
    //                  => 유니티에서 껐다 켤 때 문제가 생길 수 있다 => 초기화문제, 무결성 문제
    Dictionary<string, Queue<GameObject>> poolDict = new();

    // 풀링될 오브젝트들을 정리하기 위한 객체
    Transform rootParent;
    Dictionary<string, Transform> parentDict = new();

    public virtual void Exit()
    {
        foreach(Queue<GameObject> memoryPool in poolDict.Values)
        {
            foreach(GameObject obj in memoryPool)
            {
                if (obj != null && obj.TryGetComponent(out IInitialize initialziable))
                {
                    initialziable.Exit();
                }
            }
        }
    }
    public virtual IEnumerator Initialize()
    {
        // 풀링된 객체의 기본 루트
        rootParent = new GameObject("PoolRoot").transform;

        yield return Register(requestCharacter, FileManager.GetCharacterPrefab);

        yield return null;
    }

    public IEnumerator PostInitialize()
    {
        yield break;
    }

    protected IEnumerator Register<T_Enum>(PoolRequest<T_Enum>[] request, Func<T_Enum, GameObject> GetPrefab)
    {
        foreach (var current in request)
        {
            GameObject prefab = GetPrefab(current.wantType);
            yield return RegisterFromObject(prefab, current.wantType.ToString(), current.amount);
            yield return null;
        }
    }


    protected IEnumerator RegisterFromObject(GameObject prefab, string wantTypeName, int amount = 1)
    {
        if(prefab is null)
        {
            Debug.LogError($"Prefab of [{wantTypeName}] is null");
            yield break;
        }
        if (amount == 0)
        {
            Debug.LogWarning($"Requested _amount of [{wantTypeName}] is 0");
            yield break;
        }

        string key = prefab.name;
        Queue<GameObject> targetQueue;
        if(!poolDict.TryGetValue(key, out targetQueue)) // 등록된 키가 없다면
        {
            // key가 등록된 적 없을 경우 값의 참조는 문제가 되지만
            // 할당은 새로운 키 값 pair를 등록하는 방법이 됨
            // poolDict.Add(key, targetQueue = new()); // 이것과 같음, 단 Add는 중복된 키에 대해 오류로 판단함
            poolDict[key] = targetQueue = new();

            // 타입의 이름을 가진 오브젝트를 만들어서 루트에 넣어주기
            parentDict[key] = new GameObject(key).transform;
            parentDict[key].SetParent(rootParent);
        }
            

        GameObject result;
        for (int i = 0; i < amount; i++)
        {
            result = Instantiate(prefab);
            result.name = key;
            yield return Registration(result, targetQueue, parentDict[key]);
            yield return null;
        }
    }
    protected IEnumerator Registration(GameObject target, Queue<GameObject> queue, Transform parent)
    {
        if (target == null || queue is null) yield break;

        target.SetActive(false);
        queue.Enqueue(target);
        target.transform.SetParent(parent);

        if(target.TryGetComponent(out IPoolable poolable))
        {
            poolable.RootQueue = queue;
        }
        if(target.TryGetComponent(out IInitialize initializable))
        {
            yield return initializable.Initialize();
        }
    }

    protected Transform GetRoot(string key)
    {
        return parentDict[key];
    }


    private GameObject GetInstanceFromPool(string key)
    {
        if(poolDict.TryGetValue(key, out var queue))
        {
            if(queue.TryDequeue(out var instance))
            {
                if(queue.Count == 0)
                {
                    StartCoroutine(RegisterFromObject(instance, instance.name, 2));
                }
                return instance;
            }
        }
        return null;
    }

    private GameObject GetFromPool(string key, Transform parent,
        Vector3 pos, Quaternion rot, Vector3 localScale, SpawnEffectVal effectVal, Space coordinate)
    {
        GameObject instance = GetInstanceFromPool(key);

        // 인스턴스가 없구나...
        if (instance is null) return null;

        Transform objTransform = instance.transform;

        objTransform.SetParent(parent);
        if(coordinate == Space.World)
        {
            objTransform.position = pos;
            objTransform.rotation = rot;
            // lossyScale을 구하는 법
            // 부모를 기준으로 localScale을...
            // transform.localScale = scale;
        }
        else
        {
            objTransform.localPosition = pos;
            objTransform.localRotation = rot;
        }
        objTransform.localScale = localScale;

        /* 만약여기서 TryGetComponent가 아닌 그냥 초기화를 바로 사용하려면
         * 별다른 거 없이 그냥 바로 Dict에 IPoolable과 MonoBehaviour를 상속받는
         * 추상클래스를 만들어서 사용
         */
        if (instance.TryGetComponent(out IPoolable asPoolable))
        {
            asPoolable.OnSpawn();
        }
        else
        {
            Debug.LogWarning($"{instance.name} is not IPoolable");
        }

        instance.SetActive(true);
        if (effectVal && instance.TryGetComponent(out EffectController effectController))
        {
            effectController.PlaySpawnEffect(effectVal.effectType, effectVal.delay);
        }

        return instance;
    }

    private void ReturnToPool(GameObject target, SpawnEffectVal effectVal)
    {
        if(target?.TryGetComponent(out IPoolable asPool) ?? false)
        {
            // 돌아갈 자리 찾기
            Queue<GameObject> rootQueue = asPool.RootQueue;

            if (rootQueue is not null)
            {
                rootQueue.Enqueue(target);
                target.transform.parent = GetRoot(target.name);
                asPool.OnDespawn();
            }
        }
        else
        {
            Debug.LogWarning($"{target.name} is not IPoolable");
        }

        if (effectVal && target.TryGetComponent(out EffectController effectController))
        {
            // 이펙트 끝나면 객체 비활성화됨
            effectController.PlayDespawnEffect(effectVal.effectType, effectVal.delay);
        }
        else
        {
            target.SetActive(false);
        }
    }

    public GameObject ClaimGet(string key, Transform parent,
        Vector3 pos, Quaternion rot, Vector3 localScale, SpawnEffectVal effectVal = null, Space coordinate = Space.Self)
    {
        return GetFromPool(key, parent, pos, rot, localScale, effectVal, coordinate);
    }

    public GameObject ClaimGet(string key, Transform parent, Vector3 position, SpawnEffectVal effectVal = null)
    {
        return GetFromPool(key, parent, position, Quaternion.identity, Vector3.one, effectVal,  Space.Self);
    }

    public GameObject ClaimGet(string key, Transform parent, SpawnEffectVal effectVal = null)
    {
        return GetFromPool(key, parent, Vector3.zero, Quaternion.identity, Vector3.one, effectVal, Space.Self);
    }

    public GameObject ClaimGet(string key, Vector3 position, SpawnEffectVal effectVal = null)
    {
        return GetFromPool(key, null, position, Quaternion.identity, Vector3.one, effectVal, Space.Self);
    }

    public GameObject ClaimGet(string key, Vector3 position, Quaternion rotation, SpawnEffectVal effectVal = null)
    {
        return GetFromPool(key, null, position, rotation, Vector3.one, effectVal, Space.Self);
    }

    public GameObject ClaimGet(string key, SpawnEffectVal effectVal = null)
    {
        return GetFromPool(key, null, Vector3.zero, Quaternion.identity, Vector3.one, effectVal, Space.Self);
    }

    public void ClaimReturn(GameObject target, SpawnEffectVal effectVal = null)
    {
        ReturnToPool(target, effectVal);
    }
}
