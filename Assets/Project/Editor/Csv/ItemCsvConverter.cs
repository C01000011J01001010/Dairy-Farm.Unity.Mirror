using System;
using UnityEditor;
using UnityEngine;

public class ItemCsvConverter : BaseCsvConverter
{
    const string target = "Item";
    protected override string ConverterTarget => target;

    protected override Type TargetType => typeof(ItemData);

    [MenuItem(defaultMenu + target)]
    public static void ShowWindow() => GetWindow<ItemCsvConverter>("CSV Converter");

    protected override void ConvertDetails(ScriptableObject asset, int rowNum, string[] cols)
    {
        ItemData asItemInfo = asset as ItemData;
        if (asItemInfo == null) WarningTypeError(asset);

        asItemInfo.SetFieldByReflection(headers[2], cols[2].Trim());
    }
}
