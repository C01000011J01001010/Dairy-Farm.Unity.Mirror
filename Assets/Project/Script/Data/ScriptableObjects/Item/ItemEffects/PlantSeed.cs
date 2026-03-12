using UnityEngine;

public class PlantSeed : ItemEffectSO
{
    public override void OnAnimEvent(BaseCharacter character, InfoItemContainer item)
    {

    }

    public override void OnUse(BaseCharacter character, InfoItemContainer item)
    {
        CharacterTileChecker tileChecker = character.GetModule<CharacterTileChecker>();
        Crop crop = null;
        // 어찌어찌해서 crop 객체를 가져오고
        crop.transform.position = tileChecker.transform.position;

    }
}
