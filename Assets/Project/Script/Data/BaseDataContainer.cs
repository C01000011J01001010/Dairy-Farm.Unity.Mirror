using System;
using UnityEngine;


public abstract class BaseDataContainer<Data> : BaseObjectContainer<Data>
    where Data : BaseData
{
    public int GetIndex() => connectData.index;
    public string GetNameTag() => connectData.nameTag;

    public BaseDataContainer() : base() { }
    public BaseDataContainer(Data InitialObject) : base(InitialObject) { }
}
