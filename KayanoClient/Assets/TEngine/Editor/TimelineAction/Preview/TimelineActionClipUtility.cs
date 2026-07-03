#if UNITY_EDITOR

using KayanoAction.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TEngine.Editor.TimelineAction
{
    internal static class TimelineActionClipUtility
    {
        public static bool TryFindClip(
            PlayableAsset asset,
            out TimelineClip timelineClip,
            out TimelineAsset timelineAsset)
        {
            timelineClip = null;
            timelineAsset = null;
            if (asset == null)
            {
                return false;
            }

            var guids = AssetDatabase.FindAssets("t:TimelineAsset");
            for (var i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var timeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(path);
                if (timeline == null)
                {
                    continue;
                }

                foreach (var track in timeline.GetOutputTracks())
                {
                    foreach (var clip in track.GetClips())
                    {
                        if (clip.asset == asset)
                        {
                            timelineClip = clip;
                            timelineAsset = timeline;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool TrySyncClipDuration(PlayableAsset asset, float duration)
        {
            if (!TryFindClip(asset, out var clip, out _))
            {
                return false;
            }

            if (duration <= 0f)
            {
                return false;
            }

            clip.duration = duration;
            var track = clip.GetParentTrack();
            if (track != null)
            {
                EditorUtility.SetDirty(track);
            }

            if (track?.timelineAsset != null)
            {
                EditorUtility.SetDirty(track.timelineAsset);
            }

            if (asset != null)
            {
                EditorUtility.SetDirty(asset);
            }

            return true;
        }

        public static void AssignAudioClip(PlayableAsset asset, AudioClip audioClip, TimelineClip timelineClip)
        {
            if (asset == null || audioClip == null)
            {
                return;
            }

            switch (asset)
            {
                case PlayAudioNotifyState audio:
                    audio.clip = audioClip;
                    audio.clips = new[] { audioClip };
                    break;
                case PlayVoiceNotifyState voice:
                    voice.clip = audioClip;
                    voice.clips = new[] { audioClip };
                    break;
                default:
                    return;
            }

            if (timelineClip != null)
            {
                timelineClip.duration = audioClip.length;
                timelineClip.displayName = audioClip.name;
            }

            MarkTimelineAudioDirty(asset, timelineClip);
        }

        public static void MarkTimelineAudioDirty(PlayableAsset asset, TimelineClip timelineClip = null)
        {
            if (asset != null)
            {
                EditorUtility.SetDirty(asset);
            }

            var track = timelineClip?.GetParentTrack();
            if (track != null)
            {
                EditorUtility.SetDirty(track);
            }

            if (track?.timelineAsset != null)
            {
                EditorUtility.SetDirty(track.timelineAsset);
            }
        }

        public static GameObject ResolvePreviewOwner(PlayableAsset asset)
        {
            if (TryFindClip(asset, out _, out var timeline))
            {
                var directors = Object.FindObjectsByType<PlayableDirector>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
                for (var i = 0; i < directors.Length; i++)
                {
                    if (directors[i].playableAsset == timeline)
                    {
                        return directors[i].gameObject;
                    }
                }
            }

            if (Selection.activeGameObject != null)
            {
                return Selection.activeGameObject;
            }

            return null;
        }
    }
}

#endif
