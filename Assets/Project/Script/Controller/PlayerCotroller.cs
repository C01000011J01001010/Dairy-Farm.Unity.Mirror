using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;


public class PlayerCotroller : MonoBehaviour, IScenedInitialize
{
    [SerializeField] private int _priority = 10;

    private UserInputManager inputManager;
    private UiController uiController;

    private PlayableCharacter character;
    private CharacterInventory inventory;

    public PlayableCharacter curTargetCharacter => character;
    public event System.Action<PlayableCharacter>  OnControllTargetSet;
    public event System.Action<PlayableCharacter>  OnControllTargetRemoved;

    public int Priority => _priority;

    public static event System.Action<float/*마우스 휠 스크롤*/> OnQuickSlotScrollInput;



    private void OnEnable()
    {
        GameManager.UPDATE_OnController += Tick;
    }

    private void OnDisable()
    {
        GameManager.UPDATE_OnController -= Tick;
    }

    public void Exit()
    {
        
    }

    public IEnumerator Initialize()
    {
        inputManager = GameManager.GetManager<UserInputManager>();
        uiController = WorldManager.GetObject<UiController>();
        yield return null;
    }
    public IEnumerator PostInitialize() 
    { 
        yield break; 
    }

    public void SetControllTarget(PlayableCharacter newCharacter)
    {
        
        
        if (newCharacter != null)
        {
            // 기존 캐릭터가 있으면 메모리 정리
            if(character) OnControllTargetRemoved?.Invoke(character);
            character = newCharacter;
            inventory = newCharacter.GetModule<CharacterInventory>();

            // 새캐릭터에 이벤트 연결됐음을 알림
            OnControllTargetSet?.Invoke(newCharacter);
        }
        else
        {
            Debug.LogWarning("컨트롤 타겟이 존재하지 않음");
        }
    }

    private void Tick()
    {
        InputMove();
        InputSprint();
        InputScroll();
    }
    #region Tick
    private void InputMove()
    {
        character?.Move(inputManager.Move);
    }

    private void InputSprint()
    {
        character?.SprintHold(inputManager.Sprint);
    }

    private void InputScroll()
    {
        float scrollDelta = inputManager.ScrollY;

        if (scrollDelta != 0)
        {
            inventory?.ScrollSlot(scrollDelta);
        }
    }
    #endregion


    #region Handle Ui
    public void HandleUI_QuickSlotClicked(int index)
    {
        // Controller가 UI 이벤트를 받아 Model에게 데이터 변경을 지시
        inventory?.SetSelectedSlot(index);
    }
    #endregion

}
