using System.ComponentModel;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    [TrackClipType(typeof(BoxNotifyState)), DisplayName("碰撞盒轨道")]
    public sealed class BoxTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
            clip.duration = 0.25f;
            clip.displayName = "碰撞盒";
        }
    }
}
