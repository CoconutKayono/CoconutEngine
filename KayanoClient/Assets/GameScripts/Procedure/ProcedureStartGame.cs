using System;
using Cysharp.Threading.Tasks;
using Launcher;
using TEngine;

namespace Procedure
{
    /// <summary>
    /// 流程 => 启动游戏。
    /// <para>【流程位置】LoadAssembly 之后，启动链路的最后一步。</para>
    /// <para>【职责】关闭全部 Launcher UI（LoadUpdateUI 等）；热更 GameApp 已在 LoadAssembly 中通过 Entrance 启动。</para>
    /// <para>【下一流程】无（流程结束，由热更 GameLogic 接管：UI / 登录 / Gameplay）。</para>
    /// <para>【术语】GameApp.Entrance：热更入口，注册 LoginUI 桥接并打开 MainMenuUI；Launcher：仅启动阶段使用。</para>
    /// </summary>
    public class ProcedureStartGame : ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        protected override void OnEnter(IFsm<IProcedureModule> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            StartGame().Forget();
        }

        private async UniTaskVoid StartGame()
        {
            await UniTask.Yield();
            LauncherMgr.HideAllUI();
        }
    }
}