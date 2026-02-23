using System;
using UnityEngine;

public class WorkController : MonoBehaviour
{
    public static WorkController Inst { get; protected set; }
    public static event Action<Worker> OnWorkerChanged;
    [SerializeField] Worker character;

    void Awake()
    {
        if(Inst) Destroy(gameObject);
        else     Inst = this;
    }

    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            HarvestField(FieldManager.GetFocusedField());
        }
    }

    //컨트롤러 객체가 실제로 사용하는 함수!
    protected virtual void HarvestField(ProductField targetField)
    {
        if(character) character.SetField(targetField);
    }

    //외부에서 요청할 수 있게 준비해둔 함수!
    public static void ClaimHarvestField(ProductField targetField)
    {
        if (Inst) Inst.HarvestField(targetField);
    }

    //일꾼 선택!
    protected virtual void SetWorker(Worker target)
    {
        character?.OnDeselected();
        character = target;
        target.OnSelected();
        OnWorkerChanged?.Invoke(character);
    }

    //해달라고 부탁하기!
    public static void ClaimSetWorker(Worker target)
    {
        if (Inst) Inst.SetWorker(target);
    }

    public static Worker ClaimGetWorker()
    {
        return Inst?.character;
    }
}
