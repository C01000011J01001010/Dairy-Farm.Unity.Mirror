using UnityEngine;

public class MagicalEggDataContainer : BaseDataContainer<MagicalEggData>
{
    [Header("Up to 100%")]
    public int happiness = 0; // 행복도
    public int stress = 0; // 불행도

    public MagicalEggDataContainer(MagicalEggData InitialObject) : base(InitialObject)
    {

    }
}