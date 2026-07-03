using System.Collections.Generic;
using UnityEngine;

namespace KayanoAction.Runtime
{
    [CreateAssetMenu(fileName = "KayanoActionCatalog", menuName = "Kayano/Action Catalog")]
    public sealed class TimelineCatalogSO : ScriptableObject
    {
        public string namePrefix;

        /// <summary>进 Play 时 Front 槽起手招式（RuntimeSO.actionName；必须在 actions 内）。</summary>
        [TimelineActionName]
        public string defaultBootstrapActionName;

        public List<TimelineActionSO> actions = new();

        /// <summary>Timeline / Runtime 资产名去前缀，得到 actionName（如 Anbi_Idle → Idle）。</summary>
        public string RemoveNamePrefix(string actionFullName)
        {
            if (string.IsNullOrEmpty(namePrefix) || !actionFullName.StartsWith(namePrefix))
            {
                return actionFullName;
            }

            return actionFullName[namePrefix.Length..];
        }

        public bool ContainsAction(string actionName)
        {
            if (string.IsNullOrEmpty(actionName) || actions == null)
            {
                return false;
            }

            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                if (action != null && action.actionName == actionName)
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetBootstrapActionName(out string actionName, out string error)
        {
            actionName = defaultBootstrapActionName;
            if (string.IsNullOrEmpty(actionName))
            {
                error = "未配置 defaultBootstrapActionName。";
                return false;
            }

            if (!ContainsAction(actionName))
            {
                error = $"defaultBootstrapActionName「{actionName}」不在 actions 列表中。";
                return false;
            }

            error = null;
            return true;
        }
    }
}
