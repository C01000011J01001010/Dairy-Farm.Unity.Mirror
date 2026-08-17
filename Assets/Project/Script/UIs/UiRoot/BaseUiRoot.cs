using System.Collections;
using UnityEngine;

public class BaseUiRoot : MonoBehaviour, IInitialize, IPostInitialize, IPriority
{
    public int _priority = 0;
    public int Priority => _priority;

    public void Exit() { }

    public IEnumerator Initialize() { yield return null; }

    public IEnumerator PostInitialize() { yield return null; }
}
