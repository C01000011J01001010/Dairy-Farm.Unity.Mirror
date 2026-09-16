using CoreEngine.Actor;
using Farm.Character.Move;
using UnityEngine;

namespace Farm.Character.StateMachine
{
    public class State_Walk : BaseCharacterState
    {


        public override void Enter(IActorHost host)
        {
            if (!host.TryGetFeature(out CharacterAnimFeature anim)) return;

            anim.SetIsMove(true);
            anim.SetIsSprint(false);
        }

        public override CharacterState? CheckTransitions(IActorHost host)
        {
            if (!host.TryGetFeature(out CharacterMoveFeature move)) return null;

            if (!move.isMove) return CharacterState.Idle;
            else if (move.IsSprint) return CharacterState.Sprint;
            return null;
        }

        public override void Update(IActorHost host, float deltaTime)
        {
            if (!host.TryGetFeature(out CharacterAnimFeature anim) ||
                !host.TryGetFeature(out CharacterMoveFeature move)) return;

            anim.SetInputMove(move.inputMove);
        }

        public override void Exit(IActorHost host, CharacterState? nextState)
        {

        }
    }
}
