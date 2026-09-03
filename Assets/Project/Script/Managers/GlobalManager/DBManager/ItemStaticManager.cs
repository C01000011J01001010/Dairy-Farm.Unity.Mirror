using Farm.StaticData.Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Manager.StaticData
{
    /// <summary>
    /// 아이템의 정적 데이터 관리
    /// </summary>
    [AddComponentMenu(AssetMenu + "/ItemStaticManager")]
    public class ItemStaticManager : BaseStaticDataManager<ItemData>
    {
        protected override string Label => Constants.LABEL_ItemData;
    }
}
