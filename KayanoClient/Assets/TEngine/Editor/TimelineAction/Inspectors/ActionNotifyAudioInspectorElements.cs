#if UNITY_EDITOR

using KayanoAction.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace TEngine.Editor.TimelineAction
{
    internal static class ActionNotifyAudioInspectorElements
    {
        public static VisualElement BuildAudioClipSection(
            SerializedObject serializedObject,
            string clipPropertyName = "clip",
            string clipsPropertyName = "clips")
        {
            var clipProperty = serializedObject.FindProperty(clipPropertyName);
            var clipsProperty = serializedObject.FindProperty(clipsPropertyName);

            var section = new VisualElement();

            var clipField = new PropertyField(clipProperty, "Source Audio Clip");
            clipField.RegisterValueChangeCallback(_ => SyncAndMatchLength(serializedObject, clipProperty, clipsProperty));
            section.Add(clipField);

            var clipsField = new PropertyField(clipsProperty, "Clips");
            clipsField.RegisterValueChangeCallback(_ => SyncAndMatchLength(serializedObject, clipProperty, clipsProperty));
            section.Add(clipsField);

            section.Add(ActionNotifyInspectorElements.BuildButtonRow(
                ("From Audio Clip", () => ApplyFromSourceClip(serializedObject, clipProperty, clipsProperty, showDialog: true))));

            return section;
        }

        public static bool ApplyFromSourceClip(
            SerializedObject serializedObject,
            SerializedProperty clipProperty,
            SerializedProperty clipsProperty,
            bool showDialog = false)
        {
            serializedObject.Update();

            var source = clipProperty.objectReferenceValue as AudioClip;
            if (source == null)
            {
                if (showDialog)
                {
                    EditorUtility.DisplayDialog(ActionBakePaths.DialogTitle, "请先在 Source Audio Clip 指定音频。", "确定");
                }

                return false;
            }

            clipsProperty.arraySize = 1;
            clipsProperty.GetArrayElementAtIndex(0).objectReferenceValue = source;
            clipProperty.objectReferenceValue = source;

            Undo.RecordObject(serializedObject.targetObject, "From Audio Clip");
            serializedObject.ApplyModifiedProperties();
            MarkNotifyStateDirty(serializedObject.targetObject);

            if (TryGetPrimaryLength(source, out var length))
            {
                TimelineActionClipUtility.TrySyncClipDuration(serializedObject.targetObject as PlayableAsset, length);
            }

            if (showDialog)
            {
                EditorUtility.DisplayDialog(
                    ActionBakePaths.DialogTitle,
                    $"已载入 {source.name} 到 Clips。",
                    "确定");
            }

            return true;
        }

        private static void SyncAndMatchLength(
            SerializedObject serializedObject,
            SerializedProperty clipProperty,
            SerializedProperty clipsProperty)
        {
            serializedObject.Update();
            SyncClipFields(clipProperty, clipsProperty);
            serializedObject.ApplyModifiedProperties();
            MarkNotifyStateDirty(serializedObject.targetObject);

            if (serializedObject.targetObject is PlayAudioNotifyState audio
                && audio.TryGetPrimaryLength(out var length))
            {
                TimelineActionClipUtility.TrySyncClipDuration(audio, length);
                return;
            }

            if (serializedObject.targetObject is PlayVoiceNotifyState voice
                && voice.TryGetPrimaryLength(out length))
            {
                TimelineActionClipUtility.TrySyncClipDuration(voice, length);
            }
        }

        private static void SyncClipFields(SerializedProperty clipProperty, SerializedProperty clipsProperty)
        {
            var clip = clipProperty.objectReferenceValue as AudioClip;

            if (clip != null)
            {
                if (clipsProperty.arraySize != 1
                    || clipsProperty.GetArrayElementAtIndex(0).objectReferenceValue != clip)
                {
                    clipsProperty.arraySize = 1;
                    clipsProperty.GetArrayElementAtIndex(0).objectReferenceValue = clip;
                }

                return;
            }

            if (clipsProperty.arraySize > 0)
            {
                AudioClip primary = null;
                for (var i = 0; i < clipsProperty.arraySize; i++)
                {
                    primary = clipsProperty.GetArrayElementAtIndex(i).objectReferenceValue as AudioClip;
                    if (primary != null)
                    {
                        break;
                    }
                }

                clipProperty.objectReferenceValue = primary;
            }
        }

        private static bool TryGetPrimaryLength(AudioClip clip, out float length)
        {
            length = clip != null ? clip.length : 0f;
            return clip != null && length > 0f;
        }

        internal static void MarkNotifyStateDirty(Object target)
        {
            if (target == null)
            {
                return;
            }

            EditorUtility.SetDirty(target);
        }
    }
}

#endif
