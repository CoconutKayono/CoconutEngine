using GameLogic;
using TEngine;
using Object = UnityEngine.Object;

public class GameModule
{
    #region 框架模块

    public static RootModule Base
    {
        get => _base ??= Object.FindAnyObjectByType<RootModule>();
        private set => _base = value;
    }

    private static RootModule _base;

    public static IDebuggerModule Debugger
    {
        get => _debugger ??= Get<IDebuggerModule>();
        private set => _debugger = value;
    }

    private static IDebuggerModule _debugger;

    public static IFsmModule Fsm => _fsm ??= Get<IFsmModule>();

    private static IFsmModule _fsm;

    public static IProcedureModule Procedure => _procedure ??= Get<IProcedureModule>();

    private static IProcedureModule _procedure;

    public static IResourceModule Resource => _resource ??= Get<IResourceModule>();

    private static IResourceModule _resource;

    public static IAudioModule Audio => _audio ??= Get<IAudioModule>();

    private static IAudioModule _audio;

    public static UIModule UI => _ui ??= UIModule.Instance;

    private static UIModule _ui;

    public static ISceneModule Scene => _scene ??= Get<ISceneModule>();

    private static ISceneModule _scene;

    public static ITimerModule Timer => _timer ??= Get<ITimerModule>();

    private static ITimerModule _timer;

    public static ILocalizationModule Localization => _localization ??= Get<ILocalizationModule>();

    private static ILocalizationModule _localization;

    public static INetworkModule NetworkModule => _networkModule ??= Get<INetworkModule>();

    private static INetworkModule _networkModule;

    #endregion

    private static T Get<T>() where T : class
    {
        T module = ModuleSystem.GetModule<T>();

        Log.Assert(condition: module != null, $"{typeof(T)} is null");

        return module;
    }

    public static void Shutdown()
    {
        Log.Info("GameModule Shutdown");

        _base = null;
        _debugger = null;
        _fsm = null;
        _procedure = null;
        _resource = null;
        _audio = null;
        _ui = null;
        _scene = null;
        _timer = null;
        _localization = null;
        _networkModule = null;
    }
}
