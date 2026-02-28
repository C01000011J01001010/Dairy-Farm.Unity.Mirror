using UnityEngine;

public abstract class InfoObject : DataObject
{
    [TextArea(3, 5)]
    public string description; // ui띄울 설명

	public Sprite icon; // ui에 띄울 아이콘

    public virtual string GetDescription() => description.ToReflectionText(this);
    public virtual Sprite GetIcon() => icon;
}
