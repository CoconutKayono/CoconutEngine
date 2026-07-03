using System;
using UnityEngine;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// 动作收尾转移（clip 播完时切换到的目标动作）。
    /// </summary>
    [Serializable]
    public class TransitionInfo
    {
        [TimelineActionName]
        [InspectorName("目标动作")]
        public string actionName;

        [InspectorName("过渡时间")]
        public float fadeDuration = 0.15f;
    }
}
