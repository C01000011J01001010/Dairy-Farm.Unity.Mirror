using UnityEngine;


// 모든 csv 데이터의 기초형태
public abstract class DataObject : ScriptableObject
{
    public int index;

    [TextArea(1, 3)]
    public string nameTag = ""; // 표시될 이름

    public virtual string GetInfoName() => nameTag.ToReflectionText(this);
}
