using System;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    static ProductField lastField;

    public static event Action<ProductField> OnFieldFocusChanged;

    public static ProductField GetFocusedField() => lastField;

    public static void SetFieldFocus(ProductField newField)
    {
        //전에 알던 거랑 다르니까 이벤트 발생!
        if (newField != lastField)
        {
            OnFieldFocusChanged?.Invoke(newField);
            lastField = newField;
        }
    }
}
