using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// 人声轨道（Timeline Track）
    /// 用于在 Timeline 中播放人声/语音片段
    /// </summary>
    [TrackClipType(typeof(PlayVoiceNotifyState))]
    [DisplayName("人声轨道")]
    public sealed class PlayVoiceTrack : ActionNotifyStateTrack
    {
        /// <summary>
        /// 从 AudioClip 创建 Timeline Clip（拖放到轨道时由 Timeline 调用）
        /// </summary>
        public TimelineClip CreateClip(AudioClip audioClip)
        {
            if (audioClip == null)
            {
                return null;
            }

            var newClip = CreateClip<PlayVoiceNotifyState>();

            if (newClip.asset is PlayVoiceNotifyState voice)
            {
                AudioClipLengthUtility.ApplySourceClip(audioClip, ref voice.clip, ref voice.clips);
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

            if (clip.asset is not PlayVoiceNotifyState voice)
            {
                return;
            }

            AudioClipLengthUtility.SyncPrimaryClip(ref voice.clip, ref voice.clips);

            if (voice.TryGetPrimaryLength(out var length))
            {
                clip.duration = length;
                clip.displayName = voice.clip != null ? voice.clip.name : "人声";
            }
            else
            {
                clip.displayName = "人声";
            }

#if UNITY_EDITOR
            TimelineActionAudioEditorBridge.MarkDirty(voice, clip);
#endif
        }
    }
}
