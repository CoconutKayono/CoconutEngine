using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class PlayVoiceNotifyStateSO : NotifyStateSO
    {
        public string[] paths;
        public float dontPlayProbability;
        public AudioType audioType = AudioType.Voice;

        public override void Enter(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnPlayVoice(new PlayVoiceEvent
            {
                Context = ctx,
                Paths = paths,
                DontPlayProbability = dontPlayProbability,
                audioType = audioType,
            });
        }

        public override void Exit(in ActionTimelineContext ctx, int instanceId)
        {
        }
    }
}
