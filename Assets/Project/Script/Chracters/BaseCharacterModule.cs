using System;
using UnityEngine;

public class BaseCharacterModule : MonoBehaviour, ICharacterModule
{
    public BaseCharacter Owner { get; protected set; }

    public bool IsActive { get; protected set; }

    public event Action<bool> OnSetActive;

    public void SetActive(bool active)
    {
        IsActive = active;
        OnSetActive?.Invoke(active);
    }

    private void OnDestroy()
    {
        OnSetActive = null;
    }

    public virtual void Exit()
    {
        SetActive(false);
    }

    public virtual void Initialize(BaseCharacter owner) { }

    public virtual void PostInitialize() { }

    public virtual void OnFixedTick(float fixedDeltaTime) { }

    public virtual void OnTick(float deltaTime) { }
}