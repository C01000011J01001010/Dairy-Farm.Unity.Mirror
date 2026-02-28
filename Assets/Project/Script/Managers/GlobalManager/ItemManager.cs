using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour, IGlobalManager
{
    // 아이템 ID를 키(Key)로 사용하여 빠르게 검색하기 위한 딕셔너리
    private Dictionary<int, ItemObject> itemDatabase = new Dictionary<int/*id*/, ItemObject>();

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
        itemDatabase = fileManager.LoadAllGameData<ItemObject>("ScriptableObject/Items");

        if(itemDatabase.IsNullOrEmpty())
        {
            Debug.LogError("아이템 데이터 로드 실패");
            yield break;
        }
        Debug.Log($"[ItemManager] 총 {itemDatabase.Count}개의 아이템 데이터 로드 성공");
    }

    /// <summary>
    /// ID를 통해 특정 아이템 데이터를 가져옵니다.
    /// </summary>
    public ItemObject GetItem(int id)
    {
        if (itemDatabase.TryGetValue(id, out ItemObject item))
        {
            return item;
        }

        Debug.LogWarning($"[ItemManager] 해당 ID의 아이템을 찾을 수 없습니다: {id}");
        return null;
    }

    
}