using UnityEngine.TextCore.Text;
using CoreEngine.Actor;

namespace Farm.Character.StateMachine
{
    public class State_Idle : BaseCharacterState
    {
        public override void Enter(IActorHost host)
        {
            CharacterAnimFeature anim = null;
            if (host.TryGetFeature(out anim))
                anim.SetIsMove(false);
        }

        public override CharacterState? CheckTransitions(IActorHost host)
        {
            BaseCharacter owner = host as BaseCharacter;
            if (owner?.isMove ?? false)
            {
                if (owner.isSprint) return CharacterState.Sprint;
                else return CharacterState.Walk;
            }
            return null;
        }

        public override void Exit(IActorHost host, CharacterState? nextState)
        {

        }
    }
}
