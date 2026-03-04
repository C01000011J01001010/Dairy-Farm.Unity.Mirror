using System.Collections;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IScenedInitialize
{
    [SerializeField] private int priority = 1;
    [Header("Quick Slots")]
    public InfoItemContainer[] itemContainer = new InfoItemContainer[9];

    private ItemManager itemManager;

    public int Priority => priority;

    public IEnumerator Initialize()
    {
        // 게임 시작 시 빈 슬롯으로 깔끔하게 초기화
        for (int i = 0; i < itemContainer.Length; i++)
        {
            itemContainer[i] = new InfoItemContainer();
        }
        itemManager = GameManager.GetManager<ItemManager>();
        yield return null;
    }

    public IEnumerator LateInitialize()
    {
#if UNITY_EDITOR
        Debug.LogWarning("테스트 구문");
        AcquireItem(1, 1);
        AcquireItem(1, 2);
        AcquireItem(3, 3);
        AcquireItem(101, 99);
        AcquireItem(201, 99);
        AcquireItem(301, 99);
        AcquireItem(401, 99);
#endif
        yield break;
    }

    public void Exit()
    {

    }

    /// <summary>
    /// 필드에서 아이템을 획득했을 때 호출 (itemID만 넘겨주면 됨!)
    /// </summary>
    public void AcquireItem(int itemID, int count = 1)
    {
        int curCount = count;
        if (curCount < 0) return;

        // ItemManager를 통해 ID에 해당하는 원본 데이터를 가져옴
        BaseItem newItem = itemManager.GetItem(itemID);

        if (newItem == null)
        {
            Debug.LogError($"[Inventory] {itemID}번 아이템 데이터가 존재하지 않아 획득 불가!");
            return;
        }

        // 이미 같은 아이템이 있는 공간 찾기
        foreach (var container in itemContainer)
        {
            if (!container.IsEmpty() && container.Get() == newItem)
            {
                ChangeInPossessionAmount(container, newItem, curCount, out curCount);
                break;
            }
        }

        if (curCount < 0) return;

        // 같은 아이템이 없다면, 비어있는 슬롯 찾아서 새로 등록
        foreach (var container in itemContainer)
        {
            // 비었다면 설정후 개수 변화
            if (container.IsEmpty() && container.Set(newItem))
            {
                ChangeInPossessionAmount(container, newItem, curCount, out curCount);
                return;
            }
        }

        if(curCount > 0)
        {
            // 슬롯이 꽉 찼을 때의 처리 (아이템을 땅에 다시 떨구거나, 무시하거나)
            Debug.LogWarning("인벤토리(퀵슬롯)가 꽉 찼습니다!");
        }
    }

    /// <summary>
    /// 아이템 개수 변경
    /// </summary>
    private void ChangeInPossessionAmount(InfoItemContainer container, BaseItem newItem, int count, out int remain)
    {
        container.Push(count, out remain);
        Debug.Log($"[획득] {newItem.nameTag} {count - remain}개 획득! (현재 슬롯에 총 {container.amount}개)");
        if (remain > 0)
        {
            Debug.Log($"아이템({newItem.nameTag}) 상한 도달 -> {remain}개 획득 불가");
            // TODO 아이템을 상한 이상으로 획득시 처리할 이벤트 넣을 곳
        }
    }

    /// <summary>
    /// 특정 슬롯의 아이템을 사용 (0번, 1번, 2번 슬롯)
    /// </summary>
    public void UseItem(BaseCharacter user, int slotIndex, int count = 1)
    {
        if (slotIndex < 0 || slotIndex >= itemContainer.Length)
        {
            Debug.LogError($"슬롯번호 {slotIndex}는 슬롯 범위를 벗어남");
            return;
        }

        InfoItemContainer container = itemContainer[slotIndex];
        if (container.IsEmpty())
        {
            Debug.Log("빈 슬롯입니다.");
            return;
        }

        // 아이템 타입에 따른 사용
        BaseItem cur = container.Get();
        bool isSuccess = container.TryUse(user, count);
        // 2. 사용 결과에 따른 처리
        if (isSuccess)
        {
            // 성공했으니 종류에 따라 내구도를 깎거나 개수를 줄임!
            if (cur is Item_Equipment)
            {
                container.ReduceDurability(count);
            }
            else
            {
                container.Pop(count, out _);
            }
        }
        else
        {
            Debug.LogWarning("사용 실패시 효과음 출력 바람");
        }
    }



    
}