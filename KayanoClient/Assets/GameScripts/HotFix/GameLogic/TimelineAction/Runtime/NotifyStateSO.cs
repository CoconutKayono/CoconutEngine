using GameLogic;
using UnityEngine;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// 运行时区间通知基类。Bake 后作为 KayanoActionRuntimeSO 子资产；
    /// 由 TimelineActionService 在 [start, start + length) 区间调用 <see cref="Enter"/> / <see cref="Exit"/>。
    /// </summary>
    public abstract class NotifyStateSO : ScriptableObject
    {
        /// <summary>区间起始时间（秒，相对动作起点）。</summary>
        public float start;

        /// <summary>区间持续时长（秒）。</summary>
        public float length;

        public abstract void Enter(in ActionTimelineContext ctx, int instanceId);

        public abstract void Exit(in ActionTimelineContext ctx, int instanceId);
    }
}
