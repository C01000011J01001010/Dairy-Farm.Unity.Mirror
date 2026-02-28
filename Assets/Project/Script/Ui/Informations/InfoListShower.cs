using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoListShower<ObjectType, ContainerType, ShowerType> : DataListShower<ObjectType, ContainerType, ShowerType>
    where ObjectType : InfoObject
    where ContainerType : InfoContainer<ObjectType>
    where ShowerType : InfoShower<ObjectType, ContainerType>
{
    protected override ShowerType OnCreateShowerSucceed(ShowerType newShower, ContainerType newContainer)
    {
        newShower.Connect(newContainer);
        return base.OnCreateShowerSucceed(newShower, newContainer);
    }
}



