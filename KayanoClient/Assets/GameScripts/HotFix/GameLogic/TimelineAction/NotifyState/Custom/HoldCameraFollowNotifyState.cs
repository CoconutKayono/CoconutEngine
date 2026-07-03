using System.ComponentModel;

namespace KayanoAction.Runtime
{
    [DisplayName("锁定相机跟随")]
    [NotifyStateDuration(0.5f)]
    public sealed class HoldCameraFollowNotifyState : ActionNotifyState
    {
    }
}
