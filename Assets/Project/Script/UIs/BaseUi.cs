using System.Collections;
using UnityEngine;

public abstract class BaseUi : MonoBehaviour, IInitialize
{
    public abstract MyUi UiType { get; }

    public abstract void Exit();

    public abstract IEnumerator Initialize();

    public virtual void ClaimOpen()
    {
        // 활성화 후 가장 앞으로
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
    public virtual void ClaimClose()
    {
        gameObject.SetActive(false);
    }
}

