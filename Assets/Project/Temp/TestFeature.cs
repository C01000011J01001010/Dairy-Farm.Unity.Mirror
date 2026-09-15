using CoreEngine.Actor;
using System;
using System.Collections;
using UnityEngine;

public class TestFeature : IActorFeature
{
    public IActorHost Host { get; private set;  }

    public bool IsInit {  get; private set; }

    public void Initialize(IActorHost host)
    {
        Host = host;
        TestActor testActor = host as TestActor;
        Host.TryGetFeature<TestFeature>(out var temp);
        Host.GetComponent<Rigidbody>();
    }
}
