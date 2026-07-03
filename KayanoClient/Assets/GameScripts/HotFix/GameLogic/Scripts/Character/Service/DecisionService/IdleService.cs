using GameConfig.Main;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// Idle 服务 — 处理无输入意图
    /// </summary>
    public class IdleService : DecisionServiceBase
    {
        public IdleService() : base(EIntentAction.None) { }

        protected override bool CheckCondition(CharacterModule module, ChActionConfig config, IntentEvent intent)
        {
            // Idle 没有特殊的 ActionType 过滤，但保持结构一致
            return config.ActionType == EActionType.Idle;
        }
    }
}