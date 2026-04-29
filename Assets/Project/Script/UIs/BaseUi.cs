using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

// 모든 ui의 조상클래스
public abstract class BaseUi : MonoBehaviour, IBaseUi
{
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

public interface IBaseUi : IInitialize
{
    public void ClaimOpen();
    public void ClaimClose();

}


// 글로벌 UI들이 상속받는 인터페이스(태그)
public interface IGlobalUi : IBaseUi { } 

// 씬 UI들이 상속받는 인터페이스(태그)
public interface IScenedUi : IBaseUi { }

[Serializable]
public class AssetReferenceUi : AssetReferenceT<BaseUi>
{
    public AssetReferenceUi(string guid) : base(guid) { }
}


//[Serializable]
//public class AssetReferenceScenedUi : AssetReferenceT<ScenedUi>
//{
//    public AssetReferenceScenedUi(string guid) : base(guid) { }
//}

//[Serializable]
//public class AssetReferenceGlobalUi : AssetReferenceT<GlobalUi>
//{
//    public AssetReferenceGlobalUi(string guid) : base(guid) { }
//}
