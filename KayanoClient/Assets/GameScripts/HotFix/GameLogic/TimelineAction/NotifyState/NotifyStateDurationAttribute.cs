using System;

namespace KayanoAction.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NotifyStateDurationAttribute : Attribute
    {
        public NotifyStateDurationAttribute(float duration)
        {
            Duration = duration;
        }

        public float Duration { get; }
    }
}
