#if UNITY_EDITOR

using System.Collections.Generic;
using KayanoAction.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TEngine.Editor.ActionModule
{
    [CustomEditor(typeof(TimelineCatalogSO))]
    public sealed class KayanoActionCatalogSOEditor : UnityEditor.Editor
    {
        private PopupField<string> _bootstrapPopup;
        private HelpBox _validationBox;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(ActionModuleInspectorElements.BuildHelpBox(
                "角色招式表：Bake 后的 RuntimeSO 列表 + 默认起手招式。起手必须在 actions 内配置。",
                HelpBoxMessageType.Info));

            root.Add(ActionModuleInspectorElements.BuildDefaultInspector(
                serializedObject,
                nameof(TimelineCatalogSO.defaultBootstrapActionName)));

            root.Add(ActionModuleInspectorElements.BuildSectionHeader("默认起手招式"));
            _bootstrapPopup = new PopupField<string>("起手招式", new List<string> { "（无）" }, 0);
            _bootstrapPopup.RegisterValueChangedCallback(OnBootstrapChanged);
            root.Add(_bootstrapPopup);

            _validationBox = ActionModuleInspectorElements.BuildHelpBox(string.Empty, HelpBoxMessageType.None);
            root.Add(_validationBox);

            RefreshBootstrapChoices();
            serializedObject.ApplyModifiedProperties();

            return root;
        }

        private void OnBootstrapChanged(ChangeEvent<string> evt)
        {
            var catalog = (TimelineCatalogSO)target;
            var value = evt.newValue;
            catalog.defaultBootstrapActionName = value == "（无）" ? string.Empty : value;
            EditorUtility.SetDirty(catalog);
            RefreshValidation();
        }

        private void RefreshBootstrapChoices()
        {
            var catalog = (TimelineCatalogSO)target;
            var choices = CollectActionNames(catalog);
            if (choices.Count == 0)
            {
                choices.Add("（无）");
            }

            var current = catalog.defaultBootstrapActionName;
            var index = string.IsNullOrEmpty(current) ? 0 : choices.IndexOf(current);
            if (index < 0)
            {
                index = 0;
            }

            _bootstrapPopup.choices = choices;
            _bootstrapPopup.index = index;
            if (!string.IsNullOrEmpty(current) && choices.Contains(current))
            {
                _bootstrapPopup.SetValueWithoutNotify(current);
            }

            RefreshValidation();
        }

        private void RefreshValidation()
        {
            var catalog = (TimelineCatalogSO)target;
            if (catalog.TryGetBootstrapActionName(out _, out var error))
            {
                _validationBox.text = "配置有效：进 Play 时将播放默认起手招式。";
                _validationBox.messageType = HelpBoxMessageType.Info;
            }
            else
            {
                _validationBox.text = error;
                _validationBox.messageType = HelpBoxMessageType.Error;
            }
        }

        internal static List<string> CollectActionNames(TimelineCatalogSO catalog)
        {
            var result = new List<string>();
            if (catalog?.actions == null)
            {
                return result;
            }

            var seen = new HashSet<string>();
            for (var i = 0; i < catalog.actions.Count; i++)
            {
                var action = catalog.actions[i];
                if (action == null || string.IsNullOrEmpty(action.actionName))
                {
                    continue;
                }

                if (seen.Add(action.actionName))
                {
                    result.Add(action.actionName);
                }
            }

            result.Sort();
            return result;
        }

        internal static void ValidateCatalogOrLogError(TimelineCatalogSO catalog)
        {
            if (catalog == null)
            {
                return;
            }

            if (!catalog.TryGetBootstrapActionName(out _, out var error))
            {
                Debug.LogError($"[KayanoActionCatalog] {catalog.name}：{error}");
            }
        }
    }
}

#endif
