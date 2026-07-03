using System.ComponentModel;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    [TrackClipType(typeof(PlayParticleNotifyState)), DisplayName("粒子轨道")]
    public sealed class PlayParticleTrack : ActionNotifyStateTrack
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
            clip.displayName = "粒子";
        }
    }
}
