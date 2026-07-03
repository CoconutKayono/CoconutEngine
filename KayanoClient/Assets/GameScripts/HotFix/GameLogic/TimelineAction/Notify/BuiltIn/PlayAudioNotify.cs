using System.ComponentModel;
using UnityEngine;

namespace KayanoAction.Runtime
{
    [DisplayName("音效")]
    public sealed class PlayAudioNotify : ActionNotify
    {
        public AudioClip clip;
        public AudioClip[] clips;
        public float dontPlayProbability;

        public AudioClip[] EffectiveClips => AudioClipLengthUtility.GetEffectiveClips(clip, clips);

#if UNITY_EDITOR
        private void OnValidate()
        {
            AudioClipLengthUtility.SyncPrimaryClip(ref clip, ref clips);
        }
#endif
    }
}
