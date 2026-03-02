using System;
using UnityEngine;


public class InfoContainer<T> //: MonoBehaviour
	//where T : class
    
{
    [SerializeField]
	protected T currentObject;

    public InfoContainer()
    {
        currentObject = default;
    }

    public InfoContainer(T InitialObject)
    {
        Set(InitialObject);
    }

    public T Get() => currentObject;
	public T Set(T newObject)
	{
		return currentObject = newObject;
	}

    

}
