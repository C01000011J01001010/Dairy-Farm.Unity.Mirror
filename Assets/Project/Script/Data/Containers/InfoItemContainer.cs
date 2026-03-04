using System;
using UnityEngine;

[Serializable]
public class InfoItemContainer : InfoContainer<BaseItem>
{
    public int amount;      // 아이템 개수
    public int durability; // 아이템 내구도(장비에 사용 -> 사용 횟수 제한)


    // 새로운 아이템을 얻었을 때
    public event Action OnAmountChanged;
    public event Action OnDurabilityChanged;
    public event Action OnClear; // ui 템칸 비우라고 알림

    // 슬롯이 비어있으면 0 반환
    public int maxStack => currentObject?.maxStack ?? 0;

    // 장비가 아니거나 슬롯이 비어있으면 0 반환
    public int maxDurability => (currentObject as Item_Equipment)?.maxDurability ?? 0;

    public InfoItemContainer() : base()
    {
        currentObject = null;
    }

    public InfoItemContainer(BaseItem InitialObject) : base(InitialObject)
    {
        Set(InitialObject);
    }

    public virtual bool TryUse(BaseCharacter user,int count)
    {
        
        if (currentObject is Item_Equipment)
        {
            // 장비라면 count만큼 내구도 감소
            if (durability < count) return false;
        }
        else
        {
            // 그 외 아이템이 사용된 경우
            if (amount < count) return false;
        }

        return currentObject.TryUse(user);
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

        if(currentObject is Item_Equipment)
        {
            durability = maxDurability;
        }

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

    public void ReduceDurability(int count = 1)
    {
        //if(currentObject is not Item_Equipment)
        //{
        //    Debug.LogError("장비가 아닌 객체의 내구도 변화 시도!");
        //    return false;
        //}
        //if (durability - count < 0) return false;
        durability -= count;
        OnDurabilityChanged?.Invoke();
        //return true;
    }

    public void RefillDurability()
    {
        if (currentObject is not Item_Equipment)
        {
            Debug.LogError("장비가 아닌 객체의 내구도 변화 시도!");
            return;
        }
        durability = maxDurability;
        OnDurabilityChanged?.Invoke();
    }

    public void RefillDurability(int fillAmount, out int remain)
    {
        remain = 0;
        if (currentObject is not Item_Equipment)
        {
            Debug.LogError("장비가 아닌 객체의 내구도 변화 시도!");
            return;
        }
        if (fillAmount < 1)
        {
            Debug.LogError("fillAmount 개수가 음수이거나 0임");
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

    public override BaseItem Set(BaseItem newObject)
    {
        base.Set(newObject); // 부모 클래스의 기본 Set 기능(currentObject 할당) 실행

        if (newObject == null)
        {
            Clear();
            return null;
        }

        // 아이템 종류에 따라 초기값 셋팅
        if (newObject is Item_Equipment eq)
        {
            durability = eq.maxDurability; // 획득 시 내구도 꽉 채우기
        }
        else
        {
            // 소모품/재료는 내구도 0
            // amount는 Push나 Pop에서 알아서 관리할 테니 놔둠
            durability = 0;
        }

        return currentObject;
    }
}
