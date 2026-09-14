using UnityEngine;
using CoreEngine.Actor;

namespace Farm.Character.StateMachine
{
    public class State_Sprint : BaseCharacterState
    {
        public override void Enter(IActorHost host)
        {
            CharacterAnimFeature anim = null;
            var hostComp = host as UnityEngine.Component;
            if (hostComp != null && hostComp.TryGetComponent(out anim))
            {
                anim.SetIsMove(true);
                anim.SetIsSprint(true);
            }
        }

        public override void Update(IActorHost host, float deltaTime)
        {
            base.Update(host, deltaTime);

            BaseCharacter owner = host as BaseCharacter;
            if (owner == null) return;

            CharacterAnimFeature anim = null;
            var hostComp = host as UnityEngine.Component;
            if (hostComp != null && hostComp.TryGetComponent(out anim))
            {
                anim.SetInputMove(owner.inputMove);
            }
        }

        public override CharacterState? CheckTransitions(IActorHost host)
        {
            BaseCharacter owner = host as BaseCharacter;
            if(owner == null) return null;

            if (!owner.isMove) return CharacterState.Idle;
            else if (!owner.isSprint) return CharacterState.Walk;

            return null;
        }

        public override void Exit(IActorHost host, CharacterState? nextState)
        {

        }
    }
}
