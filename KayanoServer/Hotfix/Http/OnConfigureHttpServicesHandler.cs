using Fantasy.Async;
using Fantasy.Event;
using Fantasy.Network.HTTP;
using Microsoft.Extensions.DependencyInjection;

namespace Hotfix.Http;

/// <summary>
/// HTTP Scene 服务注册：CORS 等（CDN 静态资源供 Unity/YooAsset 拉取）。
/// </summary>
public sealed class OnConfigureHttpServicesHandler : AsyncEventSystem<OnConfigureHttpServices>
{
    protected override async FTask Handler(OnConfigureHttpServices self)
    {
        // self.Builder 是 ASP.NET Core 的 IServiceCollection
        // 这里调用 AddCors 注册 CORS 策略
        self.Builder.Services.AddCors(options =>
        {
            // 创建一个名为 "KayanoCdn" 的 CORS 策略
            options.AddPolicy("KayanoCdn", policy =>
                policy.AllowAnyOrigin()   // 允许任何域名访问
                    .AllowAnyMethod()     // 允许任何 HTTP 方法（GET/POST/PUT...）
                    .AllowAnyHeader());   // 允许任何请求头
        });

        await FTask.CompletedTask;
    }
}
