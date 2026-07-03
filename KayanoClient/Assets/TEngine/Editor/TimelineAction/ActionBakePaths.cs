#if UNITY_EDITOR

namespace TEngine.Editor.TimelineAction
{
    /// <summary>动作烘焙路径与菜单常量。</summary>
    internal static class ActionBakePaths
    {
        public const string MenuRoot = "Kayano/动作/";

        /// <summary>Runtime SO 与 Catalog 默认输出目录。</summary>
        public const string OutputFolder = "Assets/GameScripts/HotFix/GameLogic/TimelineAction/Baked";

        /// <summary>服务端 Action JSON（相对 Assets 目录）。</summary>
        public const string ServerActionRelativeFolder = "../../KayanoServer/Entity/ServerAction";

        public const string DialogTitle = "动作烘焙";
    }
}

#endif
