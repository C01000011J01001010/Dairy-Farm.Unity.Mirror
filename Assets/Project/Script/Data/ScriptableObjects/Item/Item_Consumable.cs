public enum ConsumableType
{
    Seed,
}


public class Item_Consumable : BaseItem
{
    public ConsumableType consumableType;
    public float effectValue;

    public override bool TryUse(BaseCharacter user)
    {
        throw new System.NotImplementedException();
    }
}