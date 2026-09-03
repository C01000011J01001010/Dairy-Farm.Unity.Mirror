using CoreEngine;
using CoreEngine.Actor;
using CoreEngine.Data;
using CoreEngine.Facades;
using System;
using UnityEngine;

public class BaseDatabaseAccess<DatabaseManager/*정적데이터를 관리하는 객체*/, DataType/*정적데이터SO*/> 
    : BaseActorFeature, IActorFeature, IDisposable
    where DatabaseManager : BaseStaticDataManager<DataType>
    where DataType : BaseData
{
    private DatabaseManager databaseManager;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        databaseManager = CoreFacade.GetManager<DatabaseManager>();
        if (databaseManager == null)
        {
            Debug.LogError($"정적데이터 관리자({typeof(DatabaseManager).Name}) 객체 없음");
            return;
        }
    }

    public virtual void Dispose() { }

    protected virtual DataType GetData(int ID)
    {
        // ItemManager를 통해 ID에 해당하는 원본 데이터를 가져옴
        DataType data = databaseManager.GetRecord(ID);

        if (data == null)
        {
            Debug.LogError($"[{typeof(DatabaseManager).Name}] 테이블에 {ID}번 데이터가 존재하지 않음");
            return null;
        }
        return data;
    }

    
}