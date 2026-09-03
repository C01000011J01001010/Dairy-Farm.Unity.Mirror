using System;
using UnityEngine;
using Farm.Character;

namespace Farm.StaticData.Item
{
    [CreateAssetMenu(fileName = "PlantSeed", menuName = "Item/ItemEffect/PlantSeed")]
    public class PlantSeed : BaseItemEffect
    {
        //[NonSerialized]
        //GlobalPoolManager _poolManager;
        //GlobalPoolManager PoolManager
        //{
        //    get
        //    {
        //        // 씬 전환시 fake null 방지를 위해 (??=) 대신 (== null)을 사용
        //        if (_poolManager == null)
        //        {
        //            _poolManager = GameManager.GetManager<GlobalPoolManager>();

        //            if (_poolManager == null)
        //            {
        //                Debug.LogError("MultiObjectPoolManager를 찾을 수 없음");
        //            }
        //        }
        //        return _poolManager;
        //    }
        //}

        public override void ApplyEffect(BaseCharacter character, ItemDataContainer item)
        {
            CharacterTileChecker tileChecker = null;
            if (!character.TryGetFeature(out tileChecker)) return;

            CharacterCropDataSheet cropDataSheet = null;
            if (!character.TryGetFeature(out cropDataSheet)) return;

            CropViewer cropViewer = GetNewCrop(item);

            int cropId = GetCropIndexFromSeed(item);
            CropContainer cropContainer = cropDataSheet.AcquireCrop(cropId);
            cropContainer.SetCropPosition(tileChecker.GetTilePosition());

            // view에 Model 연결
            cropViewer?.Connect(cropContainer);
        }

        private int GetCropIndexFromSeed(ItemDataContainer item)
        {
            return item.GetIndex() % 1000;
        }

        private CropViewer GetNewCrop(ItemDataContainer item)
        {
            //GameObject newObject = PoolManager.Spawn(GlobalPoolType.Crop);
            //if(newObject.TryGetComponent(out CropViewer asCropViewer))
            //{
            //    newObject.name = item.GetNameTag();
            //    return asCropViewer;
            //}
            //Debug.LogWarning($"{newObject.name}에 CropViewer 컴포넌트 없음");
            return null;
        }
    }
}


