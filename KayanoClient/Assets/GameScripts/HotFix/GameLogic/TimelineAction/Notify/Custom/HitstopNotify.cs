using System.ComponentModel;

namespace KayanoAction.Runtime
{
    [DisplayName("顿帧")]
    public sealed class HitstopNotify : ActionNotify
    {
        [TimeField(60)]
        public float duration = 0.05f;

        public float animationSpeed;
    }
}
