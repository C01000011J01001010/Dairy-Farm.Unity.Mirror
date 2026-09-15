using UnityEngine.TextCore.Text;
using CoreEngine.Actor;
using Farm.Character.Move;

namespace Farm.Character.StateMachine
{
    public class State_Idle : BaseCharacterState
    {
        public override void Enter(IActorHost host)
        {
            if (!host.TryGetFeature(out CharacterAnimFeature anim)) return;

            anim.SetIsMove(false);
        }

        public override CharacterState? CheckTransitions(IActorHost host)
        {
            if (!host.TryGetFeature(out CharacterMoveFeature move)) return null;
            if (move.isMove)
            {
                if (move.isSprint) return CharacterState.Sprint;
                else return CharacterState.Walk;
            }
            return null;
        }

        public override void Exit(IActorHost host, CharacterState? nextState)
        {

        }
    }
}
