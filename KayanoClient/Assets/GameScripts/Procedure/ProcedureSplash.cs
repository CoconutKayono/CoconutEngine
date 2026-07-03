using UnityEngine;
using ProcedureOwner = TEngine.IFsm<TEngine.IProcedureModule>;

namespace Procedure
{
    /// <summary>
    /// 流程 => 闪屏。
    /// <para>【流程位置】Launch 之后，资源热更链路之前。</para>
    /// <para>【职责】展示 Splash 闪屏（可扩展动画与时长）；当前实现为立即跳转。</para>
    /// <para>【下一流程】→ ProcedureInitPackage。</para>
    /// <para>【术语】Splash：游戏启动品牌/Logo 展示页，与 Launcher 进度 UI 不同。</para>
    /// </summary>
    public class ProcedureSplash : ProcedureBase
    {
        public override bool UseNativeDialog => true;

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            // 播放 Splash 动画
            //Splash.Active(splashTime:3f);
            //初始化资源包
            ChangeState<ProcedureInitPackage>(procedureOwner);
        }
    }
}
