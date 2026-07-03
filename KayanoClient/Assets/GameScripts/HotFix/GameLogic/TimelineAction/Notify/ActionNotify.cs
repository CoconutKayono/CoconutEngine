using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// Timeline 瞬时 Marker（仅编辑时配置）。Bake 时数据写入对应的 <see cref="NotifySO"/> 子资产。
    /// </summary>
    public abstract class ActionNotify : Marker
    {
        /// <summary>Bake 写入的调度时间（秒）。</summary>
        public float scheduleTime;
    }
}
