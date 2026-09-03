using CoreEngine.DesignPattern.StateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;



namespace Farm.Character.StateMachine
{
    public class CharacterStateController : BaseStateController<CharacterState>
    {
        public BaseCharacter Owner { get; private set; }

        public bool IsActive { get; private set; }

        public event Action<bool> Evnet_OnSetActive;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Evnet_OnSetActive = null;
        }

        public override void Exit()
        {
            base.Exit();
            SetActive(false);
        }

        public void Initialize(BaseCharacter owner)
        {
            Owner = owner;
            Initialize();
        }

        public override void PostInitialize()
        {
            foreach (BaseState<CharacterState> cur in stateDict.Values)
            {
                if (cur is BaseCharacterState asCharacterState)
                {
                    asCharacterState.Initialize(Owner);
                }
            }
            StartState();
        }

        public void SetActive(bool active)
        {
            IsActive = active;
            Evnet_OnSetActive?.Invoke(active);
        }

        protected override Dictionary<CharacterState, BaseState<CharacterState>> ProductState()
        {
            Dictionary<CharacterState, BaseState<CharacterState>> stateDict = new();
            stateDict.Add(CharacterState.Idle, new State_Idle());
            stateDict.Add(CharacterState.Walk, new State_Walk());
            stateDict.Add(CharacterState.Sprint, new State_Sprint());
            return stateDict;
        }
    }
}



