using System;
using UnityEngine;

public class Product : MonoBehaviour
{
    public event Action<GameObject> OnDestroyed;

    void OnDestroy()
    {
        OnDestroyed(gameObject);
    }
}
