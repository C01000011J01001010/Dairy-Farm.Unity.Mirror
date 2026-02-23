using UnityEngine;

public enum CharacterState
{
    None,
    Idle,
    Walk,
    Sprint,
}

public abstract class BaseCharacterState : BaseState<CharacterState>
{
    protected BaseCharacter owner;

    public BaseCharacterState(BaseCharacter owner)
    {
        this.owner = owner;
    }
}
