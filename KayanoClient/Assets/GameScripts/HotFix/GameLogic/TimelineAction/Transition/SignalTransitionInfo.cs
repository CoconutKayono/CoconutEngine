using System;
using UnityEngine;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// 外部 Signal 转移（Combat / 物理等程序逻辑触发，非玩家 Intent）。
    /// </summary>
    [Serializable]
    public class SignalTransitionInfo
    {
        [TimelineActionName]
        [InspectorName("目标动作")]
        public string actionName;

        [InspectorName("过渡时间")]
        public float fadeDuration = 0.15f;

        [InspectorName("信号名")]
        [Tooltip("Combat 调用 ActionSignalDispatch.TrySignal 时传入的名称，如 ReflectBullet。")]
        public string signalName;

        /// <summary>
        /// 全局转移开启时间（秒，相对 SessionTime；0 = 进 Action 即开启）。
        /// </summary>
        [InspectorName("开启时间")]
        [Tooltip("秒：0=进 Action 即有效；loop 不重置 SessionTime。")]
        public float openTime;

        public bool MatchesSignal(string signal)
        {
            return !string.IsNullOrEmpty(signalName)
                && string.Equals(signalName, signal, StringComparison.Ordinal);
        }
    }
}
