using System.Collections.Generic;
using TEngine;

namespace GameLogic
{
    /// <summary>
    /// 动作队列模型 — 待仲裁的可执行意图队列
    /// </summary>
    public class ChActionQueueModel
    {
        #region States

        /// <summary>
        /// 待仲裁的可执行意图列表（按添加顺序存储）
        /// </summary>
        private readonly List<ExecutableIntent> _pendingExecutableIntents = new(32);

        /// <summary>
        /// 已添加的 ActionId 查找表（用于 O(1) 去重）
        /// </summary>
        private readonly HashSet<int> _pendingLookup = new();

        #endregion

        #region Public API

        /// <summary>
        /// 检查指定 ActionId 是否已在队列中
        /// </summary>
        public bool HasPendingIntent(int actionId)
        {
            return _pendingLookup.Contains(actionId);
        }

        /// <summary>
        /// 添加可执行意图到队列（自动去重）
        /// </summary>
        public void AddPendingExecutableIntent(ExecutableIntent intent)
        {
            // ActionId 是唯一标识，必须有效
            if (intent.ActionId <= 0)
            {
                Log.Warning("[ChActionQueueModel] ActionId 无效，拒绝添加");
                return;
            }

            // HashSet 去重：已存在则跳过
            if (!_pendingLookup.Add(intent.ActionId))
            {
                return;
            }

            _pendingExecutableIntents.Add(intent);
        }

        /// <summary>
        /// 清空队列（仲裁层消费后调用）
        /// </summary>
        public void Clear()
        {
            _pendingExecutableIntents.Clear();
            _pendingLookup.Clear();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 待仲裁的可执行意图列表（只读）
        /// </summary>
        public IReadOnlyList<ExecutableIntent> PendingExecutableIntents => _pendingExecutableIntents;

        #endregion
    }
}