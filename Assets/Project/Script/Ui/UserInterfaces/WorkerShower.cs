using UnityEngine;

public class WorkerShower : MonoBehaviour
{
    [SerializeField] GameObject prefab_SelectButton;
    [SerializeField] Transform buttonParent;
    void Start()
    {
        //현재 켜져있는 모든 일꾼을 정렬하지 않고 가져와서
        foreach (var currentWorker in FindObjectsByType<Worker>(FindObjectsSortMode.None))
        {
            AddWorker(currentWorker); //그냥 추가!
        }
    }
    protected virtual void SelectWorker(Worker target)
    {
        WorkController.ClaimSetWorker(target);
    }
    protected virtual WorkerSelectButton AddWorker(Worker target)
    {
        WorkerSelectButton result = null;
        GameObject inst = Instantiate(prefab_SelectButton, buttonParent); //버튼 만들고
        if(inst.TryGetComponent(out result)) result.Connect(target); //컴포넌트 있으면 연결
        return result;
    }
}
