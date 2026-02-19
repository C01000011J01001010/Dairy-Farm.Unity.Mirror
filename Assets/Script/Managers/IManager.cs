
public interface IManager : IInitializable
{

}

public interface IScenedManager : IManager,IPriority {}

public interface IGlobalManager : IManager { }
