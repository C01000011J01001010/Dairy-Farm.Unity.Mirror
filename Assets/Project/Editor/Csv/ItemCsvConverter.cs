using System;
using UnityEditor;
using UnityEngine;

public class ItemCsvConverter : BaseCsvConverter
{
    const string target = "Item";
    protected override string ConverterTarget => target;

    protected override Type TargetType => typeof(ItemObject);

    [MenuItem(defaultMenu + target)]
    public static void ShowWindow() => GetWindow<ItemCsvConverter>("CSV Converter");

    protected override void ConvertDetails(ScriptableObject asset, int rowNum, string[] cols)
    {
        ItemObject asItemInfo = asset as ItemObject;
        if (asItemInfo == null) WarningTypeError(asset);

        asItemInfo.description = cols[2].Trim();
    }

    
}
