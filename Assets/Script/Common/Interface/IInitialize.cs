using System.Collections;

public interface IBaseInitializable
{
    void Exit();
}

public interface IInitialize : IBaseInitializable
{
    IEnumerator Initialize();
}

public interface ILateInitialize : IBaseInitializable
{
    IEnumerator LateInitialize();
}

public interface IScenedInitialize : IInitialize, ILateInitialize, IPriority { }