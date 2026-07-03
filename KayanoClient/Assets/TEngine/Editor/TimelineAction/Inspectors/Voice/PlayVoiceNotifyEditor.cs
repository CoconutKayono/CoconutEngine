#if UNITY_EDITOR

using KayanoAction.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TEngine.Editor.TimelineAction
{
    [CustomEditor(typeof(PlayVoiceNotify))]
    public sealed class PlayVoiceNotifyEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(ActionNotifyInspectorElements.BuildDefaultInspector(serializedObject));

            root.Add(ActionNotifyInspectorElements.BuildButtonRow(
                ("预览播放", PreviewSelected)));

            root.Add(ActionNotifyInspectorElements.BuildHelpBox(
                "点击「预览播放」在选中对象上试听；Timeline 内 scrub 时由 NotifyState Playable 预览。"));

            return root;
        }

        private void PreviewSelected()
        {
            var notify = (PlayVoiceNotify)target;
            var owner = Selection.activeGameObject;
            ActionTimelinePreviewRuntime.PreviewVoice(notify.clips, notify.dontPlayProbability, owner);
        }
    }
}

#endif
