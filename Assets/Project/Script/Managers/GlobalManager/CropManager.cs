using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropManager : BaseGlobalManager, IGlobalManager
{
    // 관리 중인 모든 밭의 작물 데이터 (세이브 파일에서 불러옴)
    public List<CropContainer> activeCrops = new List<CropContainer>();

    private TimeManager timeManager;
    private float _minuteTimer = 0f;

    public void Exit()
    {
        
        //TODO 저장 로직 구현
    }

    public IEnumerator Initialize()
    {
        timeManager = GameManager.GetManager<TimeManager>();
        if(timeManager != null )
        {

        }
        // TODO: 세이브 파일 로드 로직 (lastSaveTime 가져오기)
        DateTime lastSaveTime = DateTime.UtcNow; // 임시: 세이브 데이터에서 가져와야 함
        CalculateOfflineProgress(lastSaveTime);
        yield break;
    }

    private void Update()
    {
        // Time.timeScale=0 (일시정지)에 영향받지 않고 실제 현실 시간 기준 카운팅
        _minuteTimer += Time.unscaledDeltaTime;

        if (_minuteTimer >= 60f) // 현실 1분이 지나면
        {
            _minuteTimer -= 60f;
            PassTimeForCrops(1); // 1분 추가
        }
    }

    // 작물들에게 시간 전달 및 썩은 작물 정리
    public void PassTimeForCrops(int minutesToAdd)
    {
        for (int i = activeCrops.Count - 1; i >= 0; i--)
        {
            CropContainer crop = activeCrops[i];
            crop.AddMinutes(minutesToAdd);

            if (crop.isRotten)
            {
                // 썩음 처리 로직 (예: 밭을 썩은 상태로 변경)
                Debug.Log("작물이 썩었습니다!");
                activeCrops.RemoveAt(i); // 업데이트 리스트에서 제외
            }
        }
    }

    // 플레이어 수면 시 호출 (8분 = 게임 내 8시간 스킵)
    public void Sleep()
    {
        Debug.Log("수면: 작물들에게 8분(게임 8시간)의 시간을 추가합니다.");
        PassTimeForCrops(8);
    }

    // 로딩 시 오프라인 경과 시간 계산
    private void CalculateOfflineProgress(DateTime lastSaveTime)
    {
        // 마지막 저장시점에서 현재 시점까지의 시간차를 계산
        TimeSpan offlineDelta = DateTime.UtcNow - lastSaveTime;
        float totalOfflineSeconds = (float)offlineDelta.TotalSeconds;

        // 시간을 분과 초로 환산
        int offlineMinutes = (int)(totalOfflineSeconds / 60f);
        float leftoverSeconds = totalOfflineSeconds % 60f;

        foreach (var crop in activeCrops)
        {
            crop.AddOfflineTime(offlineMinutes, leftoverSeconds);
        }
    }

    
}