using UnityEngine;

public class InfoSkillContainer : InfoContainer<InfoSkill>
{
    public InfoSkillContainer() : base()
    {
        currentObject = null;
    }

    public InfoSkillContainer(InfoSkill InitialObject) : base(InitialObject)
    {
        Set(InitialObject);
    }
}
