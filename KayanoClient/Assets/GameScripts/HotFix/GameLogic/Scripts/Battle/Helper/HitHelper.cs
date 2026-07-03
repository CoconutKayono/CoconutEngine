using TEngine;
using UnityEngine;

namespace GameLogic
{
    public static class HitHelper
    {
        public static HitConfigData GetHitConfig(int hitId)
        {
            var config = ConfigSystem.Instance.Tables.TbChHitConfig.GetOrDefault(hitId);
            if (config != null)
            {
                return new HitConfigData
                {
                    DamageMult = config.DmgMult,
                    DazeMult = config.DazeMult,
                    Element = config.Element,
                };
            }

            Log.Warning($"[HitHelper] 未找到 HitId: {hitId}，使用默认值（伤害系数=1，失衡系数=1）");
            return new HitConfigData { DamageMult = 1f, DazeMult = 1f };
        }
    }
}