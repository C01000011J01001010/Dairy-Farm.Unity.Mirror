using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace Farm.Fishing
{
    //기본 모듈 참조해서 기존 코드 쌀먹
    public class FishModule : BaseCharacterModule, IActorFeature
    {
        [SerializeField] private Tilemap waterTilemap; //인스펙터에서 씬의 "Water"Tilemap 오브젝트 가져오기

        private Vector2 lastFacingDir = Vector2.down;
        baseCharacterAnim _anim;

        //결과를 밖으로 방송
        public event Action<bool> Event_OnFishingResult;

        public override void Initialize(BaseCharacter owner)
        {
            base.Initialize(owner);
            _anim = owner.GetComponent<baseCharacterAnim>();
        }

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

            if (IsSeaTile())
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
            return waterTilemap.HasTile(frontCell);
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
           // 처음부터 정수로 가져오는 방법

           // 플레이어 현재 칸을 정수로 가져옴
            Vector3Int currentcell = waterTilemap.WorldToCell(Owner.transform.position);
            // 방향의 정수 오프셋으로 변환해 칸 단위로 이동
            Vector3Int direction = new Vector3Int((int)lastFacingDir.x, (int)lastFacingDir.y, 0);

            return currentcell + direction;
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
