using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CoreEngine.GameData;
namespace Farm.GameData
{
    /// <summary>
    /// 농작물의 정적 데이터 관리
    /// </summary>
    //[AddComponentMenu(AssetMenu + "/CropStaticManager")]
    [AddComponentMenu("CropStaticManager")]
    public class CropStaticManager : BaseStaticDataManager<CropData>
    {
        //protected override string Label => Constants.LABEL_CropData;

        protected override string CatalogAddress => Constants.LABEL_CropData;

        protected override void OnLoadedDataBase(ScriptableObject loadedAsset)
        {

        }
    }
}
