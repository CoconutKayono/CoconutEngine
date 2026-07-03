using System;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// 标记 float 字段为「时间/帧数」输入（Editor Inspector 以指定帧率显示帧 ↔ 秒换算）。
    /// 例如 HitstopNotify.duration、BoxNotifyState.hitstopDuration 使用 [TimeField(60)]。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TimeFieldAttribute : Attribute
    {
        public TimeFieldAttribute(int frameRate = 60)
        {
            FrameRate = frameRate;
        }

        /// <summary>Inspector 帧率基准，默认 60 FPS。</summary>
        public int FrameRate { get; }
    }
}
