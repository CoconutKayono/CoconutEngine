using TEngine;

namespace GameLogic
{
    [EventInterface(EEventGroup.GroupLogic)]
    public interface ISettingEvents
    {
        void OnSettingChanged(string key, int value);
    }
}
