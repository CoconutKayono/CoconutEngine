#if UNITY_EDITOR

using KayanoAction.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TEngine.Editor.TimelineAction
{
    /// <summary>
    /// PlayParticleNotifyState 的自定义 Inspector 编辑器
    /// 提供粒子特效的预览功能，方便策划/美术在编辑器中查看特效效果
    /// </summary>
    [CustomEditor(typeof(PlayParticleNotifyState))]
    public sealed class PlayParticleNotifyStateEditor : UnityEditor.Editor
    {
        private GameObject _previewInstance;  // 当前预览的粒子实例
        private Button _stopButton;           // 停止预览按钮的引用

        /// <summary>
        /// 编辑器禁用时自动清理预览实例
        /// </summary>
        private void OnDisable()
        {
            StopPreview();
        }

        /// <summary>
        /// 构建自定义 Inspector UI（使用 UI Toolkit）
        /// </summary>
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            // 添加默认的属性字段（prefab、localPosition、localRotation 等）
            root.Add(ActionNotifyInspectorElements.BuildDefaultInspector(serializedObject));

            // ============================================
            // 预览控制按钮
            // ============================================

            // 预览播放按钮
            var previewButton = new Button(PreviewSelected) { text = "预览播放" };
            previewButton.style.flexGrow = 1f;

            // 停止预览按钮（初始禁用）
            _stopButton = new Button(StopPreview) { text = "停止预览" };
            _stopButton.style.flexGrow = 1f;
            _stopButton.style.marginLeft = 4f;
            _stopButton.SetEnabled(false);

            // 将按钮放入水平行布局
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    marginTop = 8f,
                },
            };
            row.Add(previewButton);
            row.Add(_stopButton);
            root.Add(row);

            // 添加使用说明提示框
            root.Add(ActionNotifyInspectorElements.BuildHelpBox(
                "Timeline 预览时：进入 Clip 生成特效，离开 Clip 自动销毁。"));

            return root;
        }

        /// <summary>
        /// 预览选中的粒子特效
        /// 在场景中实例化粒子预制体，并播放特效
        /// </summary>
        private void PreviewSelected()
        {
            // 先停止之前的预览
            StopPreview();

            // 获取当前编辑的 NotifyState
            var state = (PlayParticleNotifyState)target;

            // 获取预览的父对象（用于定位特效位置）
            var owner = TimelineActionClipUtility.ResolvePreviewOwner(state);

            _previewInstance = ActionTimelinePreviewRuntime.PreviewParticleBegin(
                state.prefab,          // 粒子预制体
                state.localPosition,   // 本地位置偏移
                state.localRotation,   // 本地旋转
                state.localScale,      // 本地缩放
                owner);                // 父对象

            // 预览成功则启用停止按钮
            _stopButton?.SetEnabled(_previewInstance != null);
        }

        /// <summary>
        /// 停止当前预览，销毁场景中的粒子实例
        /// </summary>
        private void StopPreview()
        {
            if (_previewInstance == null)
            {
                return;
            }

            // 通知运行时停止预览并销毁实例
            ActionTimelinePreviewRuntime.PreviewParticleEnd(_previewInstance);
            _previewInstance = null;

            // 禁用停止按钮
            _stopButton?.SetEnabled(false);
        }
    }
}

#endif