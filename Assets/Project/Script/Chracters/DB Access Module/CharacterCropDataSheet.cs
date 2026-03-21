using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterCropDataSheet : BaseDatabaseAccessModule<CropStaticManager, CropData>
{
    #region 저장할 데이터
    private Dictionary<int/*농작물 index*/, int /*농작물 채집횟수*/> _cropHarvestCounts;
    #endregion
    TimeManager timeManager;
    private List<CropContainer> activeCropList;

    public Dictionary<int/*농작물 index*/, int /*농작물 채집횟수*/> CropHarvestCounts => _cropHarvestCounts;

    public override void Exit()
    {
#if UNITY_EDITOR
        timeManager.Event_OnGameMinutesPassed -= PassTimeForCrops;
#else
        timeManager.Event_OnGameHoursPassed -= PassTimeForCrops;
#endif

        base.Exit();
    }
    public override void Initialize(BaseCharacter owner)
    {
        base.Initialize(owner);
        timeManager = GameManager.GetManager<TimeManager>();
        activeCropList = new();

        // TODO 세이브 데이터 로드 추가
    }

    public override void PostInitialize()
    {
#if UNITY_EDITOR
        // 빠른 테스트를 위해 사용
        timeManager.Event_OnGameMinutesPassed += PassTimeForCrops;
#else
        timeManager.Event_OnGameHoursPassed += PassTimeForCrops;
#endif
    }

    public CropContainer AcquireCrop(int id)
    {
        // 농작물 정적데이터를 감싸는 새로운 컨테이너 만들기
        CropData newCrop = GetData(id);
        CropContainer container = new();

        // 컨테이너에 데이터 연결
        container.Set(newCrop);

        // 연결된 컨테이너를 List에 삽임
        activeCropList.Add(container);
        return container;
    }

    /// <summary>
    /// 작물들에게 시간 전달 및 썩은 작물 정리
    /// </summary>
    /// <param name="realMinutesToAdd">현실의 1분 : 게임속 1시간</param>
    public void PassTimeForCrops(int realMinutesToAdd)
    {
        // 제거 될 수 있으니 뒤에부터
        for (int i = activeCropList.Count - 1; i >= 0; i--)
        {
            CropContainer crop = activeCropList[i];
            crop.AddMinutes(realMinutesToAdd);

            if (crop.IsRotten)
            {
                // 썩음 처리 로직 (예: 밭을 썩은 상태로 변경)
                Debug.Log("작물이 썩었습니다!");
                activeCropList.RemoveAt(i); // 업데이트 리스트에서 제외
            }
        }
    }

    // 플레이어 수면 시 호출 (8분 = 게임 내 8시간 스킵)
    public void OnSleep()
    {
        Debug.Log("수면: 작물들에게 8분(게임 8시간)의 시간을 추가합니다.");
        PassTimeForCrops(8);
    }

    // 로딩 시 오프라인 경과 시간 계산
    private void CalculateOfflineProgress(DateTime lastSaveTime)
    {
        TimeSpan offlineDelta = DateTime.UtcNow - lastSaveTime;
        float totalOfflineSeconds = (float)offlineDelta.TotalSeconds;

        int offlineMinutes = (int)(totalOfflineSeconds / 60f);
        float leftoverSeconds = totalOfflineSeconds % 60f;

        foreach (var crop in activeCropList)
        {
            crop.AddOfflineTime(offlineMinutes, leftoverSeconds);
        }
    }
}
