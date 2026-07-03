using Cysharp.Threading.Tasks;
using Fantasy;
using Fantasy.Async;
using Fantasy.Helper;
using Fantasy.Network;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using YooAsset;
using static TEngine.Constant;

namespace TEngine
{
    /// <summary>
    /// 网络模块：Fantasy Entry + Scene + Session
    /// </summary>
    public class NetworkModule : Module, INetworkModule
    {
        private Scene _scene;
        private Session _session;
        private Action _onConnectComplete;
        private Action _onConnectFail;
        private Action _onConnectDisconnect;

        public Session Session => _session;

        public string GateHost { get; set; }

        public int GatePort { get; set; }

        public NetworkProtocolType Protocol { get; set; }

        public int ConnectTimeoutMs { get; set; }

        public bool IsConnected => _session != null && !_session.IsDisposed;

        public string GateAddress => $"{GateHost}:{GatePort}";

        public bool EnableHeartbeat { get; set; }

        public int HeartbeatIntervalMs { get; set; }

        public int HeartbeatTimeOutMs { get; set; }

        public int HeartbeatTimeOutIntervalMs { get; set; }

        public int MaxPingSamples { get; set; }

        public override void OnInit()
        {
        }

        public override void Shutdown()
        {
            Disconnect();
            _scene?.Dispose();
            _scene = null;
        }

        public async UniTask InitializeAsync()
        {
            // 1. 加载热更新程序集(已在 ProcedureLoadAssembly 加载)
            // 注意: 必须在 Entry.Initialize() 之前加载
            //await LoadHotUpdateAssemblies();

            // 2. 初始化 Fantasy 框架
            // 此方法会自动:
            //   - 初始化日志系统(UnityLog)
            //   - 初始化序列化系统
            //   - 创建 Fantasy GameObject(DontDestroyOnLoad)
            //   - 注册 Update/LateUpdate 循环
            //   - WebGL 平台下初始化线程同步上下文
            await Fantasy.Platform.Unity.Entry.Initialize();

            // 3. 创建客户端 Scene
            // Scene 是客户端的核心容器,管理所有实体、组件和事件
            // 参数 arg: 传递给 OnSceneCreate 事件的自定义参数
            // 参数 sceneRuntimeMode: 场景运行模式(MainThread/MultiThread/ThreadPool)
            _scene = await Fantasy.Platform.Unity.Entry.CreateScene(
                arg: null,
                sceneRuntimeMode: SceneRuntimeMode.MainThread
            );
        }

        public async UniTask ConnectAsync(Action onConnectComplete = null,Action onConnectFail = null,Action onConnectDisconnect = null)
        {
            await InitializeAsync();

            if (IsConnected)
            {
                onConnectComplete?.Invoke();
                return;
            }

            _onConnectComplete = onConnectComplete;
            _onConnectFail = onConnectFail;
            _onConnectDisconnect = onConnectDisconnect;

            _session = _scene.Connect(
            remoteAddress: GateAddress,
            networkProtocolType: Protocol,
            onConnectComplete: OnConnectComplete,
            onConnectFail: OnConnectFail,
            onConnectDisconnect: OnConnectDisconnect,
            isHttps: false,
            connectTimeout: ConnectTimeoutMs,
            enableReceiveMessageJsonLog: false
            );

            Log.Debug("HybridCLR + Fantasy 初始化完成!");
        }

        private void OnConnectComplete()
        {
            if (EnableHeartbeat)
            {
                _session.AddComponent<SessionHeartbeatComponent>().Start(HeartbeatIntervalMs, HeartbeatTimeOutMs,HeartbeatTimeOutIntervalMs,MaxPingSamples);
            }

            _onConnectComplete?.Invoke();
        }

        private void OnConnectFail()
        {
            _session = null;
            Log.Error("[NetworkModule] Gate connect failed.");
            _onConnectFail?.Invoke();
        }

        private void OnConnectDisconnect()
        {
            Log.Warning("[NetworkModule] Gate disconnected.");
            _session = null;
            _onConnectDisconnect?.Invoke();
        }

        public void Disconnect()
        {
            if (_session == null || _session.IsDisposed)
            {
                _session = null;
                return;
            }

            _session.Dispose();
            _session = null;
        }
    }
}
