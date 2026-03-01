using UnityEngine;

[RequireComponent(typeof(InfoQuestConditionList))]
public class InfoQuestShower : InfoShower<InfoQuest, InfoQuestContainer>
{
    InfoQuestConditionList conditionShower;
    private void Awake()
    {
        conditionShower = GetComponent<InfoQuestConditionList>();
    }
}
