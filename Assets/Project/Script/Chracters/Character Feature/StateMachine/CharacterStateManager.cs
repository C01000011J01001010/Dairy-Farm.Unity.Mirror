using UnityEngine;
using CoreEngine.DesignPattern.StateMachine;

namespace Farm.Character.StateMachine
{
    public class CharacterStateManager : BaseStateManager<CharacterState>
    {
        protected override void SetUpStates()
        {
            AddState(CharacterState.Idle, new State_Idle());
            AddState(CharacterState.Walk, new State_Walk());
            AddState(CharacterState.Sprint, new State_Sprint());
        }
    }

}
