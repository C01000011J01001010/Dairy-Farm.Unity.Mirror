using System.Collections;

public abstract class BaseSelection : BaseUi, IInitializable
{
    public override void Exit()
    {
        ClearButtonCallback();
    }

    public override IEnumerator Initialize()
    {
        SetButtonCallback();
        yield return null;
    }

    protected abstract void SetButtonCallback();
    protected abstract void ClearButtonCallback();
}
