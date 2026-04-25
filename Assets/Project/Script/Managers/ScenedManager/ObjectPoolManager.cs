using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MultiObjectPoolManager : MonoBehaviour, IScenedManager
{
    // 씬 매니저 초기화 우선순위
    public int Priority => 0;

    [System.Serializable]
    public class PoolSetup
    {
        public PoolType poolType;
        public GameObject prefab;

        private const int maxCount = 256;
        [Range(1, maxCount)] public int defaultAmount;
        [Range(2, maxCount)] public int defaultCapacity;
        [Range(2, maxCount)] public int maxSize;

#if UNITY_EDITOR

        /// <summary>
        /// 인스펙터 조절시 오류 방지
        /// </summary>
        public void ValidateValues()
        {
            if (defaultAmount > defaultCapacity)
            {
                defaultCapacity = defaultAmount;
            }

            if (defaultCapacity > maxSize)
            {
                maxSize = defaultCapacity;
            }
        }
#endif
    }

    [SerializeField] private List<PoolSetup> poolSetups;

    private Dictionary<PoolType, IObjectPool<GameObject>> _pools;
    private Dictionary<PoolType, GameObject> _prefabs;

    // [추가됨] 하이라키에서 객체들을 묶어줄 부모 Transform을 관리하는 딕셔너리
    private Dictionary<PoolType, Transform> _poolParents;

    // --- [IScenedManager 생명주기 구현] ---

    public IEnumerator Initialize()
    {
        InitializePools();
        yield return null;
    }

    public IEnumerator PostInitialize()
    {
        yield return PreWarming();
    }

    public void Exit()
    {
        if (_pools != null)
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
            _pools.Clear();
            _prefabs.Clear();
            _poolParents.Clear(); // 딕셔너리 비우기
        }
    }

    // --- [풀링 시스템 내부 로직] ---

    private void InitializePools()
    {
        _pools = new Dictionary<PoolType, IObjectPool<GameObject>>();
        _prefabs = new Dictionary<PoolType, GameObject>();
        _poolParents = new Dictionary<PoolType, Transform>();

        foreach (var setup in poolSetups)
        {
            if (_pools.ContainsKey(setup.poolType))
            {
                Debug.LogError($"[MultiObjectPoolManager] 중복된 Pool: {setup.poolType}");
                continue;
            }

            if (setup.prefab == null) continue;

            _prefabs.Add(setup.poolType, setup.prefab);

            // 1. 하이라키 정리를 위한 전용 부모(Folder) 객체 생성
            GameObject parentObj = new GameObject($"[{setup.poolType}_Pool]");
            parentObj.transform.SetParent(this.transform);
            _poolParents.Add(setup.poolType, parentObj.transform);

            // 람다식에서 사용할 수 있도록 현재 타입 변수 캡처
            PoolType currentType = setup.poolType;

            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreateItem(currentType),
                actionOnGet: OnTakeFromPool,
                // 2. 반환될 때 타입을 알 수 있도록 람다식을 이용해 currentType 전달
                actionOnRelease: (obj) => OnReturnedToPool(obj, currentType),
                actionOnDestroy: OnDestroyPoolObject,
#if UNITY_EDITOR
                collectionCheck: true,
#else
                collectionCheck: false,
#endif
                defaultCapacity: setup.defaultCapacity,
                maxSize: setup.maxSize
            );

            _pools.Add(setup.poolType, pool);
        }
    }

    private IEnumerator PreWarming()
    {
        foreach (var setup in poolSetups)
        {
            if (!_pools.TryGetValue(setup.poolType, out IObjectPool<GameObject> pool)) continue;

            List<GameObject> prewarmList = new List<GameObject>(setup.defaultAmount);

            for (int i = 0; i < setup.defaultAmount; i++)
            {
                prewarmList.Add(pool.Get());
            }

            foreach (var obj in prewarmList)
            {
                pool.Release(obj);
            }

            yield return null;
        }
    }

    private GameObject CreateItem(PoolType type)
    {
        // 3. 인스턴스화 할 때부터 전용 부모(_poolParents)의 자식으로 생성
        GameObject obj = Instantiate(_prefabs[type], _poolParents[type]);

        if (obj.TryGetComponent(out IPooledObject pooledItem))
        {
            pooledItem.SetPool(_pools[type]);
            return obj;
        }

        Debug.LogError($"'{type}' 프리팹에 IPooledObject를 구현한 컴포넌트가 없습니다!");
        Destroy(obj);
        return null;
    }

    private void OnTakeFromPool(GameObject obj) => obj.SetActive(true);

    // 4. 매개변수로 PoolType을 추가로 받아 원래 자리로 돌려놓는 로직
    private void OnReturnedToPool(GameObject obj, PoolType type)
    {
        obj.SetActive(false);
        // 플레이 도중 부모가 바뀌었을 수 있으므로(예: 캐릭터에 부착된 이펙트), 원래 풀 폴더로 원대복귀
        obj.transform.SetParent(_poolParents[type]);
    }

    private void OnDestroyPoolObject(GameObject obj) => Destroy(obj);

    // --- [스폰 메서드] ---

    public GameObject Spawn2D(PoolType type, Vector2 position2D)
    {
        return Spawn(type, new Vector3(position2D.x, position2D.y, 0));
    }

    public GameObject Spawn(PoolType type)
    {
        if (!_pools.ContainsKey(type))
        {
            Debug.LogError($"Pooling key 오류 : [{type.ToString()}: {(int)type}]");
            return null;
        }
        GameObject obj = _pools[type].Get();
        if(obj == null)
        {
            Debug.LogWarning($"Pooling Spawn 실패 : [{type.ToString()}: {(int)type}]");
            return null;
        }

        return obj;
    }

    public GameObject Spawn(PoolType type, Vector3 position)
    {
        GameObject obj = Spawn(type);
        if(obj != null) obj.transform.position = position;
        return obj;
    }

#if UNITY_EDITOR
    HashSet<PoolType> ___typeCheckSet = new HashSet<PoolType>();

    private void OnValidate()
    {
        foreach (PoolSetup poolSetup in poolSetups)
        {
            poolSetup.ValidateValues();

            if (poolSetup.prefab == null)
            {
                Debug.LogWarning($"[MultiObjectPoolManager] {poolSetup.poolType}의 프리팹이 비어있음");
            }

            if (!___typeCheckSet.Add(poolSetup.poolType))
            {
                Debug.LogError($"[MultiObjectPoolManager] 인스펙터에 {poolSetup.poolType} 풀이 중복해서 등록되어 있음");
            }
        }
        ___typeCheckSet.Clear();
    }
#endif
}