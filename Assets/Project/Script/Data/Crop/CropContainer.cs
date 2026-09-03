using UnityEngine;
using static Constants;
using CoreEngine.Data;

[System.Serializable]
public class CropContainer : BaseDataContainer<CropData> // CropObject에 목표 성장시간(분)이 있다고 가정
{
    #region 저장할 데이터
    private int curGrowthLevel; // 0부터 시작
    private int curSpriteIndex;
    private int curGrowthMinutes; // 현재 성장 시간 (현실 분 단위)
    private bool isFullyGrown; // 다 자랐는지 나타냄
    private bool isRotten; // 썩었는지 나타냄
    private Vector3 pos; // Crop 객체가 위치할 좌표
    #endregion

    #region Set에서 세팅할 데이터
    public int targetGrowthMinutes; // 최종 성장까지 필요 시간
    public int targetGrowthLevel; // 최종 성장 단계
    public int intervalMinutes; // 성장 사이 필요한 시간(분)
    public Sprite[] cropSprites;
    #endregion

    public float remainderSeconds;   // 오프라인 계산 시 남은 초 단위 찌꺼기


    public bool IsFullyGrown => isFullyGrown;
    
    public bool IsRotten => isRotten;

    public Vector3 CropPos => pos; // 게임화면에 농작물을 위치시킬 좌표

    public event System.Action Event_OnStateChanged; // crop 객체(view) 에서 등록

    public override CropData Set(CropData newObject)
    {
        base.Set(newObject);

        targetGrowthMinutes = connectData.days_ToMaturity * 24;
        cropSprites = connectData.GetSprites();
        targetGrowthLevel = cropSprites.Length - 1;
        intervalMinutes = targetGrowthMinutes / cropSprites.Length;
        return connectData;
    }

    public void SetCropPosition(Vector3 newPosition) => pos = newPosition;

    // 외부(CropManager)에서 1분마다, 혹은 수면 시 호출
    public void AddMinutes(int minutes)
    {
        // 이미 썩었으면 시간 계산 정지
        //if (isRotten) return; // 썩었으면 cropDataBaseSheet에서 이미 제거됨
        curGrowthMinutes += minutes;
        CheckState();
    }

    // 오프라인 경과 시간 처리용
    public void AddOfflineTime(int minutes, float seconds)
    {
        if (IsRotten) return;

        remainderSeconds += seconds;
        // 남은 초가 60초(1분)를 넘어가면 분으로 치환
        if (remainderSeconds >= 60f)
        {
            minutes += (int)(remainderSeconds / 60f);
            remainderSeconds %= 60f;
        }

        AddMinutes(minutes);
    }

    // 현재 상태 갱신 (성장 및 썩음 판별)
    private void CheckState()
    {
        bool stateChanged = false; // 이벤트 호출을 한 번만 하기 위한 플래그

        // 아직 덜 자랐다면 성장 처리
        if (!isFullyGrown && !isRotten)
        {
            // 오프라인 스킵 시 타겟 레벨을 초과하지 않도록 Min으로 클램핑
            curGrowthLevel = Mathf.Min(curGrowthMinutes / intervalMinutes, targetGrowthLevel);

            if (curGrowthLevel > curSpriteIndex)
            {
                curSpriteIndex = curGrowthLevel;

                if (curGrowthLevel == targetGrowthLevel)
                {
                    isFullyGrown = true;
                }
                stateChanged = true;
            }
        }

        // 다 자랐다면 썩음 처리
        if (isFullyGrown && !isRotten)
        {
            if (curGrowthMinutes >= targetGrowthMinutes + 72) // 72분(게임 속 3일) 방치 시
            {
                isRotten = true;
                isFullyGrown = false;
                stateChanged = true;
            }
        }

        // 상태에 변화가 생겼고, 구독자(view)가 있다면 알림
        if (stateChanged)
        {
            Event_OnStateChanged?.Invoke();
        }
    }

    public Sprite GetCurrentSprite()
    {
        if(0<= curSpriteIndex && curSpriteIndex < cropSprites.Length)
        {
            return cropSprites[curSpriteIndex];
        }
        Debug.LogError($"스프라이트 범위 오류 : 최대길이({cropSprites.Length}), 현재Index({curSpriteIndex})");
        return null;
    }
}