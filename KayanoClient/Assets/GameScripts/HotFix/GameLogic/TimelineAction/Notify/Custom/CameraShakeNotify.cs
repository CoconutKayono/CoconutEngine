using System.ComponentModel;

namespace KayanoAction.Runtime
{
    [DisplayName("相机震动")]
    public sealed class CameraShakeNotify : ActionNotify
    {
        public float angle = 1f;
        public float speed = 1f;
    }
}
