using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CoreEngine.Data;

public class WorkerSelectButton : BaseObjectViewer<Worker>
{
    [SerializeField] TextMeshProUGUI text_Name;
    Button selectbutton;

    void Start()
    {
        selectbutton = GetComponentInChildren<Button>();
        selectbutton.onClick.AddListener(Select);
        //나중에 바뀌면 알려주세요!
        WorkController.OnWorkerChanged += OnWorkerChanged;
        //지금은 어떤데요?
        OnWorkerChanged(WorkController.ClaimGetWorker());
    }


    public void Select()
    {
        //그냥 여기에 연결된 오브젝트를 일꾼으로 지정하기!
        WorkController.ClaimSetWorker(GetConnectedObject());
    }

    public void OnWorkerChanged(Worker target)
    {
        //버튼 활성화 여부는         새로운 일꾼이 내 것이 아닐 때!
        selectbutton.interactable = target != GetConnectedObject();
    }

    public override void UpdateView()
    {
        if (ConnectObject) text_Name.SetText(ConnectObject.gameObject.name);
    }
}
