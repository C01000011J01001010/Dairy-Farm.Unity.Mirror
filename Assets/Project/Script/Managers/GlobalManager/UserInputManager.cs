using CoreEngine.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputManager : BaseInputManager<UserInputActions>
{

    public Vector2 Move => inputAction.Player.Move.ReadValue<Vector2>();
    public bool Sprint => inputAction.Player.Sprint.IsPressed();
    public float ScrollY => inputAction.Player.Scroll.ReadValue<Vector2>().y;

    public event System.Action Event_OnUseItemInput;


    public override void Exit()
    {
        base.Exit();
        if(inputAction != null)
        {
            inputAction.Disable();

            inputAction.Player.UseItem.performed -= OnUseItemInput;
        }
        
    }

    public override IEnumerator Initialize()
    {
        base.Initialize();

        if(inputAction != null )
        {
            inputAction.Enable();

            inputAction.Player.UseItem.performed += OnUseItemInput;
        }
        yield return null;
    }

    private void OnUseItemInput(InputAction.CallbackContext context)
    {
        Event_OnUseItemInput?.Invoke();
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
