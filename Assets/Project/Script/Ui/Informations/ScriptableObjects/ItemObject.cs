using UnityEngine;

public enum ItemType
{
    Consumable, // 소모품
    Equipment,  // 장비
    Material    // 재료
}

[CreateAssetMenu(fileName = "New ItemObject", menuName = "Items/ItemObject")]
public class ItemObject : InfoObject
{
	public ItemType itemType;
	public int maxStack;
	public int sellPrice;
}


