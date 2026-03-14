using UnityEngine;

[CreateAssetMenu(fileName = "PlantSeed", menuName = "Item/ItemEffect/PlantSeed")]
public class PlantSeed : BaseItemEffect
{
    // TODO : Prefab을 들고있지 않고 Pool에서 받아오기
    [SerializeField] CropViewer prefab;
    public override void ApplyEffect(BaseCharacter character, ItemDataContainer item)
    {
        CharacterTileChecker tileChecker = character.GetModule<CharacterTileChecker>();
        CharacterCropDataSheet cropDataSheet = character.GetModule<CharacterCropDataSheet>();

        // TODO : Pool에서 받아오기
        CropViewer cropViewer = GetNewCrop(item);

        int cropId = GetCropIndexFromSeed(item);
        CropContainer cropContainer = cropDataSheet.AcquireCrop(cropId);
        cropContainer.SetCropPosition(tileChecker.GetTilePosition());

        // view에 Model 연결
        cropViewer.Connect(cropContainer);
    }

    private int GetCropIndexFromSeed(ItemDataContainer item)
    {
        return item.GetIndex() % 1000;
    }

    private CropViewer GetNewCrop(ItemDataContainer item)
    {
        GameObject newObject = Instantiate(prefab.gameObject);
        if(newObject.TryGetComponent(out CropViewer asCropViewer))
        {
            asCropViewer.gameObject.name = item.GetNameTag();
            asCropViewer.Initialize();
            return asCropViewer;
        }
        return null;
    }
}
