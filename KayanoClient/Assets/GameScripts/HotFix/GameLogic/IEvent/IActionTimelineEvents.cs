using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// Timeline Notify / NotifyState 运行时事件：由 TimelineAction 子类发布，各域 Listener 订阅。
    /// </summary>
    [EventInterface(EEventGroup.GroupLogic)]
    public interface IActionTimelineEvents
    {
        #region 物理碰撞盒
        void OnBoxNotifyBegin(BoxNotifyBeginEvent evt);
        void OnBoxNotifyEnd(BoxNotifyEndEvent evt);
        #endregion

        #region 战斗动作（攻击、闪避、弹刀辅助）
        void OnAttackingBegin(TimelineActorEvent evt);
        void OnAttackingEnd(TimelineActorEvent evt);
        void OnDodgeBegin(TimelineActorEvent evt);
        void OnDodgeEnd(TimelineActorEvent evt);
        void OnParryAidBegin(TimelineActorEvent evt);
        void OnParryAidEnd(TimelineActorEvent evt);
        #endregion

        #region 资源消耗与恢复
        void OnConsumeStamina(ConsumeStaminaEvent evt);
        void OnRestoreSwitchCharge(RestoreSwitchChargeEvent evt);
        #endregion

        #region 特效、音效、语音
        void OnPlayParticle(PlayParticleEvent evt);
        void OnPlayAudio(PlayAudioEvent evt);
        void OnPlayVoice(PlayVoiceEvent evt);
        #endregion

        #region 打击反馈（卡肉、攻击提示）
        void OnHitstop(HitstopEvent evt);
        void OnAttackTip(AttackTipEvent evt);
        #endregion

        #region 相机控制
        void OnCameraShake(CameraShakeEvent evt);
        void OnAssistCamera(AssistCameraEvent evt);
        void OnHoldCameraFollow(HoldCameraFollowEvent evt);
        #endregion

        #region 信号与过渡（Signal / Intent）
        void OnSignalTransition(SignalTransitionEvent evt);
        void OnIntentTransitionEnter(IntentTransitionEvent evt);
        void OnIntentTransitionExit(IntentTransitionEvent evt);
        #endregion
    }

    #region 物理碰撞盒
    public struct BoxNotifyBeginEvent
    {
        public ActionTimelineContext Context;
        public BoxData Box;
        public int StateInstanceId;
    }

    public struct BoxNotifyEndEvent
    {
        public ActionTimelineContext Context;
        public BoxData Box;
        public int StateInstanceId;
    }
    #endregion

    #region 战斗动作（通用）
    public struct TimelineActorEvent
    {
        public ActionTimelineContext Context;
        public int StateInstanceId;
    }
    #endregion

    #region 资源消耗与恢复
    public struct ConsumeStaminaEvent
    {
        public ActionTimelineContext Context;
        public float Amount;
    }

    public struct RestoreSwitchChargeEvent
    {
        public ActionTimelineContext Context;
        public int Amount;
    }
    #endregion

    #region 特效、音效、语音
    public struct PlayParticleEvent
    {
        public ActionTimelineContext Context;
        public GameObject Prefab;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
        public int StateInstanceId;
        public bool IsStateEnd;
    }

    public struct PlayAudioEvent
    {
        public ActionTimelineContext Context;
        public string[] Paths;
        public float DontPlayProbability;
        public TEngine.AudioType audioType;
    }

    public struct PlayVoiceEvent
    {
        public ActionTimelineContext Context;
        public string[] Paths;
        public float DontPlayProbability;
        public TEngine.AudioType audioType;
    }
    #endregion

    #region 打击反馈（卡肉、攻击提示）
    public struct HitstopEvent
    {
        public ActionTimelineContext Context;
        public float Duration;
        public float AnimationSpeed;
    }

    public struct AttackTipEvent
    {
        public ActionTimelineContext Context;
        public bool CanParry;
    }
    #endregion

    #region 相机控制
    public struct CameraShakeEvent
    {
        public ActionTimelineContext Context;
        public float Angle;
        public float Speed;
    }

    public struct AssistCameraEvent
    {
        public ActionTimelineContext Context;
        public bool Enter;
        public int StateInstanceId;
    }

    public enum HoldCameraFollowPhase
    {
        Begin,
        Tick,
        End,
    }

    public struct HoldCameraFollowEvent
    {
        public ActionTimelineContext Context;
        public HoldCameraFollowPhase Phase;
        public int StateInstanceId;
    }
    #endregion

    #region 信号与过渡（Signal / Intent）
    public struct SignalTransitionEvent
    {
        public ActionTimelineContext Context;
        public SignalTransitionInfo TransitionInfo;
    }

    /// <summary>
    /// 指令过渡事件（由 Timeline 在指令窗口开启/关闭时发送）
    /// </summary>
    public struct IntentTransitionEvent
    {
        public ActionTimelineContext Context;
        public CommandTransitionInfo TransitionInfo;
        public int StateInstanceId;
    }
    #endregion
}