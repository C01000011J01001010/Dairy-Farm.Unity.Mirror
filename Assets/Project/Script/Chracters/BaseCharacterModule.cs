using System;
using UnityEngine;

public class BaseCharacterModule : MonoBehaviour, ICharacterModule
{
    public BaseCharacter Owner { get; protected set; }

    public bool IsActive { get; protected set; }

    public event Action<bool> Evnet_OnSetActive;

    public void SetActive(bool active)
    {
        IsActive = active;
        Evnet_OnSetActive?.Invoke(active);
    }

    private void OnDestroy()
    {
        Evnet_OnSetActive = null;
    }

    public virtual void Exit()
    {
        SetActive(false);
    }

    public virtual void Initialize(BaseCharacter owner) { Owner = owner; }

    public virtual void PostInitialize() { }

    public virtual void FixedTick(float fixedDeltaTime) { }

    public virtual void Tick(float deltaTime) { }
}