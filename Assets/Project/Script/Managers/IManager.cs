
using System.Collections;

public interface IManager : IInitialize { }

public interface IGlobalManager : IManager { }

public interface IScenedManager : IManager, ILateInitialize, IPriority { }


