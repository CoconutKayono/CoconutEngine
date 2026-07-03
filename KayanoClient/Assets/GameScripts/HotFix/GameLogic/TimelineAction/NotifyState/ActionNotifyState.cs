using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// Timeline 区间 NotifyState（仅编辑时配置 + 编辑器预览）。Bake 时数据写入对应的 <see cref="NotifyStateSO"/> 子资产。
    /// </summary>
    public abstract class ActionNotifyState : PlayableAsset, ITimelineClipAsset
    {
        public float start;
        public float length;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ActionNotifyStatePlayable>.Create(graph);
            playable.GetBehaviour().Bind(this, owner);
            return playable;
        }

        public virtual double duration => 0;

        public ClipCaps clipCaps => ClipCaps.None;
    }
}
