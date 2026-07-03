using GameConfig.Main;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 移动服务 — 处理移动意图，包含方向转换和转身判定
    /// </summary>
    public class MoveService : DecisionServiceBase
    {
        private const float TURN_BACK_ANGLE_THRESHOLD = 120f;

        public MoveService() : base(EIntentAction.Move) { }

        protected override bool CheckCondition(CharacterModule module, ChActionConfig config, IntentEvent intent)
        {
            return true;
        }

        // 转身逻辑需要在外部处理，但基类无法处理复杂的转身判定。
        // 建议：转身由单独的 TurnBackService 或 MoveService 在外部额外处理。
        // 或者重写 OnIntent 完全自定义处理。
    }
}