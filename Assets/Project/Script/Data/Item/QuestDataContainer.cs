using System.Collections.Generic;
using UnityEngine;
public class QuestDataContainer : BaseDataContainer<QuestData>
{
    #region 저장할 데이터
    public int remainingTime; // 제한시간 카운팅
    public bool isClear; // 퀘스트 완료 여부
    public bool isAdditionalClear; // 추가보상 완료 여부
    public bool isFailed; // 퀘스트 실패 여부

    // 각 조건들의 현재 진행 상황을 담는 컨테이너 리스트
    public List<QuestConditionContainer> completionProgress = new();
    public List<QuestConditionContainer> additionalProgress = new();
    public List<QuestConditionContainer> failureProgress = new();
    #endregion

    public QuestDataContainer(QuestData InitialObject) : base(InitialObject)
    {
        Set(InitialObject);
        InitializeConditions();
    }

    // 초기화: QuestData의 조건들을 바탕으로 진행도 컨테이너 생성
    private void InitializeConditions()
    {
        remainingTime = connectData.timeLimitMinutes;
        isClear = false;
        isFailed = false;

        // BaseQuestCondition에 맞게 진행도 컨테이너 초기화 (Null 체크 포함)
        if (connectData.completionConditions != null)
        {
            foreach (BaseQuestCondition cond in connectData.completionConditions)
                completionProgress.Add(new QuestConditionContainer(cond));
        }

        if (connectData.additionalConditions != null)
        {
            foreach (BaseQuestCondition cond in connectData.additionalConditions)
                additionalProgress.Add(new QuestConditionContainer(cond));
        }

        if (connectData.failureConditions != null)
        {
            foreach (BaseQuestCondition cond in connectData.failureConditions)
                failureProgress.Add(new QuestConditionContainer(cond));
        }
    }

    // 시간 업데이트 (QuestBook에서 호출해줌)
    public void UpdateTime(int deltaMinutes)
    {
        // 시간 업데이트 할 필요 없는 경우 제외
        if (isClear || isFailed || connectData.timeLimitMinutes <= 0) return;

        // 남은시간 변화량 적용
        remainingTime -= deltaMinutes;

        // 더 이상 남은시간이 없다면
        if (remainingTime <= 0)
        {
            // 남은시간은 0으로 초기화 후 실패 처리
            remainingTime = 0;
            isFailed = true;
        }
    }

    // 조건들이 만족되었는지 체크하는 메서드
    public void EvaluateStatus(BaseCharacter character)
    {
        if (isClear || isFailed) return;

        isFailed = failureProgress.Exists(c => c.IsSatisfied(character));
        if(!isFailed)
        {
            isClear = completionProgress.TrueForAll(c => c.IsSatisfied(character));
            isAdditionalClear = additionalProgress.TrueForAll(c =>  !c.IsSatisfied(character));
        }
    }
}
