using Launcher;
using TEngine;
using UnityEngine;
using ProcedureOwner = TEngine.IFsm<TEngine.IProcedureModule>;

namespace Procedure
{
    /// <summary>
    /// 流程 => 下载完成。
    /// <para>【流程位置】DownloadFile 成功之后。</para>
    /// <para>【职责】将当前 PackageVersion 写入 PlayerPrefs（GAME_VERSION），供下次可选更新/离线兜底。</para>
    /// <para>【下一流程】需清缓存 → ClearCache；否则 → Preload。</para>
    /// <para>【术语】GAME_VERSION：本地已成功的资源版本记录，与 StreamingAssets 内置版本、CDN 远程版本三者需协调。</para>
    /// </summary>
    public class ProcedureDownloadOver : ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        private bool _needClearCache;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Log.Info("下载完成!!!");

            LauncherMgr.ShowUI<LoadUpdateUI>($"下载完成...");

            // 下载完成之后再保存本地版本。
            Utility.PlayerPrefs.SetString("GAME_VERSION", _resourceModule.PackageVersion);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            if (_needClearCache)
            {
                ChangeState<ProcedureClearCache>(procedureOwner);
            }
            else
            {
                ChangeState<ProcedurePreload>(procedureOwner);
            }
        }
    }
}