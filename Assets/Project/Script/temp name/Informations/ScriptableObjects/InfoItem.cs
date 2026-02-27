using UnityEngine;

[CreateAssetMenu(fileName = "InfoItem", menuName = "Items/TestItem")]
public class InfoItem : InfoObject
{
	public ItemType itemType;
	public int maxStack;
	public int sellPrice;
}
