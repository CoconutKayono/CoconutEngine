using System.Text.Json;
using Fantasy.Battle;

namespace Fantasy;

/// <summary>
/// 加载 ServerAction 目录下的技能 JSON（运行时目录：{BaseDirectory}/ServerAction/{actionName}.json）。
/// <para>
/// 首次 Get 时读盘并缓存；缺失文件时打 Warning 并返回 1 秒循环 fallback，避免 FSM 崩溃。
/// Entity 项目需配置 JSON CopyToOutputDirectory。
/// </para>
/// </summary>
public static class ServerActionLoader
{
    private static readonly Dictionary<string, ServerActionData> Cache = new(StringComparer.Ordinal);

    public static ServerActionData Get(string actionName)
    {
        if (Cache.TryGetValue(actionName, out var cached))
        {
            return cached;
        }

        var path = Path.Combine(AppContext.BaseDirectory, "ServerAction", $"{actionName}.json");
        if (!File.Exists(path))
        {
            Log.Warning($"[ServerActionLoader] Missing action json: {path}");
            var fallback = new ServerActionData { ActionName = actionName, Duration = 1f, IsLoop = true };
            Cache[actionName] = fallback;
            return fallback;
        }

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<ServerActionData>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (data == null)
        {
            data = new ServerActionData { ActionName = actionName, Duration = 1f, IsLoop = true };
        }

        Cache[actionName] = data;
        return data;
    }
}
