#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TEngine.Editor.ActionModule
{
    internal static class ActionModuleInspectorElements
    {
        public static VisualElement BuildDefaultInspector(
            SerializedObject serializedObject,
            params string[] skipPropertyNames)
        {
            var container = new VisualElement();
            var iterator = serializedObject.GetIterator();
            if (!iterator.NextVisible(true))
            {
                return container;
            }

            do
            {
                if (ShouldSkipProperty(iterator.name, skipPropertyNames))
                {
                    continue;
                }

                var field = new PropertyField(iterator.Copy());
                if (iterator.name == "m_Script")
                {
                    field.SetEnabled(false);
                }

                container.Add(field);
            }
            while (iterator.NextVisible(false));

            return container;
        }

        public static HelpBox BuildHelpBox(string message, HelpBoxMessageType type = HelpBoxMessageType.Info)
        {
            return new HelpBox(message, type) { style = { marginTop = 6f, marginBottom = 6f } };
        }

        public static Label BuildSectionHeader(string title)
        {
            return new Label(title)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginTop = 8f,
                    marginBottom = 4f,
                },
            };
        }

        private static bool ShouldSkipProperty(string propertyName, string[] skipPropertyNames)
        {
            if (skipPropertyNames == null || skipPropertyNames.Length == 0)
            {
                return false;
            }

            for (var i = 0; i < skipPropertyNames.Length; i++)
            {
                if (propertyName == skipPropertyNames[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
}

#endif
