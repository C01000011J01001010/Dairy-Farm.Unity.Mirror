/// <summary>
/// 메인이 되는 캐릭터 클래스 외에 Monohaviour를 상속받는 모든 클래스가 구현해야함
/// </summary>
/// <typeparam name="TCharacter">캐릭터 상세클래스</typeparam>
public interface ICharacterModule<TCharacter> where TCharacter : BaseCharacter
{
    public void Exit();
    public void Initialize(TCharacter owner);
}