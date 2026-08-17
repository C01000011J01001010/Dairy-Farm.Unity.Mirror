
using System.Collections;



public interface IManager : IInitialize { }

//public interface IGlobalManager : IManager, ISetState { }
public interface IGlobalManager : IManager, IPostInitialize, ISetState { } // 이후에 변경

public interface IScenedManager : IManager, IPostInitialize, IPriority { }