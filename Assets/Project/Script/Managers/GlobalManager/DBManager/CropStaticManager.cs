using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 농작물의 정적 데이터 관리
/// </summary>
public class CropStaticManager : BaseStaticDataManager<CropData>, IGlobalManager
{
    protected override string FolderName => "Crops";
}