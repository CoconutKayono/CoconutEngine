using GameLogic;
using TEngine;

namespace KayanoAction.Runtime
{
    public sealed class PlayAudioNotifySO : NotifySO
    {
        public string[] paths;
        public float dontPlayProbability;
        public AudioType audioType = AudioType.Sound;

        public override void Notify(in ActionTimelineContext ctx)
        {
            GameEvent.Get<IActionTimelineEvents>().OnPlayAudio(new PlayAudioEvent
            {
                Context = ctx,
                Paths = paths,
                DontPlayProbability = dontPlayProbability,
                audioType = audioType,
            });
        }
    }
}
