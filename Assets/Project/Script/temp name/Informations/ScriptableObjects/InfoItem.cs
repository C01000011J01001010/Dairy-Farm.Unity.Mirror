using UnityEngine;

public enum ItemType
{
	Material, Edible, Usable, Equip, Sell
}

[CreateAssetMenu(fileName = "InfoItem", menuName = "Items/TestItem")]
public class InfoItem : InfoObject
{
	public ItemType itemType;
	public int maxStack;
	public int sellPrice;
}
