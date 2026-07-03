using System.Collections.Generic;
using TEngine;

namespace GameLogic
{
    /// <summary>
    /// 角色模块（全局单例）
    /// 职责：管理所有 CharacterStore 实例的注册、注销和查询。
    /// </summary>
    public class CharacterModule : Singleton<CharacterModule>
    {
        #region States

        private readonly Dictionary<int, CharacterStore> _allUnits = new();

        private readonly List<int> _enemyIds = new();

        private readonly List<int> _playerIds = new();

        private readonly List<DecisionServiceBase> _decisionServices = new();

        #endregion

        #region 生命周期

        protected override void OnInit()
        {
            base.OnInit();

            // 注册个人动作相关的决策服务
            _decisionServices.Add(new WalkService());
            _decisionServices.Add(new SprintService());
            _decisionServices.Add(new IdleService());
            _decisionServices.Add(new TurnBackService());
            _decisionServices.Add(new FrontDodgeService());
            _decisionServices.Add(new BackDodgeService());
            _decisionServices.Add(new InteractService());

            // 队伍切换服务由 TeamModule 管理，不在此注册
        }

        protected override void OnRelease()
        {
            foreach (var service in _decisionServices)
            {
                service?.Dispose();
            }

            _decisionServices.Clear();
            Clear();

            base.OnRelease();
        }

        #endregion

        #region 注册 / 注销

        /// <summary>
        /// 注册单位
        /// </summary>
        /// <param name="store">角色数据</param>
        /// <param name="unitType">单位类型（Player / Enemy / Summon）</param>
        public void RegisterUnit(CharacterStore store, EUnitType unitType)
        {
            if (store == null)
            {
                Log.Error("[CharacterModule] RegisterUnit 失败：store 为空");
                return;
            }

            int instanceId = store.InstanceId;

            if (_allUnits.ContainsKey(instanceId))
            {
                Log.Warning($"[CharacterModule] 单位 {instanceId} 已注册，将覆盖");
                UnregisterUnit(instanceId);
            }

            _allUnits[instanceId] = store;

            switch (unitType)
            {
                case EUnitType.Player:
                    _playerIds.Add(instanceId);
                    break;
                case EUnitType.Enemy:
                case EUnitType.Summon:
                    _enemyIds.Add(instanceId);
                    break;
            }
        }

        /// <summary>
        /// 注销单位
        /// </summary>
        public void UnregisterUnit(int instanceId)
        {
            _allUnits.Remove(instanceId);
            _enemyIds.Remove(instanceId);
            _playerIds.Remove(instanceId);
        }

        #endregion

        #region 查询

        /// <summary>
        /// 根据 InstanceId 获取单位
        /// </summary>
        public CharacterStore GetUnit(int instanceId)
        {
            return _allUnits.TryGetValue(instanceId, out var store) ? store : null;
        }

        /// <summary>
        /// 获取所有敌人
        /// </summary>
        public List<CharacterStore> GetAllEnemies()
        {
            var result = new List<CharacterStore>(_enemyIds.Count);
            foreach (var id in _enemyIds)
            {
                if (_allUnits.TryGetValue(id, out var store))
                {
                    result.Add(store);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有玩家
        /// </summary>
        public List<CharacterStore> GetAllPlayers()
        {
            var result = new List<CharacterStore>(_playerIds.Count);
            foreach (var id in _playerIds)
            {
                if (_allUnits.TryGetValue(id, out var store))
                {
                    result.Add(store);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有单位
        /// </summary>
        public List<CharacterStore> GetAllUnits()
        {
            return new List<CharacterStore>(_allUnits.Values);
        }

        #endregion

        #region 属性

        /// <summary>单位总数</summary>
        public int TotalCount => _allUnits.Count;

        /// <summary>敌人数</summary>
        public int EnemyCount => _enemyIds.Count;

        /// <summary>玩家数</summary>
        public int PlayerCount => _playerIds.Count;

        #endregion

        #region 清理

        /// <summary>
        /// 清空所有注册数据
        /// </summary>
        public void Clear()
        {
            _allUnits.Clear();
            _enemyIds.Clear();
            _playerIds.Clear();
        }

        #endregion
    }
}