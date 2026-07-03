using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 普通切换服务 — 处理队伍切换（左链招/右链招）
    /// </summary>
    public class SwitchInService : DecisionServiceBase
    {
        private CharacterStore _cachedTarget;

        public SwitchInService() : base(EIntentAction.Chain) { }

        protected override bool CheckCondition(CharacterStore store, ChActionConfig config, IntentEvent intent)
        {
            var target = GetTargetMember(intent.ChainDir);
            if (target == null || target.InstanceId == store.InstanceId)
            {
                _cachedTarget = null;
                return false;
            }

            if (target.ChActionState.GetFirstConfigByType(EActionType.SwitchIn) == null)
            {
                Log.Warning($"[SwitchInService] 目标角色 {target.InstanceId} 没有配置 SwitchIn");
                _cachedTarget = null;
                return false;
            }

            _cachedTarget = target;
            return true;
        }

        protected override ExecutableIntent? CreateExecutableIntent(
            CharacterStore store,
            int actionId,
            CommandTransitionInfo info,
            ChActionConfig config,
            IntentEvent intent)
        {
            var targetStore = _cachedTarget;
            if (targetStore == null)
            {
                Log.Warning($"[SwitchInService] 目标角色为空，请检查 CheckCondition");
                return null;
            }

            var switchInConfig = targetStore.ChActionState.GetFirstConfigByType(EActionType.SwitchIn);
            if (switchInConfig == null)
            {
                Log.Warning($"[SwitchInService] 目标角色 {targetStore.InstanceId} 没有 SwitchIn 配置");
                return null;
            }

            string targetActionName = switchInConfig.Name;
            if (string.IsNullOrEmpty(targetActionName))
            {
                Log.Warning($"[SwitchInService] 目标角色 {targetStore.InstanceId} SwitchIn 缺少动作名称");
                return null;
            }

            if (!targetStore.ChActionState.TryGetActionId(targetActionName, out _))
            {
                Log.Warning($"[SwitchInService] 目标角色 {targetStore.InstanceId} 表现动作 '{targetActionName}' 不存在");
                return null;
            }

            return new ExecutableIntent
            {
                InstanceId = store.InstanceId,
                ActionId = actionId,
                ActionName = info.actionName,
                LogicType = config.ActionType,
                Priority = switchInConfig.Priority,
                Phase = intent.Phase,
                HoldTime = intent.HoldTime,
                Direction = intent.Direction,
                ChainDir = intent.ChainDir,

                // 目标角色执行 SwitchIn 入场攻击
                memberInstanceId = targetStore.InstanceId,
                memberActionName = targetActionName,
                memberEActionType = EActionType.SwitchIn,

                // 资源消耗
                EnergyCost = config.EnergyCost,
                DecibelCost = config.DecibelCost,
                ChainGaugeCost = config.ChainGaugeCost,
                DodgeStaminaCost = config.DodgeStaminaCost,
            };
        }

        /// <summary>
        /// 根据切换方向获取目标角色
        /// </summary>
        private CharacterStore GetTargetMember(EChainDirection dir)
        {
            return dir == EChainDirection.Next
                ? TeamModule.Instance.GetNextMember()
                : TeamModule.Instance.GetPreviousMember();
        }
    }
}