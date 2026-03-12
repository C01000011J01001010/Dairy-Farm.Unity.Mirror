using System.Collections;
using UnityEngine;

/// <summary>
/// 게임에 등장하는 농작물 객체에 붙일 스크립트
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Crop : MonoBehaviour, IInitialize
{
    private CropContainer cropContainer;
    private SpriteRenderer _spriteRenderer;
    private static readonly Color32 rottenColor = new Color32(130, 70, 20, 255);

    // 농작물 제거할때 / Pool로 돌아갈 때 실행
    public void Exit()
    {
        CleanUp();
    }

    // 농작물 생성할때 / Pool에서 꺼내올 때 실행
    public IEnumerator Initialize()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        yield break;
    }

    public void CleanUp()
    {
        if(cropContainer != null)
        {
            cropContainer.Event_OnStateChanged -= UpdateView;
        }
        cropContainer = null;
    }

    public void SetData(CropContainer newCrop)
    {
        cropContainer = newCrop;
        if(cropContainer != null)
        {
            // 렌더러 색상 정상화 (썩은색 변경상태 예방)
            _spriteRenderer.color = Color.white;

            // view업데이트를 이벤트로 등록 후 실행
            cropContainer.Event_OnStateChanged += UpdateView;
            UpdateView();
        }
    }

    private void UpdateView()
    {
        // 1. 썩은 상태인지 확인
        if (cropContainer.isRotten)
        {
            // 썩은 상태면 마지막 스프라이트를 유지한 채 색상을 변경
            _spriteRenderer.color = rottenColor;
        }
        else
        {
            // 다음 단계 스프라이트로 이동
            _spriteRenderer.sprite = cropContainer.GetCurrentSprite();
        }
    }

    // TileChecker가 이 작물을 클릭/선택했을 때 데이터를 가져가기 위한 인터페이스
    public CropContainer GetCropData()
    {
        return cropContainer;
    }

    // 수확 인터랙션
    public void Harvest()
    {
        if (cropContainer.isRotten)
        {
            Debug.Log("썩은 작물을 치웠습니다.");
            // TODO 매니저 객체의 리스트에서 제거 후, 현재 객체, 컨테이너 객체 제거
        }
        else if (cropContainer.isFullyGrown)
        {
            Debug.Log("작물을 수확했습니다!");
            // TODO: 인벤토리에 아이템 추가
        }

        // TODO 매니저 객체의 리스트에서 제거 후, 컨테이너 객체 제거
        Destroy(gameObject);
    }

    
}