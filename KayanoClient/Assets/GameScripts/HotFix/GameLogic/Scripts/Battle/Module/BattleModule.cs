using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 战斗模块 — 组装战斗数据与战斗服务
    /// </summary>
    public class BattleModule : Singleton<BattleModule>
    {
        #region States

        private BattleStore _store;
        private DamageService _damageService;
        private StunnService _stunService;
        private EnergyExecutionService _energyService;
        private HitDetectionService _hitDetectionService;

        private NormalAttackService _normalAttackService;
        private SpecialAttackService _specialAttackService;
        private DashAttackService _dashAttackService;
        private UltimateService _ultimateService;

        #endregion

        public BattleStore Store => _store;

        protected override void OnInit()
        {
            base.OnInit();
            _store = new BattleStore();

            _damageService = new DamageService();
            _stunService = new StunnService();
            _energyService = new EnergyExecutionService();
            _hitDetectionService = new HitDetectionService(LayerMask.GetMask("Enemy"));

            _normalAttackService = new NormalAttackService();
            _specialAttackService = new SpecialAttackService();
            _dashAttackService = new DashAttackService();
            _ultimateService = new UltimateService();
        }

        protected override void OnRelease()
        {
            _normalAttackService?.Dispose();
            _specialAttackService?.Dispose();
            _dashAttackService?.Dispose();
            _ultimateService?.Dispose();

            _hitDetectionService?.Dispose();

            _normalAttackService = null;
            _specialAttackService = null;
            _dashAttackService = null;
            _ultimateService = null;
            _damageService = null;
            _stunService = null;
            _energyService = null;
            _hitDetectionService = null;

            _store?.ResetBattle();
            _store = null;
            base.OnRelease();
        }
    }
}
