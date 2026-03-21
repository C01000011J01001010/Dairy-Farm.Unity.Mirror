public enum ComparisonType
{
    GreaterThanOrEqual, GreaterThan,
    LessThanOrEqual, LessThan,
    Equal, NotEqual,
}

public abstract class BaseQuestCondition : BaseCondition
{
    public string title; // 조건의 이름
    public string condition; // 이런 이벤트가 일어나면 트리거하고 싶어요!
    public string identifier; // 그 대상이 누군지도 보고 싶다!
    public int count; // 그 대상이 몇 개 필요한지
    public ComparisonType comparison;

    public override bool IsSatisfied(BaseCharacter character)
    {
        return true;
    }
}