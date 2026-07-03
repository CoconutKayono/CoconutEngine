using GameConfig.Main;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    [EventInterface(EEventGroup.GroupLogic)]
    public interface IActionIntentEvents
    {
        void OnNoneIntent(IntentEvent evt);
        void OnMoveIntent(IntentEvent evt);
        void OnAttackIntent(IntentEvent evt);
        void OnDodgeIntent(IntentEvent evt);
        void OnSpecialAttackIntent(IntentEvent evt);
        void OnUltimateIntent(IntentEvent evt);
        void OnSprintIntent(IntentEvent evt);
        void OnInteractIntent(IntentEvent evt);
        void OnChainIntent(IntentEvent evt);
        void OnFinalIntent(FinalIntentEvent evt);
    }

    public struct IntentEvent
    {
        public int InstanceId;
        public EIntentAction Action;      // 意图类型：Move, Attack, Dodge...
        public EInputPhase Phase;         // 输入相位：Down, Press, Up
        public float HoldTime;            // 按住时长（秒）
        public Vector2 Direction;         // 世界空间方向（主要用于 Move/Sprint/Dodge 等）
        public EChainDirection ChainDir;  // 链招方向（仅 Chain 使用）
    }

    public struct FinalIntentEvent
    {
        public int InstanceId;
        public int ActionId;
        public string ActionName;
        public EActionType ActionType;
        public EInputPhase Phase;
        public float HoldTime;
        public Vector2 Direction;
        public EChainDirection ChainDir;

        // 资源消耗字段（从配置表读取，由各 Service 填充）
        public float EnergyCost;
        public float DecibelCost;
        public float ChainGaugeCost;
        public float DodgeStaminaCost;


    }
}