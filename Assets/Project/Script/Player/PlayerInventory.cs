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
        AcquireItem(101, 2);
        AcquireItem(201, 10);
        AcquireItem(301, 40);
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
        ItemObject newItem = itemManager.GetItem(itemID);

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
            if (container.IsEmpty())
            {
                container.Set(newItem);
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

    private void ChangeInPossessionAmount(InfoItemContainer container, ItemObject newItem, int count, out int remain)
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
    public void UseItem(int slotIndex, int count = 1)
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

        ItemObject cur = container.Get();

        int remain;

        // 아이템 타입에 따른 사용 분기
        if (cur.itemType == ItemType.Consumable)
        {
            container.Pop(count, out remain);
            Debug.Log($"[사용] {cur.nameTag} {count - remain}개 사용! (현재 슬롯에 총 {container.amount}개)");

            // TODO 아이템 사용 이벤트

            if (remain > 0)
            {
                Debug.Log($"아이템({cur.nameTag}) 재고 없음 -> {remain}개 사용 불가");
                container.Clear();
                // TODO 아이템을 다 사용한 경우 이벤트 추가
            }

        }
        else if (cur.itemType == ItemType.Equipment)
        {
            bool result = container.TryReduceDurability();
            if(result)
            {
                //TODO 아이템 사용 이벤트
            }
            else
            {
                //TODO 아이템 사용 못한다고 알리는 이벤트
            }
        }
    }

    
}