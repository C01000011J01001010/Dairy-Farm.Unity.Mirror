using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

namespace Farm.Fishing
{
    public class FishModule : BaseCharacterModule, IActorFeature
    {
        private const int FISHING_ROD_INDEX = 4; // 낚싯대 아이템 index

        //낚시대를 들고있는지 확인하기위해
        private CharacterInventory inventory;
        //입력
        private UserInputManager inputManager;
        //지금 낚시대를 장착중인가
        private bool isFishingRodEquipped;

        //낚싯대 릴 개수 (기본, 하, 중, 상)
        public int reelTier { get; private set; } = 0;

        //캐릭터가 모듈에 뭍었을때 한번 실행 컴포넌트 참조 연결, 이벤트 구독
        public override void Initialize(BaseCharacter owner)
        {
            base.Initialize(owner);
            _anim = owner.GetComponent<baseCharacterAnim>();

            inventory = owner.GetFeature<CharacterInventory>();
            inventory.Event_OnSelectedSlotChanged += OnSelectedSlotChanged;

            inputManager = GameManager.GetManager<UserInputManager>();
            inputManager.Event_OnSwitchReelInput += CycleReelTier;

            //타일맵 캐싱
            upperLayers = terrainGrid.GetComponentsInChildren<Tilemap>();
        }

        //캐릭터가 사라질때 실행
        public override void Exit()
        {
            inventory.Event_OnSelectedSlotChanged -= OnSelectedSlotChanged;
            inputManager.Event_OnSwitchReelInput -= CycleReelTier;
        }

        //===낚싯대 장비 여부===

        //지금 든게 낚시대인지 아닌지 판단하기 위한 함수
        private void OnSelectedSlotChanged(int index)
        {
            ItemDataContainer item = inventory.GetItem(index);
            isFishingRodEquipped = !item.IsEmpty() && item.Get().Index == FISHING_ROD_INDEX;
        }

        //낚시대를 들고있을때만 릴 단계를 변경할수있게
        public void CycleReelTier()
        {
            if (!isFishingRodEquipped) return;

            reelTier = (reelTier + 1) % 4;
            Debug.Log($"릴 단계: {reelTier}");
        }

        // === 물고기 획득 로직 ===

        //현재 reelTier 기준으로 기본에서 해당단계 테이블을 합쳐 가중치로 비례하여 물고기 랜덤획득
        public FishData RollFish()
        {
            List<FishData> pool = new List<FishData>();
            for (int i = 0; i <= reelTier; i++)
                pool.AddRange(table.GetTier(i));

            float totalWeight = 0f;
            foreach (FishData fish in pool) totalWeight += fish.weight;

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0f;
            foreach (FishData fish in pool)
            {
                cumulative += fish.weight;
                if (roll <= cumulative) return fish;
            }

            return pool[pool.Count - 1];

        }

        //=== 방향추적 ===

        [SerializeField] private Tilemap waterTilemap; //인스펙터에서 씬의 "Water"Tilemap 오브젝트 가져오기
        [SerializeField] private Grid terrainGrid; //인스펙터에서 위쪽 레이어부터 순서대로 넣기
        [SerializeField] private FishTable table;

        private Tilemap[] upperLayers;

        private Vector2 lastFacingDir = Vector2.down;
        baseCharacterAnim _anim;

        //결과를 밖으로 방송
        public event Action<bool> Event_OnFishingResult;

        //플레이어가 바라보는 방향 갱신
        public override void Tick(float deltaTime)
        {
            if (Owner.isMove)
            {
                lastFacingDir = SnapToCardinal(Owner.inputMove);
            }
        }

        //정면 바다 판정
        public void TryFish()
        {
            bool isFishing = IsSeaTile();

            //결과를 이벤트로 방송
            Event_OnFishingResult?.Invoke(isFishing);

            if (isFishing)
            {
                Debug.Log("낚시중");
                _anim.SetIsFishing(true);
            }

            else
            {
                Debug.Log("낚시할 곳이 없습니다.");
            }

        }

        public bool IsSeaTile()
        {
            Vector3Int frontCell = GetFrontCell();

            if (!waterTilemap.HasTile(frontCell))
            {
                return false;
            }

            foreach (Tilemap layer in upperLayers)
            {
                if (layer == waterTilemap)
                {
                    continue;
                }
                if ((layer.HasTile(frontCell)))
                {
                    return false;
                }
            }
            return true;
        }

        //캐릭터 1칸 위치값 실수에서 정수로 변환후 반환 WorldToCell 이용
        private Vector3Int GetFrontCell()
        {
            // 캐릭터 위치에서 바라보는 방향으로 1칸 만큼 이동한 월드좌표를 구함
            Vector3 frontWorldPos = Owner.transform.position + (Vector3)lastFacingDir;
            //2d라서 z 값 고정
            frontWorldPos.z = 0f;
            // 정수로 변환후 반환
            return waterTilemap.WorldToCell(frontWorldPos);
        }

        //플레이어가 움직일때 정면이 어느방향인지 체크
        private Vector2 SnapToCardinal(Vector2 dir)
        {
            //상하좌우 정리
            return Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
                ? new Vector2(Mathf.Sign(dir.x), 0)
                : new Vector2(0, Mathf.Sign(dir.y));
        }


    }
}