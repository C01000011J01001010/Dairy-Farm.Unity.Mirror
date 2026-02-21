using UnityEngine;

public class State_Sprint : BaseCharacterState
{
    baseCharacterAnim anim;

    public State_Sprint(BaseCharacter owner) : base(owner) 
    {
        anim = owner.anim;
    }

    public override CharacterState? CheckTransitions()
    {
        if (!owner.isMove) return CharacterState.Idle;
        else if (!owner.isSprint) return CharacterState.Walk;

        return null;
    }

    public override void Enter()
    {
        anim.SetIsMove(true);
        anim.SetIsSprint(true);
    }

    public override void Exit(CharacterState? nextState)
    {

    }

    public override void Update()
    {
        anim.SetInputMove(owner.inputMove);
    }
}