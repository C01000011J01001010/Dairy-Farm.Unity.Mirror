using Farm.GameData.Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreEngine.GameData;

namespace Farm.GameData
{
    /// <summary>
    /// 아이템의 정적 데이터 관리
    /// </summary>
    //[AddComponentMenu(AssetMenu + "/ItemStaticManager")]
    [AddComponentMenu("ItemStaticManager")]
    public class ItemStaticManager : BaseStaticDataManager<ItemData>
    {
        //protected override string Label => Constants.LABEL_ItemData;

        protected override string CatalogAddress => throw new System.NotImplementedException();

        protected override void OnLoadedDataBase(ScriptableObject loadedAsset)
        {
            throw new System.NotImplementedException();
        }
    }
}
