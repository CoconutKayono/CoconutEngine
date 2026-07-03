#if UNITY_EDITOR

using KayanoAction.Runtime;
using UnityEditor;
using UnityEngine;

namespace TEngine.Editor.TimelineAction
{
    /// <summary>动作烘焙 Editor 菜单入口。</summary>
    public static class ActionBakeMenu
    {
        private const string BakeSelected = ActionBakePaths.MenuRoot + "烘焙选中 Timeline";
        private const string BakeSelectedToCatalog = ActionBakePaths.MenuRoot + "烘焙选中 Timeline 并写入 Catalog";
        private const string BakeAllToCatalog = ActionBakePaths.MenuRoot + "烘焙全部 Timeline 并写入 Catalog";

        [MenuItem(BakeSelected)]
        private static void BakeSelectedTimeline()
        {
            var timeline = Selection.activeObject as ActionTimeline;
            if (timeline == null)
            {
                EditorUtility.DisplayDialog(
                    ActionBakePaths.DialogTitle,
                    "请在 Project 中选中一个 ActionTimeline（如 Anbi_Idle）。",
                    "确定");
                return;
            }

            ActionBakeAssetUtility.EnsureFolder(ActionBakePaths.OutputFolder);
            var catalog = ActionBakeAssetUtility.ResolveCatalog();
            var runtime = ActionBakePipeline.Bake(timeline, catalog, ActionBakePaths.OutputFolder);
            if (runtime != null)
            {
                EditorGUIUtility.PingObject(runtime);
                Debug.Log($"[ActionBake] 已烘焙: {timeline.name} → {AssetDatabase.GetAssetPath(runtime)}");
            }
        }

        [MenuItem(BakeSelected, true)]
        private static bool BakeSelectedTimelineValidate()
        {
            return Selection.activeObject is ActionTimeline;
        }

        [MenuItem(BakeSelectedToCatalog)]
        private static void BakeSelectedTimelinesToCatalog()
        {
            var catalog = ActionBakeAssetUtility.ResolveCatalog();
            if (catalog == null)
            {
                EditorUtility.DisplayDialog(
                    ActionBakePaths.DialogTitle,
                    "未找到 TimelineCatalogSO。请选中 Catalog 资产，或在项目中创建一个。",
                    "确定");
                return;
            }

            var timelines = ActionBakeAssetUtility.GetSelectedTimelines();
            if (timelines.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    ActionBakePaths.DialogTitle,
                    "请同时选中至少一个 ActionTimeline（可与 Catalog 多选）。",
                    "确定");
                return;
            }

            BakeToCatalog(catalog, timelines, "选中");
        }

        [MenuItem(BakeSelectedToCatalog, true)]
        private static bool BakeSelectedTimelinesToCatalogValidate()
        {
            return ActionBakeAssetUtility.GetSelectedTimelines().Count > 0;
        }

        [MenuItem(BakeAllToCatalog)]
        private static void BakeAllTimelinesToCatalog()
        {
            var catalog = ActionBakeAssetUtility.ResolveCatalog();
            if (catalog == null)
            {
                EditorUtility.DisplayDialog(
                    ActionBakePaths.DialogTitle,
                    "未找到 TimelineCatalogSO。请选中 Catalog 资产，或在项目中创建一个。",
                    "确定");
                return;
            }

            var timelines = ActionBakeAssetUtility.FindAllTimelines();
            if (timelines.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    ActionBakePaths.DialogTitle,
                    "项目中未找到任何 ActionTimeline。",
                    "确定");
                return;
            }

            var catalogPath = ActionBakeAssetUtility.GetCatalogHint(catalog);
            if (!EditorUtility.DisplayDialog(
                    ActionBakePaths.DialogTitle,
                    $"将烘焙全部 {timelines.Count} 个 Timeline 并写入 Catalog：\n{catalogPath}",
                    "烘焙",
                    "取消"))
            {
                return;
            }

            BakeToCatalog(catalog, timelines, "全部");
        }

        [MenuItem(BakeAllToCatalog, true)]
        private static bool BakeAllTimelinesToCatalogValidate()
        {
            return true;
        }

        private static void BakeToCatalog(
            TimelineCatalogSO catalog,
            System.Collections.Generic.List<ActionTimeline> timelines,
            string scopeLabel)
        {
            ActionBakeAssetUtility.EnsureFolder(ActionBakePaths.OutputFolder);
            var count = ActionBakePipeline.BakeTimelines(timelines, catalog, ActionBakePaths.OutputFolder);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(catalog);
            Debug.Log(
                $"[ActionBake] 已烘焙 {scopeLabel} {count} 个 Timeline → Catalog: " +
                ActionBakeAssetUtility.GetCatalogHint(catalog));
        }
    }
}

#endif
