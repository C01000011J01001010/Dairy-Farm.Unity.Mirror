using CoreEngine.DesignPattern.StateMachine;
using CoreEngine.Actor;
using UnityEngine;

namespace Farm.Character.StateMachine
{
    public enum CharacterState
    {
        None,
        Idle,
        Walk,
        Sprint,
    }

    public abstract class BaseCharacterState : BaseState<CharacterState>
    {

    }

}

