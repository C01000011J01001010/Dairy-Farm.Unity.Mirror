using System.Collections;
using UnityEngine;

/// <summary>
/// 게임에 등장하는 농작물 객체에 붙일 스크립트
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class CropViewer : BaseDataViewer<CropContainer, CropData>
{
    private SpriteRenderer spriteRenderer;
    private static readonly Color32 rottenColor = new Color32(130, 70, 20, 255);

    protected override void OnDisconnected(CropContainer target)
    {
        if (ConnectObject != null)
        {
            ConnectObject.Event_OnStateChanged -= UpdateView;
        }
        base.OnDisconnected(target);
    }

    // 농작물 처음 생성할때 / Pool에서 처음 만들 때 실행
    public void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Initialize된 객체가 CropContainer에 연결될 때 Connet에서 실행
    protected override void OnConnected()
    {
        // 렌더러 색상 정상화 (썩은색 변경상태 예방)
        spriteRenderer.color = Color.white;

        // view업데이트를 이벤트에 추가
        ConnectObject.Event_OnStateChanged += UpdateView;

        SetPosition(ConnectObject.CropPos);
    }

    public override void UpdateView()
    {
        if (IsEmpty()) return;

        // 스프라이트 초기화
        spriteRenderer.sprite = ConnectObject.GetCurrentSprite();

        // 썩은 상태인지 확인
        if (ConnectObject.IsRotten)
        {
            // 썩은 상태면 마지막 스프라이트를 유지한 채 색상을 변경
            spriteRenderer.color = rottenColor;
        }
    }

    // TileChecker가 이 작물을 클릭/선택했을 때 데이터를 가져가기 위한 인터페이스
    public CropContainer GetCropData()
    {
        return ConnectObject;
    }

    // 수확 인터랙션
    public void Harvest()
    {
        if (ConnectObject.IsRotten)
        {
            Debug.Log("썩은 작물을 치웠습니다.");
            // TODO 매니저 객체의 리스트에서 제거 후, 현재 객체, 컨테이너 객체 제거
        }
        else if (ConnectObject.IsFullyGrown)
        {
            Debug.Log("작물을 수확했습니다!");
            // TODO: 인벤토리에 아이템 추가
        }

        // TODO 매니저 객체의 리스트에서 제거 후, 컨테이너 객체 제거
        Destroy(gameObject);
    }

    public void SetPosition(Vector3 pos)
    {
        pos.z = 0;
        transform.position = pos;
    }
}