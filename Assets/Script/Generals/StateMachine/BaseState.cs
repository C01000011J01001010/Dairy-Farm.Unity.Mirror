using System;
using UnityEngine;

public abstract class BaseState <TState> where TState : struct, Enum
{
    // state 시작시
    public abstract void Enter();

    // 매 프레임에서 Update보다 먼저 실행
    // 여기서 분기되면 Exit으로 이동
    public abstract TState? CheckTransitions();

    public abstract void Update();

    // state 종료시
    public abstract void Exit(TState? nextState);
}
