using Cysharp.Threading.Tasks;
using Fantasy.Network;
using System;

namespace TEngine
{
    /// <summary>
    /// 网络模块接口（Fantasy Scene / Session 生命周期，不含业务 RPC）。
    /// 连接参数见 <see cref="NetworkSetting"/>。
    /// </summary>
    public interface INetworkModule
    {

        Session Session { get; }

        /// <summary>
        /// 初始化 Fantasy 框架并创建客户端 Scene，不建立连接。
        /// </summary>
        UniTask InitializeAsync();

        /// <summary>
        /// 连接 Gate 服务器。
        /// </summary>
        UniTask ConnectAsync(Action onConnectComplete = null, Action onConnectFail = null, Action onConnectDisconnect = null);

        /// <summary>
        /// 断开当前 Session，保留 Scene 以便重连。
        /// </summary>
        void Disconnect();
    }
}
