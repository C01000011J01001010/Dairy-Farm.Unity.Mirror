using UnityEngine;

public class InfoItemContainer : InfoContainer<InfoItem>
{
    public InfoItemContainer() : base()
    {
        currentObject = null;
    }

    public InfoItemContainer(InfoItem InitialObject) : base(InitialObject)
    {
        Set(InitialObject);
    }
}
