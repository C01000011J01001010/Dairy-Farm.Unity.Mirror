using UnityEngine;

public class InfoItemContainer : InfoContainer<ItemObject>
{
    public InfoItemContainer() : base()
    {
        currentObject = null;
    }

    public InfoItemContainer(ItemObject InitialObject) : base(InitialObject)
    {
        Set(InitialObject);
    }
}
