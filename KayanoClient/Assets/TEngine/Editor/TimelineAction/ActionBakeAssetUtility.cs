#if UNITY_EDITOR

using System.Collections.Generic;
using KayanoAction.Runtime;
using UnityEditor;
using UnityEngine;

namespace TEngine.Editor.TimelineAction
{
    /// <summary>烘焙相关资产查找、目录创建、Catalog 解析。</summary>
    internal static class ActionBakeAssetUtility
    {
        public static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            var parts = folder.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        public static List<ActionTimeline> FindAllTimelines()
        {
            var results = new List<ActionTimeline>();
            var guids = AssetDatabase.FindAssets("t:ActionTimeline");
            for (var i = 0; i < guids.Length; i++)
            {
                var timeline = AssetDatabase.LoadAssetAtPath<ActionTimeline>(
                    AssetDatabase.GUIDToAssetPath(guids[i]));
                if (timeline != null)
                {
                    results.Add(timeline);
                }
            }

            return results;
        }

        public static List<TimelineCatalogSO> FindAllCatalogs()
        {
            var results = new List<TimelineCatalogSO>();
            var guids = AssetDatabase.FindAssets("t:TimelineCatalogSO");
            for (var i = 0; i < guids.Length; i++)
            {
                var catalog = AssetDatabase.LoadAssetAtPath<TimelineCatalogSO>(
                    AssetDatabase.GUIDToAssetPath(guids[i]));
                if (catalog != null)
                {
                    results.Add(catalog);
                }
            }

            return results;
        }

        /// <summary>
        /// 解析目标 Catalog：优先选区中的 Catalog，其次项目中唯一的 Catalog，再次 Baked 目录下的 Catalog。
        /// </summary>
        public static TimelineCatalogSO ResolveCatalog()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is TimelineCatalogSO selected)
                {
                    return selected;
                }
            }

            var catalogs = FindAllCatalogs();
            if (catalogs.Count == 0)
            {
                return null;
            }

            if (catalogs.Count == 1)
            {
                return catalogs[0];
            }

            for (var i = 0; i < catalogs.Count; i++)
            {
                var path = AssetDatabase.GetAssetPath(catalogs[i]);
                if (path.Replace('\\', '/').Contains(ActionBakePaths.OutputFolder))
                {
                    return catalogs[i];
                }
            }

            return catalogs[0];
        }

        public static List<ActionTimeline> GetSelectedTimelines()
        {
            var results = new List<ActionTimeline>();
            foreach (var obj in Selection.objects)
            {
                if (obj is ActionTimeline timeline)
                {
                    results.Add(timeline);
                }
            }

            return results;
        }

        public static string GetCatalogHint(TimelineCatalogSO catalog)
        {
            return catalog != null
                ? AssetDatabase.GetAssetPath(catalog)
                : "<未找到 Catalog>";
        }
    }
}

#endif
