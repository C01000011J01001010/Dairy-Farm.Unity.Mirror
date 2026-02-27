using UnityEngine;

// 에디터에서 우클릭으로 생성할 수 있도록 메뉴 추가
[CreateAssetMenu(fileName = "NewItem", menuName = "GameData/ItemData")]
public class ItemData : ScriptableObject, IPrimaryKey
{
    public int ID;              // 고유 ID (CSV의 Primary Key 역할)
    public string Name;         // 아이템 이름 (예: "Aries Shield", "Star Dust")
    [TextArea]
    public string Description;      // 아이템 설명
    public Sprite Icon;         // Ui에 표시될 아이콘

    public ItemType itemType;       // 아이템 타입

    int IPrimaryKey.ID => ID;
}

public enum ItemType
{
    Consumable, // 소모품
    Equipment,  // 장비
    Material    // 재료
}