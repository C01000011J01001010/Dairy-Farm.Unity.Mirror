using System.Collections.Generic;
using UnityEngine;





public class CharacterStateController : BaseStateController<CharacterState>
{
    BaseCharacter owner;

    protected override Dictionary<CharacterState, BaseState<CharacterState>> ProductState()
    {
        Dictionary<CharacterState, BaseState<CharacterState>> stateDict = new();
        stateDict.Add(CharacterState.Idle, new State_Idle(owner));
        stateDict.Add(CharacterState.Walk, new State_Walk(owner));
        stateDict.Add(CharacterState.Sprint, new State_Sprint(owner));
        return stateDict;
    }

    public override void Initialize(CharacterState defaultState)
    {
        owner = GetComponent<BaseCharacter>();
        base.Initialize(defaultState);
    }
}
