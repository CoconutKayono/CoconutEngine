#if UNITY_EDITOR

using KayanoAction.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TEngine.Editor.TimelineAction
{
    [CustomEditor(typeof(PlayVoiceNotifyState))]
    public sealed class PlayVoiceNotifyStateEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(ActionNotifyAudioInspectorElements.BuildAudioClipSection(serializedObject));
            root.Add(ActionNotifyInspectorElements.BuildDefaultInspector(serializedObject, "clip", "clips"));

            root.TrackSerializedObjectValue(serializedObject, _ => TryAutoMatchClipLength());

            root.Add(ActionNotifyInspectorElements.BuildButtonRow(
                ("匹配音频长度", () => TryAutoMatchClipLength(showDialog: true)),
                ("预览播放", PreviewSelected)));

            root.Add(ActionNotifyInspectorElements.BuildHelpBox(
                "可从 Project 将 AudioClip 拖到「人声轨道」，或指定 Source Audio Clip 后点 From Audio Clip。"));

            return root;
        }

        private void TryAutoMatchClipLength(bool showDialog = false)
        {
            var state = (PlayVoiceNotifyState)target;
            if (!state.TryGetPrimaryLength(out var length))
            {
                if (showDialog)
                {
                    EditorUtility.DisplayDialog(ActionBakePaths.DialogTitle, "请先指定 AudioClip。", "确定");
                }

                return;
            }

            if (TimelineActionClipUtility.TrySyncClipDuration(state, length))
            {
                if (showDialog)
                {
                    EditorUtility.DisplayDialog(
                        ActionBakePaths.DialogTitle,
                        $"已同步 Clip 长度 = {length:F3}s",
                        "确定");
                }
            }
            else if (showDialog)
            {
                EditorUtility.DisplayDialog(
                    ActionBakePaths.DialogTitle,
                    "未找到引用此资产的 Timeline Clip。请在 Timeline 中选中对应 Clip。",
                    "确定");
            }
        }

        private void PreviewSelected()
        {
            var state = (PlayVoiceNotifyState)target;
            var owner = TimelineActionClipUtility.ResolvePreviewOwner(state);
            ActionTimelinePreviewRuntime.PreviewVoice(state.EffectiveClips, state.dontPlayProbability, owner);
        }
    }
}

#endif
