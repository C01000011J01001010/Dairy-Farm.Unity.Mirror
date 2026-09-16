
using UnityEngine;
using CoreEngine.GameData;
namespace Farm.GameData
{
    //[AddComponentMenu(AssetMenu + "/QuestStaticManager")]
    [AddComponentMenu("QuestStaticManager")]

    public class QuestStaticManager : BaseStaticDataManager<QuestData>
    {
        //protected override string Label => Constants.LABEL_QuestData;

        protected override string CatalogAddress => Constants.LABEL_QuestData;

        protected override void OnLoadedDataBase(ScriptableObject loadedAsset)
        {

        }
    }
}
