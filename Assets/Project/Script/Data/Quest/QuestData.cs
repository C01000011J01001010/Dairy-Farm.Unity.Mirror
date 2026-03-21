using System.Collections.Generic;
using UnityEngine;



public enum QuestType
{
    Main,  // 필수 퀘스트
    Major, // 권장 퀘스트
    Sub, // 플레이어 선택
}


public class QuestData : BaseData_ForUi
{
    [Header("Quest Identity")]
    public QuestType questType;

    [Header("Requirements (수락 조건)")]
    public List<int> prerequisiteQuestIDs; // 선행 퀘스트 ID 목록

    [Header("Progress (진행 및 완료)")]
    public List<BaseQuestCondition> completionConditions;   // 완료 조건 (몬스터 처치, 아이템 수집 등)
    public List<BaseQuestCondition> additionalConditions;   // 추가보상 조건
    public List<BaseQuestCondition> failureConditions;    // 실패 조건 (특정 NPC 사망 등)

    [Header("Rewards (보상)")]
    public List<BaseQuestReward> baseRewards;           // 확정 보상
    public List<BaseQuestReward> additionalRewards;     // 추가조건 만족시 보상
    public List<BaseQuestReward> optionalRewards; // 선택 보상

    [Header("World")]
    public int MapId = -1;   // Map 위치 (MapId < 0 이면 미니맵 표시 x)
    public Vector2Int questLocation;        // 미니맵 마커 표시용 목적지 좌표 (TileManager와 연동)
    public float questLocationRange;        // 미니맵 마커 기준 퀘스트 수행 지역 범위 (원의 반지름)

    [Header("System")]
    public bool isRepeatable;           // 반복 가능 여부
    // 게임속 제한 시간(게임 속 1분 -> 현실 1초)
    public int timeLimitMinutes = -1;   //  (time < 0이면 무제한) (time > 0 이면 Container에서 카운팅)
}
