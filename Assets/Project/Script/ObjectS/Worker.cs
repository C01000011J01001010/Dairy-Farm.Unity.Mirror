using Unity.VisualScripting;
using UnityEngine;

public class Worker : MonoBehaviour
{
    ProductField workingField;

    [SerializeField] GameObject selectedSign;

    GameObject destroyTarget;
    [SerializeField] float destroyRange = .5f;

    void Update()
    {
        if (destroyTarget)
        {
            Vector3 direction = (destroyTarget.transform.position - transform.position);
            float distance = direction.magnitude;
            if(distance < destroyRange)
            {
                Destroy(destroyTarget);
            }
            else
            {
                transform.Translate(3.0f * Time.deltaTime * direction.normalized);
            }
        }
        else if(workingField) //부술 건 없는데.. 일할 곳은 있으면
        {
            GetNextDestroyTarget(workingField); //여기의 다른 생산품 찾기!
        }
    }
    
    public virtual void OnSelected()
    {
        selectedSign.SetActive(true);
    }

    public virtual void OnDeselected()
    {
        selectedSign.SetActive(false);
    }

    public virtual bool SetField(ProductField newField)
    {
        workingField = newField;
        GetNextDestroyTarget(workingField);
        return workingField;
    }
    public virtual void GetNextDestroyTarget(ProductField targetField)
    {
        if (targetField)
        {
            destroyTarget = targetField.GetProduct(GetPreferedProduct);
        }
    }

    public virtual bool GetPreferedProduct(GameObject target)
    {
        if (target) return true;
        return false;
    }
}
