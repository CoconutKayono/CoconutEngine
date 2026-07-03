using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    [CreateAssetMenu(fileName = "KayanoActionTimeline", menuName = "Kayano/Action Timeline")]
    public sealed class ActionTimeline : TimelineAsset
    {
        [InspectorName("动作 ID")]
        public int actionId;

        /// <summary>
        /// 招式主意图（与 Scripts/Character/Enum/EIntentAction 一致，用于 Move 阻断等规则）。
        /// </summary>
        [InspectorName("主意图")]
        public EIntentAction primaryIntent;

        [InspectorName("循环播放")]
        public bool isLoop;

        [InspectorName("允许移动旋转")]
        public bool enableRotation;

        [InspectorName("允许相机回中")]
        public bool enableRecenter;

        [InspectorName("允许面向最近敌人")]
        [Tooltip("攻击段软索敌：自动转向最近 CombatTarget（Demo enableLookAtMonster）。")]
        public bool enableLookAtMonster;

        [TimelineActionName]
        [InspectorName("继承转移来源")]
        public string inheritActionTransition;

        [InspectorName("收尾转移")]
        [Tooltip("非 loop 时必填：clip 播完时的收尾转移（Bake 校验；Runtime 不兜底）。")]
        public TransitionInfo finishTransition;

        [FormerlySerializedAs("intentTransitions")]
        public List<CommandTransitionInfo> commandTransitions = new();

        [InspectorName("信号转移")]
        [Tooltip("外部 Signal（Combat/物理）触发的动作转移；由 ActionSignalDispatch.TrySignal 消费。")]
        public List<SignalTransitionInfo> signalTransitions = new();
    }
}
