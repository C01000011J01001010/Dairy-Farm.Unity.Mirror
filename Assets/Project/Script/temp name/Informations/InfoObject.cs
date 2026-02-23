using UnityEngine;

//[CreateAssetMenu(fileName = "InfoObject", menuName = "Scriptable Objects/InfoObject")]
public abstract class InfoObject : ScriptableObject
{
	//퀘스트, 아이템, 스킬, 대사 등에 공통적으로 들어가는 요소가 뭘까?
	public Sprite icon;
    [TextArea(1, 3)]
    public string infoName;
    [TextArea(3, 5)]
    public string description;
	public int index;

    public virtual Sprite GetIcon() => icon;
    public virtual string GetInfoName() => infoName.ToReflectionText(this);
    public virtual string GetDescription() => description.ToReflectionText(this);
}
