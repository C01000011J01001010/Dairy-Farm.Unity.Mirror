public class InfoShower<T, F> : DataShower<T, F>
    where T : InfoObject
    where F : InfoContainer<T>
{
    protected override void Visualize(T newTarget)
    {
        if (newTarget == null) return;
        SetDescription(newTarget);
        SetName(newTarget);
        SetIcon(newTarget);
    }
    public virtual void SetDescription(T newTarget) { descriptionText.text = newTarget.GetDescription(); }
    public virtual void SetName(T newTarget) { nameText.text = newTarget.GetInfoName(); }
    public virtual void SetIcon(T newTarget) { iconImage.sprite = newTarget.GetIcon(); }
}