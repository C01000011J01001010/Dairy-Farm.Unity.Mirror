using System;
using UnityEngine;


public class QuestShower : InfoListShower<InfoQuest, InfoQuestContainer, InfoQuestShower>
{
    [SerializeField] InfoQuest[] containers;

    private void Start()
    {
        foreach(var container in containers)
        {
            AddUnique(new InfoQuestContainer(container));
        }
    }
}
