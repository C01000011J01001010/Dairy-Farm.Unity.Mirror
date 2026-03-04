using UnityEngine;

//public enum ItemType
//{
//    Equipment,  // 장비
//    Consumable, // 소모품
//    Material    // 재료
//}

public abstract class BaseItem : InfoObject
{
	//public ItemType itemType;
	public int maxStack;
    public int sellPrice;

    /// <summary>
    /// 개별 클래스에서 어떻게 할건지 정의
    /// </summary>
    /// <param name="user">이 아이템의 사용자</param>
    /// <returns></returns>
    public virtual bool TryUse(BaseCharacter user) => true;
}


