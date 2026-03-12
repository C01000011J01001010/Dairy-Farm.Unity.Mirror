using UnityEngine;

[System.Serializable]
public class CropContainer : InfoContainer<CropObject> // CropObject에 목표 성장시간(분)이 있다고 가정
{
    public int currentSpriteIndex;
    public int currentGrowthMinutes; // 현재 성장 시간 (현실 분 단위)
    public float remainderSeconds;   // 오프라인 계산 시 남은 초 단위 찌꺼기

    public int targetGrowthMinutes => throw new System.Exception("미정");
    public Sprite[] cropSprites => currentObject.GetSprites();

    // 다 자랐는지 나타냄
    public bool isFullyGrown { get; private set; }

    // 썩었는지 나타냄
    public bool isRotten { get; private set; }

    public event System.Action Event_OnStateChanged;

    // 외부(CropManager)에서 1분마다, 혹은 수면 시 호출
    public void AddMinutes(int minutes)
    {
        if (isRotten) return; // 이미 썩었으면 시간 계산 정지

        currentGrowthMinutes += minutes;
        CheckState();
    }

    // 오프라인 경과 시간 처리용
    public void AddOfflineTime(int minutes, float seconds)
    {
        if (isRotten) return;

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
        // currentObject(CropData SO)에 targetGrowthMinutes가 있다고 가정합니다.
        int targetTime = targetGrowthMinutes;

        if (currentGrowthMinutes >= targetTime + 72) // 72분(게임 속 3일) 방치 시
        {
            isRotten = true;
            isFullyGrown = false;
        }
        else if (currentGrowthMinutes >= targetTime)
        {
            isFullyGrown = true;
        }
    }

    // 현재 진행도에 따른 스프라이트 인덱스 반환
    public int GetCurrentSpriteIndex()
    {
        if (isRotten)
        {

            return currentObject.GetSprites().Length - 1; // 썩은 이미지 (보통 마지막 배열)
        }

        // 성장 진행도 비율 (0.0 ~ 1.0)
        float progress = Mathf.Clamp01((float)currentGrowthMinutes / targetGrowthMinutes);

        // 썩은 이미지를 제외한 성장 단계 계산
        int maxGrowthStages = currentObject.GetSprites().Length - 2;

        return Mathf.FloorToInt(progress * maxGrowthStages);
    }

    public Sprite GetCurrentSprite()
    {
        if(0<= currentSpriteIndex && currentSpriteIndex < cropSprites.Length)
        {
            return cropSprites[currentSpriteIndex];
        }
        Debug.LogError($"스프라이트 범위 오류 : 최대길이({cropSprites.Length}), 현재Index({currentSpriteIndex})");
        return null;
    }
}