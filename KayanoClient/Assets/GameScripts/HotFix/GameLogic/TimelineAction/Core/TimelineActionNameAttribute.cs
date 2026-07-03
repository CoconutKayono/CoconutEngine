using System;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// 标记字段为「动作名称」下拉选择（Editor 侧通过 Catalog 解析可选动作列表）。
    /// 用于 KayanoActionTimelineSO.inheritActionTransition、KayanoActionRuntimeSO.inheritActionName 等字段。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TimelineActionNameAttribute : Attribute
    {
    }
}
