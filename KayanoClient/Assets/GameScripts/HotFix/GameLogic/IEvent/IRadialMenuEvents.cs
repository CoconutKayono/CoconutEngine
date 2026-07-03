using TEngine;

namespace GameLogic
{
    /// <summary>
    /// 扇形转盘扇区被点击时广播，各业务 Module 订阅处理。
    /// </summary>
    [EventInterface(EEventGroup.GroupLogic)]
    public interface IRadialMenuEvents
    {
        void OnSectorSelected(RadialMenuSectorEvent evt);
    }

    public struct RadialMenuSectorEvent
    {
        public int Index;
    }
}
