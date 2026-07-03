using Fantasy;
using Fantasy.Async;
using Fantasy.Event;
using Fantasy.Network.HTTP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace Hotfix.Http;

/// <summary>
/// HTTP Scene 中间件：映射 Bundles/ 为 YooAsset 远程目录。
/// 客户端 URL：http://host:8080/Kayano/Windows64/{fileName}
/// </summary>
public sealed class OnConfigureHttpApplicationHandler : AsyncEventSystem<OnConfigureHttpApplication>
{
    private static FileExtensionContentTypeProvider CreateYooAssetContentTypes()
    {
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".version"] = "text/plain";
        provider.Mappings[".bytes"] = "application/octet-stream";
        provider.Mappings[".hash"] = "text/plain";
        provider.Mappings[".bundle"] = "application/octet-stream";
        return provider;
    }

    protected override async FTask Handler(OnConfigureHttpApplication self)
    {
        var app = self.Application;
        var bundleRoot = CdnPaths.BundleRoot;

        app.UseCors("KayanoCdn");

        if (Directory.Exists(bundleRoot))
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(bundleRoot),
                RequestPath = "",
                ContentTypeProvider = CreateYooAssetContentTypes(),
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.CacheControl = "no-cache";
                }
            });

            Log.Info($"[CDN] Static files: {bundleRoot}  (e.g. /{CdnPaths.ProjectName}/Windows64/)");
        }
        else
        {
            Log.Warning($"[CDN] Bundle directory not found: {bundleRoot}. Copy YooAsset build output to KayanoServer/Bundles/Kayano/Windows64/.");
        }

        await FTask.CompletedTask;
    }
}
