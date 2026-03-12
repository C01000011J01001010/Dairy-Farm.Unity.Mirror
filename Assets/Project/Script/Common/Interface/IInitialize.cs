using System.Collections;

public interface IBaseInitializable
{
    void Exit();
}

public interface ISetState
{
    public bool IsInit { get; }
    public bool EndInit();
}

public interface IInitialize : IBaseInitializable
{
    IEnumerator Initialize();
}

public interface IPostInitialize : IBaseInitializable
{
    IEnumerator PostInitialize();
}

public interface IScenedInitialize : IInitialize, IPostInitialize, IPriority { }