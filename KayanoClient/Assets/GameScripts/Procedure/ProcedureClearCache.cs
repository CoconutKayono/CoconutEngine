using Launcher;
using TEngine;
using ProcedureOwner = TEngine.IFsm<TEngine.IProcedureModule>;

namespace Procedure
{
    /// <summary>
    /// 流程 => 清理缓存。
    /// <para>【流程位置】DownloadOver 之后（可选分支，_needClearCache 为 true 时）。</para>
    /// <para>【职责】ClearCacheFilesAsync 清理 YooAsset 未使用的沙盒缓存文件，释放磁盘空间。</para>
    /// <para>【下一流程】→ ProcedurePreload。</para>
    /// <para>【术语】沙盒缓存：YooAsset 下载到 persistentDataPath 的 AB/DLL 副本，与 StreamingAssets 内置包不同。</para>
    /// </summary>
    public class ProcedureClearCache : ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        private ProcedureOwner _procedureOwner;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            _procedureOwner = procedureOwner;
            Log.Info("清理未使用的缓存文件！");

            LauncherMgr.ShowUI<LoadUpdateUI>($"清理未使用的缓存文件...");

            var operation = _resourceModule.ClearCacheFilesAsync();
            operation.Completed += Operation_Completed;
        }


        private void Operation_Completed(YooAsset.AsyncOperationBase obj)
        {
            LauncherMgr.ShowUI<LoadUpdateUI>($"清理完成 即将进入游戏...");

            ChangeState<ProcedurePreload>(_procedureOwner);
        }
    }
}