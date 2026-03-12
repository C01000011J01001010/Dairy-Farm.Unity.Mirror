using System;
using UnityEngine;

public class CharacterActionController : MonoBehaviour, ICharacterModule
{
    // 현재 손에 들고 있는(선택된) 아이템
    private InfoItemContainer currentItem;

    public event Action<bool> OnSetActive;

    public BaseCharacter Owner { get; private set; }

    public bool IsActive {  get; private set; }

    // 1. 마우스 클릭이나 아이템 사용 버튼을 눌렀을 때 호출
    public void OnItemUseInput()
    {
        if (currentItem == null || currentItem.Get().effects == null) return;

        // 아이템이 가진 모든 효과 리스트를 순회하며 OnUse 실행
        foreach (var effect in currentItem.Get().effects)
        {
            effect.OnUse(Owner, currentItem);
        }
    }

    // 2. 유니티 애니메이션 창에서 Event로 등록할 공통 함수
    public void OnAnimEventTrigger()
    {
        if (currentItem == null || currentItem.Get().effects == null) return;

        // 아이템이 가진 모든 효과 리스트를 순회하며 OnAnimEvent 실행
        foreach (var effect in currentItem.Get().effects)
        {
            effect.OnAnimEvent(Owner, currentItem);
        }
    }

    //--------------------------------- Module

    // (참고용) 인벤토리에서 아이템을 선택했을 때 호출해줄 함수
    public void EquipItem(InfoItemContainer itemToEquip)
    {
        currentItem = itemToEquip;
    }

    public void SetActive(bool active)
    {
        IsActive = active;
        OnSetActive?.Invoke(active);
    }

    public void Exit()
    {

    }

    public void Initialize(BaseCharacter owner)
    {

    }

    public void PostInitialize()
    {

    }

    public void Tick(float deltaTime)
    {

    }

    public void FixedTick(float fixedDeltaTime)
    {

    }

    
}