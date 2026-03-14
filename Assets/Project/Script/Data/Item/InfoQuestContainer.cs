using System.Collections.Generic;
using UnityEngine;
public class InfoQuestConditionContainer : BaseObjectContainer<QuestTrigger>
{
    int currentCount;

    public InfoQuestConditionContainer() { }

    public InfoQuestConditionContainer(QuestTrigger InitialObject) : base(InitialObject) { }
}



public class InfoQuestContainer : BaseDataContainer<InfoQuest>
{
    List<InfoQuestConditionContainer> conditions = new();

    public InfoQuestContainer() : base() { }
    public InfoQuestContainer(InfoQuest InitialObject) : base(InitialObject) 
    { 
        Set(InitialObject);
        QuestSetting(InitialObject);
    }

    public void QuestSetting(InfoQuest newQuest)
    {
        conditions.Clear();
        foreach(QuestTrigger trigger in newQuest.successTrigger)
        {
            InfoQuestConditionContainer currentContainer = new(trigger);
            conditions.Add(currentContainer);
        }
    }

}
