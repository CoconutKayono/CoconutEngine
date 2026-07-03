#if UNITY_EDITOR

using KayanoAction.Runtime;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace TEngine.Editor.TimelineAction
{
    [CustomTimelineEditor(typeof(BoxNotifyState))]
    internal sealed class BoxNotifyStateClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            if (clip.asset is not BoxNotifyState box)
            {
                return;
            }

            clip.displayName = box.boxType.ToString();
        }
    }
}

#endif
