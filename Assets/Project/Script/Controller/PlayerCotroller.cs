using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.TextCore.Text;


public class PlayerCotroller : MonoBehaviour, IScenedInitialize
{
    [SerializeField] private int _priority = 10;

    private UserInputManager inputManager;
    private UiController uiController;

    private PlayableCharacter character;
    private CharacterInventory inventory;

    public PlayableCharacter curTargetCharacter => character;

    // 각 객체는(특히 Ui) 어떤 캐릭터가 대상이 될지 모르니 캐릭터의 이벤트에 연결할 수 없음
    // 그러므로 컨트롤러에서 해결
    public event System.Action<PlayableCharacter>  Event_OnControllTargetSet;
    public event System.Action<PlayableCharacter>  Event_OnControllTargetRemoved;

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
        inputManager.Event_OnUseItemInput -= UseItem;
    }

    public IEnumerator Initialize()
    {
        inputManager = GameManager.GetManager<UserInputManager>();
        uiController = WorldManager.GetObject<UiController>();

        inputManager.Event_OnUseItemInput += UseItem;
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
            if(character)
            {
                character.OnControllTargetRemoved();
                Event_OnControllTargetRemoved?.Invoke(character);
            }
            character = newCharacter;
            inventory = newCharacter.GetModule<CharacterInventory>();

            // 새캐릭터에 이벤트 연결됐음을 알림
            character.OnControllTargetSet();
            Event_OnControllTargetSet?.Invoke(newCharacter);
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

    public void UseItem()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("UI 클릭 중이므로 아이템 사용 액션을 무시");
            return;
        }
        inventory?.UseItem();
    }

}
