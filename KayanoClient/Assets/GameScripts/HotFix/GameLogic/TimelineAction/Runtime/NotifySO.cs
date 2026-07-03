using GameLogic;
using UnityEngine;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// 运行时瞬时通知基类。Bake 后作为 KayanoActionRuntimeSO 子资产；
    /// 由 TimelineActionService 按 scheduleTime 调度并调用 <see cref="Notify"/>。
    /// </summary>
    public abstract class NotifySO : ScriptableObject
    {
        /// <summary>调度时间（秒，相对动作起点）。</summary>
        public float scheduleTime;

        public abstract void Notify(in ActionTimelineContext ctx);
    }
}
