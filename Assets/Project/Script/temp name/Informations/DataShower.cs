using TMPro;
using UnityEngine;
using UnityEngine.UI;

//InfoShower는 InfoObject종류라면 다 보여줄 수 있는 친구가 될 것이다~!
public class DataShower<T,F> : ObjectShower<F>
	//where T : InfoObject
	where F : InfoContainer<T>
{
	protected T currentTarget = default;
	[SerializeField] protected TextMeshProUGUI nameText, descriptionText;
	[SerializeField] protected Image iconImage;

	public void Set(T newTarget)
	{
		currentTarget = newTarget;
		Visualize(newTarget);
	}

    protected virtual void Visualize(T newTarget) { }


    public override void Visualize(F target)
	{
		if(target == null) return;
		Visualize(target.Get());
	}
}


