using System.Collections.Generic;
using TEngine;

namespace GameLogic
{
    /// <summary>
    /// 房间模块 — 管理当前房间/区域内的所有战斗单位
    /// </summary>
    public class CharacterManager : Singleton<CharacterManager>
    {
        #region States
        private Dictionary<int, CharacterModule> _allUnits = new();

        private List<int> _enemyIds = new();

        private List<int> _playerIds = new();

        #endregion

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            Clear();
        }
        #region Getters & Setter

        /// <summary>
        /// 注册单位到房间
        /// </summary>
        /// <param name="module">角色模块</param>
        /// <param name="unitType">单位类型（Player / Enemy / Summon）</param>
        public void RegisterUnit(CharacterModule module, EUnitType unitType)
        {
            if (module == null)
            {
                Log.Error("[CharacterManager] RegisterUnit 失败：module 为空");
                return;
            }

            int instanceId = module.InstanceId;

            if (_allUnits.ContainsKey(instanceId))
            {
                Log.Warning($"[CharacterManager] 单位 {instanceId} 已注册，将覆盖");
                UnregisterUnit(instanceId);
            }

            _allUnits[instanceId] = module;

            switch (unitType)
            {
                case EUnitType.Player:
                    _playerIds.Add(instanceId);
                    break;
                case EUnitType.Enemy:
                    _enemyIds.Add(instanceId);
                    break;
                case EUnitType.Summon:
                    _enemyIds.Add(instanceId);
                    break;
            }
        }

        /// <summary>
        /// 从房间注销单位
        /// </summary>
        public void UnregisterUnit(int instanceId)
        {
            _allUnits.Remove(instanceId);
            _enemyIds.Remove(instanceId);
            _playerIds.Remove(instanceId);
        }

        /// <summary>
        /// 根据 InstanceId 获取单位模块
        /// </summary>
        public CharacterModule GetUnit(int instanceId)
        {
            return _allUnits.TryGetValue(instanceId, out var module) ? module : null;
        }

        /// <summary>
        /// 获取所有敌人模块
        /// </summary>
        public List<CharacterModule> GetAllEnemies()
        {
            var result = new List<CharacterModule>(_enemyIds.Count);
            foreach (var id in _enemyIds)
            {
                if (_allUnits.TryGetValue(id, out var module))
                {
                    result.Add(module);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有玩家模块
        /// </summary>
        public List<CharacterModule> GetAllPlayers()
        {
            var result = new List<CharacterModule>(_playerIds.Count);
            foreach (var id in _playerIds)
            {
                if (_allUnits.TryGetValue(id, out var module))
                {
                    result.Add(module);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有单位模块
        /// </summary>
        public List<CharacterModule> GetAllUnits()
        {
            return new List<CharacterModule>(_allUnits.Values);
        }

        /// <summary>
        /// 获取当前房间内单位总数
        /// </summary>
        public int TotalCount => _allUnits.Count;

        /// <summary>
        /// 获取当前房间内敌人数
        /// </summary>
        public int EnemyCount => _enemyIds.Count;

        /// <summary>
        /// 获取当前房间内玩家数
        /// </summary>
        public int PlayerCount => _playerIds.Count;

        /// <summary>
        /// 清空所有注册数据（场景切换/战斗结束时调用）
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