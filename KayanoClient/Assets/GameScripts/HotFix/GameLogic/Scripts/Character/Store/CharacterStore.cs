using GameConfig.Main;
using KayanoAction.Runtime;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class CharacterStore
    {
        #region States
        private int _instanceId = 0;
        private ChAttributeModel _attribute;
        private ChActionModel _actionState;
        private ChStateModel _state;
        private Animator _animator;
        private Transform _owner;
        private EUnitType _unitType;
        private ChMotionConfig _chMotionConfig;
        #endregion

        #region Constructor
        public CharacterStore(
            EUnitType unitType,
            int configId,
            List<TimelineActionSO> actions,
            string defaultActionName,
            Animator animator,
            Transform owner)
        {
            _instanceId = InstanceIdGenerator.Create();
            _animator = animator;
            _owner = owner;
            _unitType = unitType;

            var attrDic = _unitType switch
            {
                EUnitType.Player => LoadPlayerAttributes(configId),
                EUnitType.Enemy => LoadMonsterAttributes(configId),
                _ => new Dictionary<int, float>(),
            };

            var ChMotionConfig = LoadMotionConfig(configId);

            _attribute = new ChAttributeModel(configId, attrDic);
            _actionState = new ChActionModel(actions, defaultActionName);
            _state = new ChStateModel();
        }
        #endregion


        #region Getter,Setter
        public int InstanceId => _instanceId;
        public int ConfigId => _attribute.ConfigId;
        public ChAttributeModel ChAttribute => _attribute;
        public ChActionModel ChActionState => _actionState;
        public ChStateModel ChState => _state;
        public Animator Animator => _animator;
        public Transform Owner => _owner;
        public EUnitType UnitType => _unitType;

        public ChMotionConfig ChMotionConfig => _chMotionConfig;

        public float RotationSpeed => _chMotionConfig.RotationSpeed;

        public float WalkToRunTime => _chMotionConfig.WalkToRunTime;

        public float TurnThreshold => _chMotionConfig.TurnThreshold;
        #endregion

        #region 读取属性配置表
        private Dictionary<int, float> LoadPlayerAttributes(int characterId)
        {
            var attrConfig = ConfigSystem.Instance.Tables.TbChAttrConfig.GetOrDefault(characterId);
            if (attrConfig == null)
            {
                Log.Error($"找不到角色配置：ConfigId = {characterId}");
                return new Dictionary<int, float>();
            }

            var attrList = attrConfig.BaseAttr;
            var attrDic = new Dictionary<int, float>(attrList.Count);

            for (int i = 0; i < attrList.Count; i++)
            {
                var attr = attrList[i];
                attrDic[attr.AttrType] = attr.Value;
            }

            return attrDic;
        }

        private Dictionary<int, float> LoadMonsterAttributes(int characterId)
        {
            // Todo: 从配置表加载怪物属性
            return new Dictionary<int, float>();
        }

        #endregion

        #region 读取角色Motion配置表 
        private ChMotionConfig LoadMotionConfig(int characterId)
        {
            var config = ConfigSystem.Instance.Tables.TbChMotionConfig.GetOrDefault(characterId);
            if (config == null)
            {
                Log.Warning($"[CharacterModule] 角色 {characterId} 未配置 MotionConfig");
                return null;
            }
            return config;
        }
        #endregion
    }
}
