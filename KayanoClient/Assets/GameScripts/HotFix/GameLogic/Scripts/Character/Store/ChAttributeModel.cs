using Fantasy;
using System;
using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// 角色属性模型 — HP、能量、失衡等资源与 Luban 属性字典。
    /// </summary>
    public class ChAttributeModel
    {
        #region Attributes
        private int _configId;
        private Dictionary<int, float> _attributes;

        // ---------- 高频读写，独立字段 ----------
        private int _curHp;
        private int _curEnergy;
        private int _curDaze;
        private float _dazeTime;
        private bool _isInvincible;

        // 闪避槽
        private int _curDodgeStamina;
        private int _maxDodgeStamina;

        // 大招能量
        private int _curUltimateEnergy;
        private int _maxUltimateEnergy;

        // 喧响值
        private float _curDecibels;
        private float _maxDecibels;
        #endregion

        #region Events
        public event Action<int, float> OnAttributeChanged;
        public event Action<string, float> OnResourceChanged;
        #endregion

        public ChAttributeModel(int configId, Dictionary<int, float> attributes)
        {
            _configId = configId;
            _attributes = attributes;
        }

        #region Getter, Setter
        public float GetAttr(int attributeId)
        {
            if (!_attributes.TryGetValue(attributeId, out float value))
            {
                Log.Debug($"属性ID {attributeId} 不存在于当前属性字典中");
                return 0;
            }
            return value;
        }

        public void SetAttr(int attributeId, float value)
        {
            _attributes[attributeId] = value;
            OnAttributeChanged?.Invoke(attributeId, value);
        }

        public int Health
        {
            get => _curHp;
            set
            {
                int maxHp = (int)GetAttr(301); // HP_BASE + HP_ADD 等计算
                _curHp = Math.Clamp(value, 0, maxHp);
                OnResourceChanged?.Invoke("Health", _curHp);
            }
        }

        public int CurrentEnergy
        {
            get => _curEnergy;
            set
            {
                _curEnergy = Math.Clamp(value, 0, 150);
                OnResourceChanged?.Invoke("Energy", _curEnergy);
            }
        }

        public int CurrentDaze
        {
            get => _curDaze;
            set
            {
                _curDaze = Math.Clamp(value, 0, 1000);
                OnResourceChanged?.Invoke("Daze", _curDaze);
            }
        }

        public int CurrentDodgeStamina
        {
            get => _curDodgeStamina;
            set
            {
                _curDodgeStamina = Math.Clamp(value, 0, _maxDodgeStamina);
                OnResourceChanged?.Invoke("Dodge", _curDodgeStamina);
            }
        }

        public int CurrentUltimateEnergy
        {
            get => _curUltimateEnergy;
            set
            {
                _curUltimateEnergy = Math.Clamp(value, 0, _maxUltimateEnergy);
                OnResourceChanged?.Invoke("Ultimate", _curUltimateEnergy);
            }
        }

        public float CurrentDecibels
        {
            get => _curDecibels;
            set
            {
                _curDecibels = Math.Clamp(value, 0, _maxDecibels);
                OnResourceChanged?.Invoke("Decibels", _curDecibels);
            }
        }

        public int ConfigId => _configId;
        public bool IsAlive => _curHp > 0;

        public float DazeTime
        {
            get => _dazeTime;
            set => _dazeTime = value;
        }

        public bool IsInvincible
        {
            get => _isInvincible;
            set => _isInvincible = value;
        }

        public int MaxDodgeStamina => _maxDodgeStamina;
        public int MaxUltimateEnergy => _maxUltimateEnergy;
        public float MaxDecibels => _maxDecibels;
        #endregion
    }
}