using System.Collections.Generic;
using GameConfig.Main;
using KayanoAction.Runtime;
using TEngine;

namespace GameLogic
{
    /// <summary>
    /// 动作数据模型 — 静态配置数据，构造后不变
    /// 
    /// 内部维护两套索引：
    ///   1. _actionDic：按动作名 → TimelineActionSO（表现资产）
    ///   2. _typeToActionsCache：按 EActionType → List&lt;ChActionConfig&gt;（逻辑配置）
    /// </summary>
    public class ChActionDataModel
    {
        #region 私有字段

        /// <summary>表现动作字典：动作名 → TimelineActionSO</summary>
        private readonly Dictionary<string, TimelineActionSO> _actionDic;

        /// <summary>逻辑配置缓存：EActionType → List&lt;ChActionConfig&gt;</summary>
        private readonly Dictionary<EActionType, List<ChActionConfig>> _typeToActionsCache;

        #endregion

        #region 构造

        public ChActionDataModel(List<TimelineActionSO> actions)
        {
            // ===== 构建表现动作字典（按动作名索引） =====
            _actionDic = new Dictionary<string, TimelineActionSO>();

            foreach (var action in actions)
            {
                if (_actionDic.ContainsKey(action.actionName))
                {
                    Log.Error($"表现动作名称 '{action.actionName}' 重复，将覆盖之前的条目");
                }
                _actionDic[action.actionName] = action;
            }

            // ===== 构建逻辑配置缓存（按 EActionType 索引） =====
            _typeToActionsCache = new Dictionary<EActionType, List<ChActionConfig>>();

            foreach (var kvp in _actionDic)
            {
                var config = ConfigSystem.Instance.Tables.TbChActionConfig.GetOrDefault(kvp.Value.actionId);
                if (config == null)
                {
                    continue;
                }

                if (!_typeToActionsCache.TryGetValue(config.ActionType, out var list))
                {
                    list = new List<ChActionConfig>();
                    _typeToActionsCache[config.ActionType] = list;
                }
                list.Add(config);
            }
        }

        #endregion

        #region 表现动作查询（按动作名，返回 TimelineActionSO）

        /// <summary>
        /// 尝试获取指定名称的表现动作资产
        /// </summary>
        public bool TryGetAction(string actionName, out TimelineActionSO action)
            => _actionDic.TryGetValue(actionName, out action);

        /// <summary>
        /// 尝试获取指定名称的表现动作 ID
        /// </summary>
        public bool TryGetActionId(string actionName, out int actionId)
        {
            if (_actionDic.TryGetValue(actionName, out var action))
            {
                actionId = action.actionId;
                return true;
            }

            actionId = -1;
            return false;
        }

        /// <summary>
        /// 获取所有可用表现动作名称列表
        /// </summary>
        public List<string> GetAvailableActionNames()
            => new List<string>(_actionDic.Keys);

        /// <summary>
        /// 检查是否存在指定名称的表现动作
        /// </summary>
        public bool HasAction(string actionName)
            => _actionDic.ContainsKey(actionName);

        #endregion

        #region 逻辑配置查询（按 EActionType，返回 ChActionConfig）

        /// <summary>
        /// 根据逻辑动作类型获取该类型下的所有配置列表（只读）
        /// </summary>
        public IReadOnlyList<ChActionConfig> GetConfigsByType(EActionType targetType)
        {
            if (_typeToActionsCache.TryGetValue(targetType, out var list))
            {
                return list;
            }

            return System.Array.Empty<ChActionConfig>();
        }

        /// <summary>
        /// 根据逻辑动作类型获取该类型下的第一个配置
        /// </summary>
        public ChActionConfig GetFirstConfigByType(EActionType targetType)
        {
            var list = GetConfigsByType(targetType);
            return list.Count > 0 ? list[0] : null;
        }

        #endregion

        #region 属性

        /// <summary>表现动作字典（按动作名索引 TimelineActionSO）</summary>
        public Dictionary<string, TimelineActionSO> ActionDic => _actionDic;

        #endregion
    }
}