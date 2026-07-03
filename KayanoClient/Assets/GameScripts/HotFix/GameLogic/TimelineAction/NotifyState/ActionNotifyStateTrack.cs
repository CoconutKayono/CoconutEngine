using System.ComponentModel;
using System.Reflection;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    [TrackClipType(typeof(ActionNotifyState)), DisplayName("通知状态轨道")]
    public class ActionNotifyStateTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);

            var state = (ActionNotifyState)clip.asset;
            var durationAttr = state.GetType().GetCustomAttribute<NotifyStateDurationAttribute>();
            clip.duration = durationAttr?.Duration ?? 0.5f;

            var nameAttr = state.GetType().GetCustomAttribute<DisplayNameAttribute>();
            if (nameAttr != null)
            {
                clip.displayName = nameAttr.DisplayName;
            }
        }
    }
}
