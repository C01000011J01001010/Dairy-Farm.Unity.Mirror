
using System.Collections;



public interface IManager : IInitialize { }

public interface IGlobalManager : IManager, ISetState { }

public interface IScenedManager : IManager, IPostInitialize, IPriority { }


