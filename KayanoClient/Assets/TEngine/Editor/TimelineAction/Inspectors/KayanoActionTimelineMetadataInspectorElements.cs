#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TEngine.Editor.TimelineAction
{
    internal static class KayanoActionTimelineMetadataInspectorElements
    {
        public static VisualElement Build(SerializedObject serializedObject)
        {
            return new IMGUIContainer(() => DrawMetadata(serializedObject));
        }

        private static void DrawMetadata(SerializedObject serializedObject)
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("m_Script"),
                    new GUIContent("脚本"));
            }

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("m_DurationMode"),
                new GUIContent("时长模式"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("actionId"),
                new GUIContent("动作 ID"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("primaryIntent"),
                new GUIContent("主意图"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("isLoop"),
                new GUIContent("循环播放"));

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("动作参数", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("enableRotation"),
                new GUIContent("允许移动旋转"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("enableRecenter"),
                new GUIContent("允许相机回中"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("enableLookAtMonster"),
                new GUIContent("允许面向最近敌人"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("inheritActionTransition"),
                new GUIContent("继承转移来源"));

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("收尾转移", EditorStyles.boldLabel);

            var finish = serializedObject.FindProperty("finishTransition");
            EditorGUILayout.PropertyField(
                finish.FindPropertyRelative("actionName"),
                new GUIContent("目标动作"));
            EditorGUILayout.PropertyField(
                finish.FindPropertyRelative("fadeDuration"),
                new GUIContent("过渡时间"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
