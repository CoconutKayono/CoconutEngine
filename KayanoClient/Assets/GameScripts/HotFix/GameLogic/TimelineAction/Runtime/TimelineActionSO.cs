using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// Bake 后的纯动作数据（ActionModule 执行层唯一读取源；不含运行时逻辑）。
    /// </summary>
    [CreateAssetMenu(fileName = "KayanoActionRuntime", menuName = "Kayano/Action Runtime")]
    public sealed class TimelineActionSO : ScriptableObject
    {
        public int actionId;

        /// <summary>查表键与 Animator State 名（烘焙时由 Timeline 名去 Catalog 前缀，如 Idle）。</summary>
        public string actionName;

        public EIntentAction primaryIntent;

        public bool isLoop;

        /// <summary>允许根据移动输入旋转角色根节点（Demo ActionArgs.enableRotation）。</summary>
        public bool enableRotation;

        /// <summary>侧移时相机水平回中（Demo ActionArgs.enableRecenter）。</summary>
        public bool enableRecenter;

        /// <summary>攻击段面向最近敌人（Demo ActionArgs.enableLookAtMonster）。</summary>
        public bool enableLookAtMonster;

        public AnimationClip clip;

        [TimelineActionName]
        public string inheritActionName;

        public TransitionInfo finishTransition;

        [FormerlySerializedAs("intentTransitions")]
        public List<CommandTransitionInfo> commandTransitions = new();

        public List<SignalTransitionInfo> signalTransitions = new();

        /// <summary>瞬时 Notify 子资产引用（Bake 时从 Timeline Marker 写入 NotifySO）。</summary>
        public List<NotifySO> notifies = new();

        /// <summary>区间 NotifyState 子资产引用（Bake 时从 Timeline Clip 写入 NotifyStateSO）。</summary>
        public List<NotifyStateSO> notifyStates = new();
    }
}
