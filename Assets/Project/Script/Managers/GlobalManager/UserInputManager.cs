using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputManager : BaseGlobalManager, IGlobalManager
{
    private  UserInputActions input;

    public Vector2 Move => input.Player.Move.ReadValue<Vector2>();
    public bool Sprint => input.Player.Sprint.IsPressed();
    public float ScrollY => input.Player.Scroll.ReadValue<Vector2>().y;

    public event System.Action Event_OnUseItemInput;

    private void OnEnable()
    {
        input?.Enable();
    }

    private void OnDisable()
    {
        input?.Disable();
    }

    public void Exit()
    {
        if(input != null)
        {
            input.Disable();

            input.Player.UseItem.performed -= OnUseItemInput;
        }
        
    }

    public IEnumerator Initialize()
    {
        input ??= new UserInputActions();

        if(input != null )
        {
            input.Enable();

            input.Player.UseItem.performed += OnUseItemInput;
        }
        yield return null;
    }

    private void OnUseItemInput(InputAction.CallbackContext context)
    {
        Event_OnUseItemInput?.Invoke();
    }

    public void OnOpenUi()
    {
        input.UI.Enable();       // UI 조작 입력 켬
        input.Player.Disable();  // 플레이어 이동 입력 끔
    }

    public void OnCloseUi()
    {
        input.UI.Disable();
        input.Player.Enable();   // 다시 플레이어 이동 가능하게 함
    }
}
