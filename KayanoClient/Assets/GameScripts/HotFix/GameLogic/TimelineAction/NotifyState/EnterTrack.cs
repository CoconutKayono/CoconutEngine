using System.ComponentModel;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    [TrackClipType(typeof(ActionNotifyState)), DisplayName("进入轨道")]
    public sealed class EnterTrack : ActionNotifyStateTrack
    {
    }
}
