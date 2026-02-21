using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputManager : MonoBehaviour, IGlobalManager
{
    public static UserInputManager tempInst { get; private set; }
    private  UserInputActions input;

    public Vector2 Move => input.Player.Move.ReadValue<Vector2>();
    public bool Sprint => input.Player.Sprint.IsPressed();

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
        input?.Disable();
    }

    public IEnumerator Initialize()
    {
        input ??= new UserInputActions();
        input?.Enable();
        yield return null;
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
