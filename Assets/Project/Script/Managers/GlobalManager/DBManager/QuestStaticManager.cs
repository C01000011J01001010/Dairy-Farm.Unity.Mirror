
using UnityEngine;

[AddComponentMenu(AssetMenu + "/QuestStaticManager")]

public class QuestStaticManager : BaseStaticDataManager<QuestData>
{
    protected override string Label => Constants.LABEL_QuestData;
}