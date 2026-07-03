#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TEngine.Editor.TimelineAction
{
    internal static class CommandTransitionListInspectorElements
    {
        public static VisualElement Build(SerializedObject serializedObject)
        {
            var container = new VisualElement();

            var header = new Label("全局指令转移")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginTop = 8f,
                    marginBottom = 4f,
                },
            };
            container.Add(header);

            container.Add(ActionNotifyInspectorElements.BuildHelpBox(
                "指令转移：按键 + 阶段匹配后切换动作，按当前动作（含继承链）查表跳转。"
                + "开启时间 = 相对 Action 启动后的 SessionTime（loop 不回绕），0 表示进 Action 即生效。"
                + "示例：Walk 上配 SpecialAttack + Press，开启时间 0.8s 后切换到 Attack_02。",
                HelpBoxMessageType.Info));

            var listGui = new IMGUIContainer(() => DrawTransitionList(serializedObject))
            {
                style = { marginTop = 4f },
            };
            container.Add(listGui);

            return container;
        }

        private static void DrawTransitionList(SerializedObject serializedObject)
        {
            serializedObject.Update();

            var list = serializedObject.FindProperty("commandTransitions");
            if (list == null)
            {
                EditorGUILayout.HelpBox("未找到 commandTransitions 属性。", MessageType.Error);
                return;
            }

            EditorGUILayout.LabelField("指令转移列表", EditorStyles.boldLabel);

            for (var i = 0; i < list.arraySize; i++)
            {
                var element = list.GetArrayElementAtIndex(i);
                DrawTransitionElement(element, i, list, serializedObject);
            }

            EditorGUILayout.Space(4f);
            if (GUILayout.Button("添加指令转移"))
            {
                list.InsertArrayElementAtIndex(list.arraySize);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawTransitionElement(
            SerializedProperty element,
            int index,
            SerializedProperty list,
            SerializedObject serializedObject)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(
                element.FindPropertyRelative("command"),
                new GUIContent("按键"));

            EditorGUILayout.PropertyField(
                element.FindPropertyRelative("phase"),
                new GUIContent("阶段"));

            EditorGUILayout.PropertyField(
                element.FindPropertyRelative("actionName"),
                new GUIContent("目标动作"));

            EditorGUILayout.PropertyField(
                element.FindPropertyRelative("fadeDuration"),
                new GUIContent("过渡时间(s)"));

            EditorGUILayout.PropertyField(
                element.FindPropertyRelative("openTime"),
                new GUIContent("开启时间(s)"));

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("删除", GUILayout.Width(64f)))
            {
                list.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4f);
        }
    }
}

#endif
