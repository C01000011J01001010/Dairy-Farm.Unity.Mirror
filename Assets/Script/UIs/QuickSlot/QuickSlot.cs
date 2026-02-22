using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public delegate void OnSelected(int index);

public class QuickSlot : BaseUi
{
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty scrollAction; // 마우스 휠 액션 연결

    public event OnSelected OnSlotSelected;

    private int currentIndex = 0;

    private BaseButton[] slots; // 슬롯 버튼들을 순서대로 할당

    public override MyUi UiType => MyUi.QuickSlot;

    public override void Exit()
    {

    }

    public override IEnumerator Initialize()
    {
        slots = GetComponentsInChildren<BaseButton>();


        // [추가] 모든 슬롯 버튼에 클릭 이벤트 리스너를 자동으로 등록합니다.
        for (int i = 0; i < slots.Length; i++)
        {
            int index = i; // 클로저(Closure) 문제를 피하기 위해 지역 변수 복사
            yield return slots[i].Initialize();
            slots[i].AddCallback(() => CALLBACK_SlotClicked(index));
        }

        // 처음 시작할 때 0번 슬롯 선택
        SelectSlot(0);
        yield return null;
    }

    private void OnEnable()
    {
        // 액션 활성화
        scrollAction.action.Enable();
        // 입력이 발생했을 때 실행될 함수 등록
        scrollAction.action.performed += CALLBACK_OnScroll;
    }

    private void OnDisable()
    {
        scrollAction.action.performed -= CALLBACK_OnScroll;
        scrollAction.action.Disable();
    }

    // 기존 스크립트의 Update 문에 추가하거나 별도 스크립트로 작성
    void Update()
    {
        // 현재 선택된 오브젝트가 없는데(null), 마우스 클릭 등으로 해제되었다면
        if (EventSystem.current?.currentSelectedGameObject == null &&
            slots != null && slots.Length > 0)
        {
            // 마지막으로 선택했던 슬롯(currentIndex)을 다시 강제로 선택
            EventSystem.current?.SetSelectedGameObject(slots[currentIndex]?.gameObject);
        }
    }

    private void SelectSlot(int index)
    {
        if (slots is null || slots.Length == 0)
        {
            Debug.LogWarning("slot bug");
            return;
        }


        GameObject targetSlot = slots[index].gameObject;
        EventSystem.current.SetSelectedGameObject(targetSlot);

        OnSlotSelected?.Invoke(index);
    }

    private void CALLBACK_OnScroll(InputAction.CallbackContext context)
    {
        // 마우스 휠의 Vector2 값 읽기 (y값이 휠 회전량)
        Vector2 scrollValue = context.ReadValue<Vector2>();

        if (scrollValue.y != 0)
        {
            // 휠 방향에 따라 인덱스 변경 (y가 양수면 위/이전, 음수면 아래/다음)
            if (scrollValue.y > 0) currentIndex--;
            else currentIndex++;

            // 인덱스가 슬롯 범위를 넘지 않게 순환(Wrap) 처리
            if (currentIndex < 0) currentIndex = slots.Length - 1;
            else if (currentIndex >= slots.Length) currentIndex = 0;

            // UI 선택 상태 업데이트
            SelectSlot(currentIndex);
        }
    }

    private void CALLBACK_SlotClicked(int index)
    {
        currentIndex = index; // 클릭한 버튼의 인덱스로 동기화
        SelectSlot(currentIndex);
        Debug.Log($"Clicked Slot: {index + 1}");
    }


}