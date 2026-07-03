namespace Hotfix.Http;

/// <summary>
/// YooAsset 热更 CDN 路径约定，须与 Unity UpdateSetting.projectName / GetPlatformName() 一致。
/// </summary>
public static class CdnPaths
{
    /// <summary>对应 UpdateSetting.projectName</summary>
    public const string ProjectName = "Kayano";

    /// <summary>静态资源根目录（Main 输出目录下的 Bundles/）</summary>
    public static string BundleRoot =>
        Path.Combine(AppContext.BaseDirectory, "Bundles");
}
