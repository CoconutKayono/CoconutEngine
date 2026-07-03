using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 输入缓冲数据 — 窗口期内的离散输入队列。
    /// </summary>
    public class InputBufferModel
    {
        #region 数据结构

        public struct Entry
        {
            public EIntentAction Action;
            public EInputPhase Phase;
            public float HoldTime;
            public Vector2 Direction;
            public EChainDirection ChainDir;
            public float Time;

            public IntentEvent ToIntentEvent(int instanceId)
            {
                return new IntentEvent
                {
                    InstanceId = instanceId,
                    Action = Action,
                    Phase = Phase,
                    HoldTime = HoldTime,
                    Direction = Direction,
                    ChainDir = ChainDir,
                };
            }

            public static Entry FromSnapshot(
                EIntentAction action,
                EInputPhase phase,
                in InputStateModel state,
                EChainDirection chainDir = default)
            {
                return new Entry
                {
                    Action = action,
                    Phase = phase,
                    HoldTime = state.GetHoldTime(action),
                    Direction = action == EIntentAction.Move ? state.Move : Vector2.zero,
                    ChainDir = chainDir,
                };
            }
        }

        #endregion

        #region States

        private readonly Queue<Entry> _entries = new();
        private float _duration = 0.1f;

        #endregion

        #region Actions

        public void Push(Entry entry)
        {
            entry.Time = Time.time;
            _entries.Enqueue(entry);
        }

        public bool Pop(out Entry entry)
        {
            PurgeExpired();
            if (_entries.Count == 0)
            {
                entry = default;
                return false;
            }

            entry = _entries.Dequeue();
            return true;
        }

        public void Clear() => _entries.Clear();

        private void PurgeExpired()
        {
            while (_entries.Count > 0 && Time.time - _entries.Peek().Time > _duration)
            {
                _entries.Dequeue();
            }
        }

        #endregion

        #region Getter & Setter

        public float Duration
        {
            get => _duration;
            set => _duration = value;
        }

        #endregion
    }
}
