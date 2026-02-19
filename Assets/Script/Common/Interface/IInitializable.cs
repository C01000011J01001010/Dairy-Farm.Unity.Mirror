using System.Collections;

public interface IInitializable
{
    public void Exit();
    public IEnumerator Initialize();
}