using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 농작물의 정적 데이터 관리
/// </summary>
public class CropManager : BaseDBManager<CropData>, IGlobalManager
{
    protected override string Directory => "ScriptableObject/Crops";
}