using UnityEngine;

[AddComponentMenu(AssetMenu + "/MagicalEggStaticManager")]

public class MagicalEggStaticManager : BaseStaticDataManager<MagicalEggData>
{
    protected override string Label => Constants.LABEL_MagicalEggData;
}
