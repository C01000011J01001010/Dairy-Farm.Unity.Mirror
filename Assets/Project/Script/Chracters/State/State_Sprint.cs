using UnityEngine;

namespace Farm.Character.StateMachine
{
    public class State_Sprint : BaseCharacterState
    {
        public override void Enter(CharacterStateController controller)
        {
            CharacterAnim anim = null;
            if(controller.Host.TryGetComponent(out anim))
            {
                anim.SetIsMove(true);
                anim.SetIsSprint(true);
            }
            
            
        }

        public override void Update(CharacterStateController controller, float deltaTime)
        {
            base.Update(controller, deltaTime);

            BaseCharacter owner = controller.Host as BaseCharacter;
            if (owner == null) return;

            CharacterAnim anim = null;
            if (controller.Host.TryGetComponent(out anim))
            {
                anim.SetInputMove(owner.inputMove);
            }
        }

        public override CharacterState? CheckTransitions(CharacterStateController controller)
        {
            BaseCharacter owner = controller.Host as BaseCharacter;
            if(owner == null) return null;

            if (!owner.isMove) return CharacterState.Idle;
            else if (!owner.isSprint) return CharacterState.Walk;

            return null;
        }

        

        public override void Exit(CharacterStateController controller, CharacterState? nextState)
        {

        }
    }
}
