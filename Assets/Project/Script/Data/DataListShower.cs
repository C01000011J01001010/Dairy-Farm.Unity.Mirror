using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataListShower<ObjectType, ContainerType, ShowerType> : MonoBehaviour
    where ShowerType : Component
{
    protected Dictionary<ContainerType, ShowerType> showerDictionary = new();

    [SerializeField] protected LayoutGroup containBox;

    GameObject _showerPreset = null;
    public GameObject ShowerPreset
    {
        get
        {
            if (_showerPreset) return _showerPreset;
            else return _showerPreset = containBox?.transform.GetChild(0)?.gameObject;
        }
    }

    public void Awake()
    {
        if (ShowerPreset is null) Debug.LogError($"{gameObject.name} has no Preset Object");
        else ShowerPreset.SetActive(false); // 프리셋은 가져왔으니 꺼버림

        Initialize();
    }

    public virtual void Initialize() { }

    public ShowerType CreateShower(ContainerType newContainer)
    {
        if (ShowerPreset is null) return null;
        //생성해주기!
        GameObject inst = Instantiate(ShowerPreset, containBox.transform);
        // 생성되었고         컴포넌트도 잘 가지고 있다면
        if (inst && inst.TryGetComponent(out ShowerType result))
        {
            result = OnCreateShowerSucceed(result, newContainer); // 성공!
            result.gameObject.SetActive(true); // 설정 끝나면 켜기
            return result;

        }
        else // inst를 생성할 수 없었거나, 그 친구가 Shower가 아니던데?
        {
            return OnCreateShowerFailed(inst, newContainer); //실패!
        }
    }

    protected virtual ShowerType OnCreateShowerSucceed(ShowerType newShower, ContainerType newContainer) => newShower;
    protected virtual ShowerType OnCreateShowerFailed(GameObject inst, ContainerType newContainer)
    {
        //Instantiate가 실패하고 말았습니다 (프리팹의 널 체크는 위에서 했어요!)
        //메모리 할당에 실패한 거예요!
        if (inst)
        {
            //나중에 저희가 잘못 적용했을 때에 확인할 수 있도록 준비해준 에러 메시지!
            Debug.LogError($"Invalid Shower Preset : {inst.name}");
            Destroy(inst);
        }
        return null;
    }

    //Unique : 유일한 -> 중복으로 추가하지 않을 것임!
    public virtual void AddUnique(ContainerType newContainer)
    {
        //이미 있는 오브젝트라면 추가하지 않습니다!
        if (showerDictionary.ContainsKey(newContainer)) return;

        //shower를 만들고
        //새로운 요소를 추가하고
        //shower에 연결하고
        //그 다음에 layoutgroup에 추가해주기

        ShowerType newShower = CreateShower(newContainer);
        if (newShower is null) return;
        showerDictionary.Add(newContainer, newShower);
        //newShower.Connect(newContainer);

    }
}
