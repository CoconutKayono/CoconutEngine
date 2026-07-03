using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    [TrackClipType(typeof(PlayAudioNotifyState))]
    [DisplayName("音效轨道")]
    public sealed class PlayAudioTrack : ActionNotifyStateTrack
    {
        public TimelineClip CreateClip(AudioClip audioClip)
        {
            if (audioClip == null)
            {
                return null;
            }

            var newClip = CreateClip<PlayAudioNotifyState>();
            if (newClip.asset is PlayAudioNotifyState audio)
            {
                AudioClipLengthUtility.ApplySourceClip(audioClip, ref audio.clip, ref audio.clips);
            }

            newClip.duration = audioClip.length;
            newClip.displayName = audioClip.name;

#if UNITY_EDITOR
            TimelineActionAudioEditorBridge.MarkDirty(newClip.asset as PlayableAsset, newClip);
#endif

            return newClip;
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);

            if (clip.asset is not PlayAudioNotifyState audio)
            {
                return;
            }

            AudioClipLengthUtility.SyncPrimaryClip(ref audio.clip, ref audio.clips);

            if (audio.TryGetPrimaryLength(out var length))
            {
                clip.duration = length;
                clip.displayName = audio.clip != null ? audio.clip.name : "音效";
            }
            else
            {
                clip.displayName = "音效";
            }

#if UNITY_EDITOR
            TimelineActionAudioEditorBridge.MarkDirty(audio, clip);
#endif
        }
    }
}
