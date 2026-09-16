using CoreEngine.DesignPattern.StateMachine;
using CoreEngine;
using System.Diagnostics;


namespace Farm.Character.StateMachine
{
    [System.Serializable]
    public class CharacterStateController : BaseStateController<CharacterState, CharacterStateManager>,
        ITick, IFixedTick
    {

#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            defaultStateType = CharacterState.Idle;
        }
#endif
    }
}



