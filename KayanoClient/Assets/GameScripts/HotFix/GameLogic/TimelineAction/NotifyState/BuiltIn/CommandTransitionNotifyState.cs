using GameLogic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

namespace KayanoAction.Runtime
{
    [DisplayName("指令转移窗口")]
    public sealed class CommandTransitionNotifyState : ActionNotifyState
    {
        [InspectorName("目标动作")]
        public string actionName;

        [InspectorName("过渡时间")]
        public float fadeDuration = 0.15f;

        [FormerlySerializedAs("intent")]
        [InspectorName("按键")]
        public EIntentAction command;

        [InspectorName("阶段")]
        public EInputPhase phase = EInputPhase.Press;

        [InspectorName("开启时间")]
        public float openTime;

        public CommandTransitionInfo ToTransitionInfo()
        {
            return new CommandTransitionInfo
            {
                actionName = actionName,
                fadeDuration = fadeDuration,
                command = command,
                phase = phase,
                openTime = openTime,
            };
        }
    }
}
