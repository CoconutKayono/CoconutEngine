using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 战斗模块 — 管理失衡值、连携槽等战斗数据
    /// </summary>
    public class BattleStore
    {
        #region States

        /// <summary>单位实例ID → 当前失衡值</summary>
        private Dictionary<int, float> _stunValues = new();

        /// <summary>单位实例ID → 最大失衡值</summary>
        private Dictionary<int, float> _maxStunValues = new();

        /// <summary>单位实例ID → 是否处于失衡状态</summary>
        private Dictionary<int, bool> _stunStates = new();

        /// <summary>队伍共用连携槽（0~1000）</summary>
        private float _chainGauge;

        #endregion

        #region Event

        /// <summary>失衡值变化（实例ID, 当前值）</summary>
        public event System.Action<int, float> OnStunValueChanged;

        /// <summary>失衡状态变化（实例ID, 是否失衡）</summary>
        public event System.Action<int, bool> OnStunStateChanged;

        /// <summary>连携槽变化（当前值）</summary>
        public event System.Action<float> OnChainGaugeChanged;

        /// <summary>连携槽已满</summary>
        public event System.Action OnChainReady;

        #endregion

        #region Getter & Setter

        public float ChainGauge => _chainGauge;
        public bool IsChainReady => _chainGauge >= 1000f;

        public float GetStunValue(int instanceId)
        {
            return _stunValues.TryGetValue(instanceId, out var value) ? value : 0f;
        }

        public float GetMaxStunValue(int instanceId)
        {
            return _maxStunValues.TryGetValue(instanceId, out var value) ? value : 100f;
        }

        public bool IsEnemyStunned(int instanceId)
        {
            return _stunStates.TryGetValue(instanceId, out var stunned) && stunned;
        }

        #endregion

        #region Action（原子操作）

        /// <summary>注册战斗单位</summary>
        public void RegisterUnit(int instanceId, float maxStunValue)
        {
            _stunValues[instanceId] = 0f;
            _maxStunValues[instanceId] = maxStunValue;
            _stunStates[instanceId] = false;
        }

        /// <summary>注销战斗单位</summary>
        public void UnregisterUnit(int instanceId)
        {
            _stunValues.Remove(instanceId);
            _maxStunValues.Remove(instanceId);
            _stunStates.Remove(instanceId);
        }

        /// <summary>重置所有战斗数据</summary>
        public void ResetBattle()
        {
            _stunValues.Clear();
            _maxStunValues.Clear();
            _stunStates.Clear();
            _chainGauge = 0f;
        }

        /// <summary>累加失衡值（达到阈值自动触发失衡状态）</summary>
        public void AddStunValue(int instanceId, float value)
        {
            if (!_stunValues.ContainsKey(instanceId))
            {
                Log.Warning($"[BattleModule] 单位 {instanceId} 未注册");
                return;
            }

            float maxStun = GetMaxStunValue(instanceId);
            float newValue = Mathf.Min(_stunValues[instanceId] + value, maxStun);
            _stunValues[instanceId] = newValue;

            OnStunValueChanged?.Invoke(instanceId, newValue);

            if (newValue >= maxStun && !IsEnemyStunned(instanceId))
            {
                EnterStunState(instanceId);
            }
        }

        /// <summary>退出失衡状态</summary>
        public void ExitStunState(int instanceId)
        {
            if (!_stunStates.ContainsKey(instanceId)) return;

            _stunStates[instanceId] = false;
            _stunValues[instanceId] = 0f;

            OnStunStateChanged?.Invoke(instanceId, false);
            Log.Debug($"[BattleModule] 单位 {instanceId} 退出失衡状态");
        }

        /// <summary>增加连携槽</summary>
        public void AddChainGauge(float value)
        {
            float oldValue = _chainGauge;
            _chainGauge = Mathf.Min(_chainGauge + value, 1000f);

            OnChainGaugeChanged?.Invoke(_chainGauge);

            if (_chainGauge >= 1000f && oldValue < 1000f)
            {
                OnChainReady?.Invoke();
                Log.Debug("[BattleModule] 连携槽已满！");
            }
        }

        /// <summary>消耗连携槽</summary>
        public void ConsumeChainGauge(float amount = 1000f)
        {
            _chainGauge = Mathf.Max(0f, _chainGauge - amount);
            OnChainGaugeChanged?.Invoke(_chainGauge);
        }

        #endregion

        #region Utils

        private void EnterStunState(int instanceId)
        {
            if (!_stunStates.ContainsKey(instanceId)) return;

            _stunStates[instanceId] = true;
            _stunValues[instanceId] = GetMaxStunValue(instanceId);

            OnStunStateChanged?.Invoke(instanceId, true);
            Log.Debug($"[BattleModule] 单位 {instanceId} 进入失衡状态");
        }

        #endregion
    }
}
