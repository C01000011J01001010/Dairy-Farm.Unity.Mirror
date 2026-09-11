using UnityEngine.TextCore.Text;

namespace Farm.Character.StateMachine
{
    public class State_Idle : BaseCharacterState
    {
        public override void Enter(CharacterStateController controller)
        {
            CharacterAnim anim = null;
            if (controller.Host.TryGetFeature(out anim))
                anim.SetIsMove(false);
        }

        public override CharacterState? CheckTransitions(CharacterStateController controller)
        {
            BaseCharacter owner = controller.Host as BaseCharacter;
            if (owner?.isMove ?? false)
            {
                if (owner.isSprint) return CharacterState.Sprint;
                else return CharacterState.Walk;
            }
            return null;
        }

        public override void Exit(CharacterStateController controller, CharacterState? nextState)
        {
            
        }
    }
}
