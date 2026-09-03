using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantSeed", menuName = "Item/ItemEffect/PlantSeed")]
public class PlantSeed : BaseItemEffect
{
    [NonSerialized]
    MultiObjectPoolManager _poolManager;
    MultiObjectPoolManager PoolManager
    {
        get
        {
            // 씬 전환시 fake null 방지를 위해 (??=) 대신 (== null)을 사용
            if (_poolManager == null)
            {
                _poolManager = WorldManager.GetManager<MultiObjectPoolManager>();

                if (_poolManager == null)
                {
                    Debug.LogError("MultiObjectPoolManager를 찾을 수 없음");
                }
            }
            return _poolManager;
        }
    }

    public override void ApplyEffect(BaseCharacter character, ItemDataContainer item)
    {
        CharacterTileChecker tileChecker = character.GetFeature<CharacterTileChecker>();
        CharacterCropDataSheet cropDataSheet = character.GetFeature<CharacterCropDataSheet>();

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
        GameObject newObject = PoolManager.Spawn(PoolType.Crop);
        if(newObject.TryGetComponent(out CropViewer asCropViewer))
        {
            newObject.name = item.GetNameTag();
            return asCropViewer;
        }
        Debug.LogWarning($"{newObject.name}에 CropViewer 컴포넌트 없음");
        return null;
    }
}
