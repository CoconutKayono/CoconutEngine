using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 动作仲裁服务 — 从可执行意图中选出优先级最高的最终意图
    /// </summary>
    public class ActionArbitrationService
    {
        #region States
        private CharacterModule _characterModule;
        private ActionStateModel _actionState;
        #endregion

        #region Constructor
        public ActionArbitrationService(CharacterModule characterModule)
        {
            _characterModule = characterModule;
            _actionState = characterModule.ActionState;
        }
        #endregion

        #region Tick
        /// <summary>
        /// 收集可执行意图，选择优先级最高的意图作为最终动作，并发送给事件系统。
        /// </summary>
        public void Tick()
        {
            var list = _actionState.PendingExecutableIntents;

            if (list == null || list.Count == 0)
            {
                return;
            }

            int finalIndex = GetMaxPriorityIndex(list);

            var final = list[finalIndex];

            _actionState.ClearPendingExecutableIntents();

            GameEvent.Get<IActionIntentEvents>().OnFinalIntent(new FinalIntentEvent
            {
                InstanceId = final.InstanceId,
                ActionId = final.ActionId,
                ActionName = final.ActionName,
                Phase = final.Phase,
                HoldTime = final.HoldTime,
                Direction = final.Direction,
                ChainDir = final.ChainDir,
                EnergyCost = final.EnergyCost,
                DecibelCost = final.DecibelCost,
                ChainGaugeCost = final.ChainGaugeCost,
                DodgeStaminaCost = final.DodgeStaminaCost,
            });
        }
        #endregion

        private int GetMaxPriorityIndex(IReadOnlyList<ExecutableIntent> intents)
        {
            if (intents == null || intents.Count == 0) return -1;

            int maxIndex = 0;
            int maxPriority = int.MinValue;

            for (int i = 0; i < intents.Count; i++)
            {
                if (intents[i].Priority < maxPriority)  // 数值越小优先级越高
                {
                    maxPriority = intents[i].Priority;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        public void Dispose()
        {
            _characterModule = null;
            _actionState = null;
        }
    }
}