using System.ComponentModel;
using UnityEngine;

namespace KayanoAction.Runtime
{
    [DisplayName("人声")]
    public sealed class PlayVoiceNotifyState : ActionNotifyState
    {
        public AudioClip clip;
        public AudioClip[] clips;
        public float dontPlayProbability;

        public AudioClip[] EffectiveClips => AudioClipLengthUtility.GetEffectiveClips(clip, clips);

        public override double duration
        {
            get
            {
                if (TryGetPrimaryLength(out var length))
                {
                    return length;
                }

                return base.duration;
            }
        }

        public bool TryGetPrimaryLength(out float length)
        {
            return AudioClipLengthUtility.TryGetPrimaryLength(clip, clips, out length);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            AudioClipLengthUtility.SyncPrimaryClip(ref clip, ref clips);
        }
#endif
    }
}
