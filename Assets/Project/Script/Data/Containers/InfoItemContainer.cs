using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class InfoItemContainer : InfoContainer<ItemObject>
{
    public int amount;      // 아이템 개수
    public int durability; // 아이템 내구도(사용 횟수 제한)


    // 새로운 아이템을 얻었을 때
    public event Action OnAmountChanged;
    public event Action OnDurabilityChanged;
    public event Action OnClear; // ui 템칸 비우라고 알림

    public int maxStack => currentObject.maxStack;
    public int maxDurability => currentObject.maxDurability;

    public InfoItemContainer() : base()
    {
        currentObject = null;
    }

    public InfoItemContainer(ItemObject InitialObject) : base(InitialObject)
    {
        Set(InitialObject);
    }

    public virtual void Push(int pushAmount, out int remain)
    {
        if (pushAmount < 1)
        {
            Debug.LogError("pushAmount 개수가 음수이거나 0임");
            remain = 0;
            return;
        }
        AddAndGetExcess(ref amount, pushAmount, maxStack, out remain);

        // ui에 변화 알림
        OnAmountChanged?.Invoke();
    }

    public virtual void Pop(int popAmount, out int remain)
    {
        if (popAmount < 1)
        {
            Debug.LogError("popAmount 개수가 음수이거나 0임");
            remain = 0;
            return;
        }

        amount -= popAmount;
        remain = 0;

        // 아이템 추가 후 최대치보다 많이 갖게 되면 나머지는 뱉어냄
        if(amount < 0)
        {
            remain = amount * -1;
            amount = 0;
        }
        if(amount == 0)
        {
            Clear();
        }

        // ui에 변화 알림
        OnAmountChanged?.Invoke();
    }

    public bool TryReduceDurability(int count = 1)
    {
        if (durability - count < 0) return false;
        durability -= count;
        OnDurabilityChanged?.Invoke();
        return true;
    }

    public void RefillDurability()
    {
        durability = maxDurability;
        OnDurabilityChanged?.Invoke();
    }

    public void RefillDurability(int fillAmount, out int remain)
    {
        if (fillAmount < 1)
        {
            Debug.LogError("fillAmount 개수가 음수이거나 0임");
            remain = 0;
            return;
        }
        AddAndGetExcess(ref durability, fillAmount, maxDurability, out remain);

        // ui에 변화 알림
        OnDurabilityChanged?.Invoke();
    }

    private void AddAndGetExcess(ref int cur, int plus, int max, out int remain)
    {
        cur += plus;
        remain = 0;

        // 최대치보다 많이 갖게 되면 나머지는 뱉어냄
        if (cur > max)
        {
            remain = cur - max;
            cur = max;
        }
    }

    // 슬롯이 비어있는지 확인하는 헬퍼 함수
    public bool IsEmpty() => currentObject == null || amount <= 0;

    // 슬롯 초기화 (아이템을 다 썼을 때)
    public void Clear()
    {
        currentObject = null;
        amount = 0;
        OnClear?.Invoke();
    }
}
