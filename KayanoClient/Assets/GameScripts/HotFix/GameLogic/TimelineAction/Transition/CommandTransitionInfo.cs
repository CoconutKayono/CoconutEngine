using System;
using GameLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KayanoAction.Runtime
{
    [Serializable]
    public class CommandTransitionInfo
    {
        [TimelineActionName]
        [InspectorName("目标动作")]
        public string actionName;

        [InspectorName("过渡时间")]
        public float fadeDuration = 0.15f;

        [FormerlySerializedAs("intent")]
        [InspectorName("按键")]
        public EIntentAction command;

        [InspectorName("阶段")]
        public EInputPhase phase = EInputPhase.Press;

        /// <summary>
        /// 全局转移开启时间（秒，相对本 Action 的 SessionTime；0 = 进 Action 即开启；loop 不回绕）。
        /// </summary>
        [InspectorName("开启时间")]
        [Tooltip("秒：0=进 Action 即有效；1.5=SessionTime≥1.5s 才进入 Available。loop 不重置 SessionTime。")]
        public float openTime;

        public bool Check(EIntentAction command, EInputPhase phase)
        {
            return this.command == command && this.phase == phase;
        }
    }
}
