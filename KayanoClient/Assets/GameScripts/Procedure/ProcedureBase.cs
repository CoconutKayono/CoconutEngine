using TEngine;

namespace Procedure
{
    /// <summary>
    /// Procedure 流程基类。
    /// <para>──────────────── Kayano 启动流程总览（入口：ProcedureLaunch）────────────────</para>
    /// <para>ProcedureLaunch → 启动器（Launcher / 语言 / 音效）</para>
    /// <para>    ↓</para>
    /// <para>ProcedureSplash → 闪屏展示</para>
    /// <para>    ↓</para>
    /// <para>ProcedureInitPackage → 初始化 YooAsset Package 运行时</para>
    /// <para>    ↓</para>
    /// <para>ProcedureInitResources → 请求远程版本号并更新资源 Manifest</para>
    /// <para>    ↓ HostPlayMode 且有待下载补丁</para>
    /// <para>ProcedureCreateDownloader → 创建资源下载器，统计补丁大小</para>
    /// <para>    ↓</para>
    /// <para>ProcedureDownloadFile → 下载补丁文件（AB / DLL.bytes 等）</para>
    /// <para>    ↓</para>
    /// <para>ProcedureDownloadOver → 下载完成，写入本地版本号</para>
    /// <para>    ↓ 可选</para>
    /// <para>ProcedureClearCache → 清理未使用的 YooAsset 缓存</para>
    /// <para>    ↓</para>
    /// <para>ProcedurePreload → 预加载 PRELOAD 标签资源</para>
    /// <para>    ↓</para>
    /// <para>ProcedureLoadAssembly → HybridCLR 加载热更 DLL + AOT 元数据，进入 GameApp</para>
    /// <para>    ↓</para>
    /// <para>ProcedureStartGame → 关闭 Launcher UI，热更逻辑接管</para>
    /// <para>──────────────── 分支说明 ────────────────</para>
    /// <para>• EditorSimulateMode / OfflinePlayMode：InitResources 后直接 → Preload</para>
    /// <para>• WebPlayMode / 边玩边下载：InitResources 后跳过下载 → Preload</para>
    /// <para>• HostPlayMode：Manifest 对比有差异 → CreateDownloader 链路，无差异 → Preload</para>
    /// <para>──────────────── 术语表 ────────────────</para>
    /// <para>• Procedure（流程）：TEngine 有限状态机中的一个启动阶段，OnEnter 进入、ChangeState 切换。</para>
    /// <para>• Package（资源包）：YooAsset 管理的 AB 集合；本项目默认名为 DefaultPackage。</para>
    /// <para>• Manifest（资源清单）：记录 Package 版本号与全部资源文件的元数据（*.version / *.bytes）。</para>
    /// <para>• EPlayMode（运行模式）：EditorSimulateMode 编辑器模拟；OfflinePlayMode 纯本地；HostPlayMode 内置+CDN 热更；WebPlayMode WebGL 远程。</para>
    /// <para>• StreamingAssets：Player 内置资源目录，首包 Manifest 与 AB 落在此处（package/DefaultPackage/）。</para>
    /// <para>• CDN / 资源服务器：HostPlayMode 远程下载地址，由 UpdateSetting.ResDownLoadPath 配置。</para>
    /// <para>• Patch（补丁）：本地 Manifest 与远程对比后，需要增量下载的文件集合。</para>
    /// <para>• Downloader（下载器）：YooAsset ResourceDownloaderOperation，负责批量下载补丁。</para>
    /// <para>• PRELOAD（预加载标签）：打在 YooAsset 资源上的 Label，ProcedurePreload 按标签提前 Load 进内存。</para>
    /// <para>• HybridCLR（热更新）：IL2CPP 运行时动态加载 C# 程序集；热更 DLL 以 .bytes 打进 AB。</para>
    /// <para>• HotUpdateAssemblies：热更程序集列表（GameProto.dll、GameLogic.dll），见 UpdateSetting。</para>
    /// <para>• AOT Metadata（AOT 元数据）：HybridCLR 为裁剪后的 AOT DLL 补充泛型元数据，非热更 DLL 本身。</para>
    /// <para>• Launcher：启动阶段 UI 框架（LoadUpdateUI 进度条 / 弹窗），热更完成后由 ProcedureStartGame 关闭。</para>
    /// <para>• GameApp.Entrance：热更域入口，ProcedureLoadAssembly 加载完 DLL 后反射调用，启动 GameLogic。</para>
    /// </summary>
    public abstract class ProcedureBase : TEngine.ProcedureBase
    {
        /// <summary>
        /// 获取流程是否使用原生对话框
        /// 在一些特殊的流程（如游戏逻辑对话框资源更新完成前的流程）中，可以考虑调用原生对话框进行消息提示行为
        /// </summary>
        public abstract bool UseNativeDialog { get; }
        
        protected readonly IResourceModule _resourceModule = ModuleSystem.GetModule<IResourceModule>();
    }
}