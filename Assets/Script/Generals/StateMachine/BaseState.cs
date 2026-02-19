using System;
using UnityEngine;

public abstract class BaseState <TState> : IDisposable where TState : struct, Enum
{
    public virtual void Dispose() { }

    // state 시작시
    public virtual void Enter() { }

    // 매 프레임에서 Update보다 먼저 실행
    // 여기서 분기되면 Exit으로 이동
    public abstract TState? CheckTransitions(); 

    public virtual void Update(){}

    // state 종료시
    public virtual void Exit(TState nextState){}
}
