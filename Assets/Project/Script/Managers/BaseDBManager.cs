using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 하나의 정적 데이터(Table)를 관리하는 매니저의 기반구조
/// </summary>
public abstract class BaseDBManager<TDataObject> : BaseGlobalManager, IGlobalManager
    where TDataObject : BaseData
{
    protected abstract string Directory {get;}
    // 아이템 ID를 키(Key)로 사용하여 빠르게 검색하기 위한 딕셔너리
    protected Dictionary<int, TDataObject> database = new Dictionary<int/*id*/, TDataObject>();
    public void Exit()
    {

    }

    public IEnumerator Initialize()
    {
        yield return InitializeDatabase();
        yield return null;
    }

    private IEnumerator InitializeDatabase()
    {
        // Resources/Items 경로 안에 ScriptableObject들을 모아두었다고 가정
        FileManager fileManager = GameManager.GetManager<FileManager>();
        database = fileManager.LoadAllGameData<TDataObject>(Directory);

        if (database.IsNullOrEmpty())
        {
            Debug.LogError($"[{this.GetType().Name}] 데이터 로드 실패");
            yield break;
        }
        Debug.Log($"[{this.GetType().Name}] 총 {database.Count}개의 데이터 로드 성공");
    }

    /// <summary>
    /// ID를 통해 DB의 특정 레코드(데이터)를 가져옴
    /// </summary>
    public TDataObject GetRecord(int id)
    {
        if (database.TryGetValue(id, out TDataObject record))
        {
            return record;
        }

        Debug.LogWarning($"[{this.GetType().Name}] 해당 ID의 레코드(데이터)를 찾을 수 없음: {id}");
        return null;
    }
}