using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템의 정적 데이터 관리
/// </summary>
public class ItemManager : BaseDBManager<ItemData>, IGlobalManager
{
    protected override string Directory => "ScriptableObject/Items";
}