using System;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseHub<THub, TManager, TGameObject> : MonoBehaviour
    where THub : BaseHub<THub, TManager, TGameObject>
    where TManager :  IManager, IPostInitialize
    where TGameObject :  IInitialize, IPostInitialize, IPriority
{
    protected static THub _instance;
    public static THub Inst => _instance;

    // 개별 순서를 가진 최우선 초기화 대상들
    protected List<TManager> PreSetManagerList = new();

    protected Dictionary<Type, TManager> managerDict = new();
    protected Dictionary<Type, List<TGameObject> /*중복된 객체 허용*/> objectsDict = new();


    // PreSetManagerList의 초기화 대상들
    protected virtual bool RegisterManager() { return true; }

    protected bool TryGetOrAddManager<T>() where T : MonoBehaviour, TManager
    {
        T manager = gameObject.GetOrAddComponent<T>();
        if (manager is null)
        {
            Debug.LogAssertion($"GetOrAddComponent Failed => {typeof(T).Name}");
            return false;
        }

        if (!managerDict.TryAdd(manager.GetType(), manager))
        {
            Debug.LogWarning($"Manager({typeof(T).Name}) is alreay Added");
            return false;
        }
        PreSetManagerList.Add(manager);
        return true;
    }

    public static T GetObject<T>() where T : MonoBehaviour, TGameObject
    {
        // 딕셔너리에 들어있는 리스트에서 첫번째 원소를 반환
        List<TGameObject> rawObjects = GetRawObjects<T>();
        TGameObject result = default;
        if (rawObjects != null) result = rawObjects[0];
        return (T)result;
    }

    public static T[] GetObjects<T>() where T : MonoBehaviour, TGameObject
    {
        // 딕셔너리에 있는 리스트를 T로 캐스팅하여 배열로 반환
        return GetRawObjects<T>()?.Cast<T>().ToArray();
    }

    private static List<TGameObject> GetRawObjects<T>() where T : MonoBehaviour, TGameObject
    {
        Type wantType = typeof(T);

        if (Inst.objectsDict.ContainsKey(wantType))
        {
            return Inst.objectsDict[wantType];
        }
        Debug.LogWarning($"Type({wantType.Name}) 객체 없음");
        return null;
    }

    public static T GetManager<T>() where T : MonoBehaviour, TManager
    {
        Type managerType = typeof(T);

        if (Inst.managerDict.TryGetValue(managerType, out TManager manager))
        {
            return (T)manager;
        }

        Debug.LogError("정의되지 않은 매니저 객체");
        return null;
    }
}