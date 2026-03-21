using System.Collections.Generic;
using UnityEngine;

class CharacterQuestBook : BaseDatabaseAccessModule<QuestStaticManager, QuestData>
{
    #region 데이터
    // 현재 진행 중인 퀘스트들 (Key: 퀘스트 Index)
    private Dictionary<int, QuestDataContainer> _activeQuests = new();

    // 완료한 퀘스트 횟수 (Key: 퀘스트 Index, Value: 클리어 횟수)
    private Dictionary<int, int> _questClearCounts = new();
    #endregion

    public IReadOnlyDictionary<int, QuestDataContainer> ActiveQuests => _activeQuests;
    public IReadOnlyDictionary<int, int> QuestClearCounts => _questClearCounts;

    private TimeManager timeManager;

    public event System.Action<QuestDataContainer> Event_OnAcceptQuest;     // 퀘스트 수락시
    public event System.Action<QuestDataContainer> Event_OnClearQuest;      // 퀘스트 완료시
    public event System.Action<QuestDataContainer> Event_OnFailQuest;       // 퀘스트 실패시

    public override void Exit()
    {
        base.Exit();
        timeManager.Event_OnGameMinutesPassed -= QuestTimeOutCount;
    }
    public override void Initialize(BaseCharacter owner)
    {
        base.Initialize(owner);
        // 필요하다면 여기서 세이브 데이터를 로드하여 _activeQuests와 _questClearCounts를 복구합니다.
        timeManager = GameManager.GetManager<TimeManager>();
        timeManager.Event_OnGameMinutesPassed += QuestTimeOutCount;
    }

    private void QuestTimeOutCount(int minutes)
    {
        foreach (var quest in _activeQuests.Values)
        {
            // 제한 시간이 있다면
            if (quest.Get().timeLimitMinutes > 0)
            {
                quest.UpdateTime(minutes);
                if (quest.isFailed)
                {
                    // 실패처리된 퀘스트
                    FailQuest(quest.GetIndex());
                    break; // Dictionary 변경 중 오류 방지 (또는 List에 담아 후처리)
                }
            }
        }
    }

    // 퀘스트 수락 가능 여부 체크
    private bool CanAcceptQuest(int questID)
    {
        QuestData data = GetData(questID);
        if (data == null)
        {
            Debug.LogError($"[Quest Index : {questID}] Quest 존재하지 않음");
            return false;
        }

        // 이미 진행 중인지 체크
        if (_activeQuests.ContainsKey(questID))
        {
            Debug.LogWarning($"[Quest Index : {questID}] Quest 이미 진행중");
            return false;
        }

        // 이미 클리어했고, 반복 불가능한 퀘스트인지 체크
        if (_questClearCounts.ContainsKey(questID) && !data.isRepeatable)
        {
            Debug.LogWarning($"[Quest Index : {questID}] Quest 이미 완료됨(반복 불가)");
            return false;
        }

        // 선행 퀘스트 클리어 여부 체크
        if (data.prerequisiteQuestIDs != null)
        {
            bool isSatisfied = true;
            foreach (int preID in data.prerequisiteQuestIDs)
            {
                // 선행 퀘스트를 클리어 하지 않았을때
                if (!_questClearCounts.ContainsKey(preID) || _questClearCounts[preID] == 0)
                {
                    Debug.Log ($"선행퀘스트 [Quest Index : {questID}]가 완료되지 않음");
                    isSatisfied = false;
                }
            }
            if (!isSatisfied) return false;
        }

        return true;
    }

    // 퀘스트 수락
    public bool AcceptQuest(int questID)
    {
        if (!CanAcceptQuest(questID)) return false;

        QuestData data = GetData(questID);
        QuestDataContainer newQuest = new QuestDataContainer(data);

        _activeQuests.Add(questID, newQuest);
        Event_OnAcceptQuest?.Invoke(newQuest);

        return true;
    }

    // 3. 퀘스트 완료 시도 (NPC에게 보고하거나 조건 달성, 완료버튼 이벤트 호출 시)
    public bool TryCompleteQuest(int questID)
    {
        if (!_activeQuests.TryGetValue(questID, out QuestDataContainer questContainer))
            return false;

        // 내부적으로 조건 달성했는지 체크
        questContainer.EvaluateStatus(Owner);

        if (questContainer.isClear)
        {
            // 액티브에서 제거
            _activeQuests.Remove(questID);

            // 클리어 횟수 증가
            if (_questClearCounts.ContainsKey(questID))
                _questClearCounts[questID]++;
            else
                _questClearCounts.Add(questID, 1);

            // TODO: 보상 지급 로직 (questContainer.connectData.baseRewards 등 활용)
            // RewardManager.GiveRewards(questContainer.connectData.baseRewards);

            Event_OnClearQuest?.Invoke(questContainer);
            return true;
        }

        return false;
    }

    // 퀘스트 실패 처리
    public void FailQuest(int questID)
    {
        if (_activeQuests.TryGetValue(questID, out QuestDataContainer questContainer))
        {
            _activeQuests.Remove(questID);
            Event_OnFailQuest?.Invoke(questContainer);
        }
    }

    // 퀘스트 포기 (플레이어가 수동으로 취소)
    public void AbandonQuest(int questID) => FailQuest(questID);
}