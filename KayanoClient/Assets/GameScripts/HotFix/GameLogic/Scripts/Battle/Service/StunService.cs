using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 失衡执行服务 — 监听 HitRequestEvent，独立处理失衡值累加
    /// </summary>
    public class StunnService
    {
        public StunnService()
        {
            GameEvent.AddEventListener<HitRequestEvent>(
                IBattleEvents_Event.OnHitRequest,
                OnHitRequest);
        }

        private void OnHitRequest(HitRequestEvent evt)
        {
            var defenderStore = CharacterModule.Instance.GetUnit(evt.TargetId);
            if (defenderStore == null) return;

            var defenderStats = defenderStore.ChAttribute;
            if (!defenderStats.IsAlive) return;

            // 获取招式配置中的失衡系数
            var hitConfig = HitHelper.GetHitConfig(evt.BoxData.HitId);
            float stunValue = defenderStats.GetAttr(400) * hitConfig.DazeMult; // 冲击力 * 失衡系数

            // 累加失衡值
            BattleModule.Instance.Store.AddStunValue(evt.TargetId, stunValue);
        }
    }
}