using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseStateController<TState> : MonoBehaviour where TState : struct, Enum
{
    [SerializeField]protected TState currentStateType;
    protected BaseState<TState> CurrentState;
    protected Dictionary<TState, BaseState<TState>> stateDict;

    public TState CurrentStateType => currentStateType;

    public virtual void Exit()
    {
        // None을 전달
        CurrentState?.Exit(null);
    }

    public virtual void Initialize(TState defaultState)
    {
        stateDict = ProductState();
        currentStateType = defaultState;
        CurrentState = GetState(defaultState);
        CurrentState.Enter();
    }
    protected abstract Dictionary<TState, BaseState<TState>> ProductState();

    public virtual void UpdateFromOwner()
    {
        TState? nextState = CurrentState.CheckTransitions();

        if (nextState.HasValue)
        {
            TransitionTo(nextState.Value);
            return;
        }
        CurrentState?.Update();
    }

    protected virtual void TransitionTo(TState nextState)
    {
        CurrentState.Exit(nextState);

        currentStateType = nextState;
        CurrentState = GetState(nextState);

        CurrentState.Enter();
    }

    protected virtual BaseState<TState> GetState(TState wantState)
    {
        if (stateDict.ContainsKey(wantState)) return stateDict[wantState];
        else
        {
            Debug.LogWarning($"the key({wantState.ToString()}) not contained in stateDict");
            return null;
        }
    }

    
}