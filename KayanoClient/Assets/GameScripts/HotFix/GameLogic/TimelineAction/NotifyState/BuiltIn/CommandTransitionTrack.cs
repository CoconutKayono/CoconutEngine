using System.ComponentModel;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    [TrackClipType(typeof(CommandTransitionNotifyState)), DisplayName("指令转移轨道")]
    public sealed class CommandTransitionTrack : ActionNotifyStateTrack
    {
    }
}
