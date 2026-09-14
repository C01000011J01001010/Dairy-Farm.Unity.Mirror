using CoreEngine;
using CoreEngine.Actor;
using UnityEngine;

public class TestActor : BaseActor, IActorHost
{
    readonly TestFeature feature = new();
    protected void Awake()
    {
        feature.Initialize(this);
    }

    bool IActorHost.TryGetFeature<T>(out T feature)
    {
        throw new System.NotImplementedException();
    }
}
