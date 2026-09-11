using UnityEngine;
using CoreEngine.Manager;
namespace Farm.StaticData
{
    //[AddComponentMenu(AssetMenu + "/MagicalEggStaticManager")]
    [AddComponentMenu("MagicalEggStaticManager")]

    public class MagicalEggStaticManager : BaseStaticDataManager<MagicalEggData>
    {
        //protected override string Label => Constants.LABEL_MagicalEggData;

        protected override string CatalogAddress => throw new System.NotImplementedException();

        protected override void OnLoadedDataBase(ScriptableObject loadedAsset)
        {
            throw new System.NotImplementedException();
        }
    }

}
