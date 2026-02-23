using UnityEngine.TextCore.Text;

public class State_Idle : BaseCharacterState
{
    baseCharacterAnim anim;
    public State_Idle(BaseCharacter owner) : base(owner)
    {
        anim = owner.anim;
    }

    public override CharacterState? CheckTransitions()
    {
        if (owner.isMove)
        {
            if(owner.isSprint)return CharacterState.Sprint;
            else return CharacterState.Walk;
        }
        return null;
    }

    public override void Enter()
    {
        anim.SetIsMove(false);
    }

    public override void Exit(CharacterState? nextState)
    {
        
    }

    public override void Update()
    {
        
    }
}