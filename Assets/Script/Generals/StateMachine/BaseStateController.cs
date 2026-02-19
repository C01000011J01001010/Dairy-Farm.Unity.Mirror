using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseStateController<TState> : MonoBehaviour where TState : struct, Enum
{
    [SerializeField]protected TState currentStateType;
    protected BaseState<TState> CurrentState;
    protected Dictionary<TState, BaseState<TState>> stateDict = new();

    public TState CurrentStateType => currentStateType;

    public virtual void Exit()
    {
        foreach (var state in stateDict.Values)
        {
            state.Dispose();
        }
    }

    public virtual void Initialize(TState defaultState)
    {
        StateAdd();
        currentStateType = defaultState;
        CurrentState = GetState(defaultState);
        CurrentState.Enter();
    }
    protected abstract void StateAdd();

    public virtual void UpdateFromOwner()
    {
        TState? want = CurrentState.CheckTransitions();
        if (want == null)
        {
            CurrentState?.Update();
        }
        else
        {
            TransitionTo(want.Value);
        }
    }

    protected virtual void TransitionTo(TState want)
    {
        CurrentState.Exit(want);

        currentStateType = want;
        CurrentState = GetState(want);

        CurrentState.Enter();
    }

    protected virtual BaseState<TState> GetState(TState wantState)
    {
        if (stateDict.ContainsKey(wantState)) return stateDict[wantState];
        else
        {
            Debug.Log($"the key({wantState.ToString()}) not contained in stateDict");
            return null;
        }
    }

    
}