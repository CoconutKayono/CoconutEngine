using Fantasy.Async;
using Fantasy.Battle;
using Fantasy.Entitas;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Fantasy.Network.Roaming;
using Fantasy.Platform.Net;
using Fantasy.Roaming;

namespace Fantasy;

public sealed class C2G_ConnectRoamingRequestHandler : MessageRPC<C2G_ConnectRoamingRequest, G2C_ConnectRoamingResponse>
{
    protected override async FTask Run(
        Session session,
        C2G_ConnectRoamingRequest request,
        G2C_ConnectRoamingResponse response,
        Action reply)
    {
        var result = await session.TryCreateRoaming(1, 10000);
        if (result.Status == CreateRoamingStatus.SessionAlreadyHasRoaming)
        {
            response.ErrorCode = 2;
            return;
        }

        if (!Scene.SceneTypeDictionary.TryGetValue("Map", out _))
        {
            response.ErrorCode = 3;
            Log.Error("[C2G_ConnectRoamingRequest] Map SceneType not configured.");
            return;
        }

        var mapConfigs = SceneConfigData.Instance.GetSceneBySceneType(
            Scene.SceneTypeDictionary["Map"]);
        if (mapConfigs == null || mapConfigs.Count == 0)
        {
            response.ErrorCode = 4;
            return;
        }

        var mapConfig = mapConfigs[0];
        var accountId = session.GetComponent<GateSessionComponent>()?.AccountId ?? 0L;
        using var args = Entity.Create<BattleRoamingArgs>(session.Scene);
        args.AccountId = accountId;
        var linkCode = await result.Roaming.Link(session, mapConfig, RoamingType.MapRoamingType, args);
        if (linkCode != 0)
        {
            response.ErrorCode = linkCode;
            Log.Error($"[C2G_ConnectRoamingRequest] Link Map failed ErrorCode={linkCode}");
        }
    }
}

public sealed class GateSessionComponent : Entity
{
    public long AccountId;
}
