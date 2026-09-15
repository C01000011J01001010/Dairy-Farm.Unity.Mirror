using CoreEngine.Actor;
using CoreEngine.DesignPattern.StateMachine;
using CoreEngine;


namespace Farm.Character.StateMachine
{
    [System.Serializable]
    public class CharacterStateController : BaseStateController<CharacterState, CharacterStateManager>,
        ITick, IFixedTick
    {

#if UNITY_EDITOR
        public void OnValidate()
        {
            defaultStateType = CharacterState.Idle;
        }
#endif
    }
}



