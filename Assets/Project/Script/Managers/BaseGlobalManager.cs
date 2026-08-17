using System.Collections;
using UnityEngine;

public abstract class BaseGlobalManager : MonoBehaviour, IGlobalManager
{
    public bool IsInit { get; private set; }

    public bool EndInit() => IsInit = true;

    public virtual void Exit() {  }

    public virtual IEnumerator Initialize() { yield break; }

    public virtual IEnumerator PostInitialize() { yield break; }
}