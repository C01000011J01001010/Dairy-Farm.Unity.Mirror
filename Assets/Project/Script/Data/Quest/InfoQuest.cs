using UnityEngine;

public enum ComparisonType
{
    GreaterThanOrEqual, GreaterThan,
    LessThanOrEqual,    LessThan,
    Equal,              NotEqual,
}

[System.Serializable]
public struct QuestTrigger
{
    public string title; // 조건의 이름
	public string condition; // 이런 이벤트가 일어나면 트리거하고 싶어요!
	public string identifier; // 그 대상이 누군지도 보고 싶다!
    public int count; // 그 대상이 몇 개 필요한지
    public ComparisonType comparison;
    public bool isEssential; // 이거 꼭 있어야 하는 퀘스트인가?
}

[CreateAssetMenu(fileName = "InfoQuest", menuName = "Quests/NormalQuest")]
public class InfoQuest : BaseData_ForUi
{
	//이런 코드가 들어오는 순간 퀘스트 달성 확인
	public QuestTrigger[] successTrigger; 
	public QuestTrigger[] failTrigger;

	//Condition ID
	//  Kill    Goblin : Require
	//  Kill    Slime  : Do
	//  Kill Slime 1
	//public int requirements;
}
