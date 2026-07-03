using GameConfig.Main;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 交互服务 — 处理交互意图
    /// TODO：当前无用，因为我们的动画系统没有使用到交互意图，暂时保留
    /// </summary>
    public class InteractService : DecisionServiceBase
    {
        public InteractService() : base(EIntentAction.Interact) { }

        protected override bool CheckCondition(CharacterModule module, ChActionConfig config, IntentEvent intent)
        {
            // 只处理交互类型的动作
            return config.ActionType == EActionType.None;
        }
    }
}