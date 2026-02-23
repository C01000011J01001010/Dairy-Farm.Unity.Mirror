using UnityEngine;

// 인스펙터 메뉴에서도 수동으로 만들 수 있게 메뉴를 등록합니다.
[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    [TextArea] // 설명을 길게 쓸 수 있게 텍스트 영역 제공
    public string description;
}