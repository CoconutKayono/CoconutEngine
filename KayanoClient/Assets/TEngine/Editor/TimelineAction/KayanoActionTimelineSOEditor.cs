#if UNITY_EDITOR

using KayanoAction.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TEngine.Editor.TimelineAction
{
    [CustomEditor(typeof(ActionTimeline))]
    public sealed class KayanoActionTimelineSOEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            root.Add(KayanoActionTimelineMetadataInspectorElements.Build(serializedObject));
            root.Add(CommandTransitionListInspectorElements.Build(serializedObject));
            root.Add(SignalTransitionListInspectorElements.Build(serializedObject));
            root.Add(new IMGUIContainer(DrawValidationHelpBox));

            root.Bind(serializedObject);
            return root;
        }

        private void DrawValidationHelpBox()
        {
            serializedObject.Update();
            var timeline = (ActionTimeline)target;
            if (timeline.isLoop)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            var finish = timeline.finishTransition;
            if (finish != null && !string.IsNullOrEmpty(finish.actionName))
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "非 loop 动作必须配置「收尾转移 → 目标动作」（Bake 与 Runtime 均不兜底）。",
                MessageType.Error);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
