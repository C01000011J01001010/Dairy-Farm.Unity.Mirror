using CoreEngine.EventBus;
using CoreEngine.Input;
using CoreEngine.Interface;
using CoreEngine.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Farm.Input
{
    #region 입력 인터페이스
    #endregion

    #region 입력 이벤트
    public struct UseItemEvent : IEvent { }
    #endregion

    public class UserInputManager : BaseInputManager<UserInputActions>, 
        IMoveInput,ISprintInput, IScrollDeltaInput
    {

        //public Vector2 Move => inputAction.Player.Move.ReadValue<Vector2>();
        //public bool Sprint => inputAction.Player.Sprint.IsPressed();
        //public float ScrollY => inputAction.Player.Scroll.ReadValue<Vector2>().y;

        // 인터페이스 공급 관리
        private readonly InterfaceBinderContainer interfacePublisherBinder = new();

        UserInputActions.PlayerActions PlayerActions => inputAction.Player;

        #region 인터페이스 구현
        Vector2 IMoveInput.Value => PlayerActions.Move.ReadValue<Vector2>();
        bool ISprintInput.Value => PlayerActions.Sprint.IsPressed();
        float IScrollDeltaInput.Value => PlayerActions.Scroll.ReadValue<Vector2>().y;
        #endregion
        


        //public event System.Action Event_OnUseItemInput;


        public override void Exit()
        {
            base.Exit();

            PlayerActions.UseItem.performed -= OnUseItemInput;

            interfacePublisherBinder.UnbindAll();
        }

        public override IEnumerator Initialize()
        {
            yield return base.Initialize();

            PlayerActions.UseItem.performed += OnUseItemInput;

            interfacePublisherBinder.Add(new InterfacePublisher<IMoveInput>(this));
            interfacePublisherBinder.Add(new InterfacePublisher<ISprintInput>(this));
            interfacePublisherBinder.Add(new InterfacePublisher<IScrollDeltaInput>(this));
            interfacePublisherBinder.BindAll();

            yield return null;
        }

        private void OnUseItemInput(InputAction.CallbackContext context)
        {
            EventBus<UseItemEvent>.Publish(new UseItemEvent());
            //Event_OnUseItemInput?.Invoke();
        }

        public void OnOpenUi()
        {
            inputAction.UI.Enable();       // UI 조작 입력 켬
            inputAction.Player.Disable();  // 플레이어 이동 입력 끔
        }

        public void OnCloseUi()
        {
            inputAction.UI.Disable();
            inputAction.Player.Enable();   // 다시 플레이어 이동 가능하게 함
        }
    }
}

