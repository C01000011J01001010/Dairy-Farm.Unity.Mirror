using CoreEngine.Actor;
using CoreEngine.Extentions;
using Farm.StaticData.Item;
using System;
using UnityEngine;

namespace Farm.Character
{
    public class CharacterActionController : BaseActorFeature, IDisposable
    {
        // 현재 손에 들고 있는(선택된) 아이템
        private ItemDataContainer currentItem;
        private CharacterInventory inventory;
        private BaseCharacter character;


        public void Dispose()
        {
            inventory.Event_OnSelectedSlotChanged -= EquipItem;
            inventory.Event_OnItemUseInput -= OnItemUseInput;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            character = Host as BaseCharacter;

            if (!Host.TryGetFeature(out inventory)) return;
            inventory.Event_OnSelectedSlotChanged += EquipItem;
            inventory.Event_OnItemUseInput += OnItemUseInput;
        }


        // 마우스 클릭이나 아이템 사용 버튼을 눌렀을 때 호출
        public void OnItemUseInput()
        {
            if (currentItem.IsEmpty()) return;

            BaseItemEffect[] OnUseEffects = currentItem.Get().GetEffect_OnUse();
            if (OnUseEffects == null) return;

            // 아이템이 가진 모든 효과 리스트를 순회하며 OnUse 실행
            foreach (var effect in OnUseEffects)
            {
                effect.ApplyEffect(character, currentItem);
            }
        }

        // 유니티 애니메이션 창에서 Event로 등록할 공통 함수
        public void OnAnimEventTrigger(int effectIndex)
        {
            if (currentItem.IsEmpty()) return;

            BaseItemEffect[] OnAnimeEventEffects = currentItem.Get().GetEffect_OnAnimEvent();
            if (OnAnimeEventEffects == null) return;

            if (OnAnimeEventEffects.TryGet(effectIndex, out BaseItemEffect effect))
            {
                effect.ApplyEffect(character, currentItem);
            }
            else
            {
                Debug.LogError("존재하지 않는 AnimeEvent");
            }
        }

        // 인벤토리에서 아이템을 선택했을 때 호출해줄 함수
        public void EquipItem(int index)
        {
            ItemDataContainer curItem = inventory.GetItem(index);
            currentItem = curItem;
            if (curItem.IsEmpty())
            {
                Debug.LogWarning("비어있는 아이템");
            }
        }
    }
}
