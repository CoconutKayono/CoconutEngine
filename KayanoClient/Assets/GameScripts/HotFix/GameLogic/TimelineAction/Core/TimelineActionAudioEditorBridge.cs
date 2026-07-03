#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    /// <summary>
    /// Timeline 音频 Clip 拖放时的 Editor 桥接：统一 Record Undo + SetDirty，
    /// 确保 PlayableAsset、Track、TimelineAsset 三处序列化一致。
    /// </summary>
    internal static class TimelineActionAudioEditorBridge
    {
        public static void MarkDirty(PlayableAsset asset, TimelineClip timelineClip)
        {
            if (asset != null)
            {
                Undo.RecordObject(asset, "Assign Audio Clip");
                EditorUtility.SetDirty(asset);
            }

            var track = timelineClip?.GetParentTrack();
            if (track != null)
            {
                Undo.RecordObject(track, "Assign Audio Clip");
                EditorUtility.SetDirty(track);
            }

            if (track?.timelineAsset != null)
            {
                Undo.RecordObject(track.timelineAsset, "Assign Audio Clip");
                EditorUtility.SetDirty(track.timelineAsset);
            }
        }
    }
}

#endif
