using System.Collections;
using UnityEngine;

public abstract class BaseGlobalManager : MonoBehaviour, ISetState
{
    public bool IsInit { get; private set; }

    public bool EndInit() => IsInit = true;
}