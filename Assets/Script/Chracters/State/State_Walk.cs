using UnityEngine;

public class State_Walk : BaseCharacterState
{
    baseCharacterAnim anim;

    public State_Walk(BaseCharacter owner) : base(owner) 
    {
        anim = owner.anim;
    }

    public override CharacterState? CheckTransitions()
    {
        if (!owner.isMove) return CharacterState.Idle;
        else if (owner.isSprint) return CharacterState.Sprint;
        return null;
    }

    public override void Enter()
    {
        anim.SetIsMove(true);
        anim.SetIsSprint(false);
    }

    public override void Exit(CharacterState? nextState)
    {

    }

    public override void Update()
    {
        anim.SetInputMove(owner.inputMove);
    }
}