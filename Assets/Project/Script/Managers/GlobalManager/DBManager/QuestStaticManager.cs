
using UnityEngine;

using CoreEngine.Manager;
namespace Farm.StaticData
{
    //[AddComponentMenu(AssetMenu + "/QuestStaticManager")]
    [AddComponentMenu("QuestStaticManager")]

    public class QuestStaticManager : BaseStaticDataManager<QuestData>
    {
        //protected override string Label => Constants.LABEL_QuestData;

        protected override string CatalogAddress => throw new System.NotImplementedException();

        protected override void OnLoadedDataBase(ScriptableObject loadedAsset)
        {
            throw new System.NotImplementedException();
        }
    }
}
