using CoreEngine.Data;
using Farm.Character;

namespace Farm.StaticData.Quest
{
    public class QuestConditionContainer : BaseObjectContainer<BaseQuestCondition>
    {
        public QuestConditionContainer(BaseQuestCondition questCondition) : base(questCondition)
        { }

        public bool IsSatisfied(BaseCharacter character)
        {
            bool isSatisfied = connectData.IsSatisfied(character);
            return isSatisfied;
        }
    }
}
