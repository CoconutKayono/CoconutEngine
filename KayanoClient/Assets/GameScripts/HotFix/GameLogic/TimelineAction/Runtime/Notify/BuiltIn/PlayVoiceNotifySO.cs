using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class PlayVoiceNotifySO : NotifySO
    {
        public string[] paths;
        public float dontPlayProbability;
        public AudioType audioType = AudioType.Voice;

        public override void Notify(in ActionTimelineContext ctx)
        {
            GameEvent.Get<IActionTimelineEvents>().OnPlayVoice(new PlayVoiceEvent
            {
                Context = ctx,
                Paths = paths,
                DontPlayProbability = dontPlayProbability,
                audioType = audioType,
            });
        }
    }
}
