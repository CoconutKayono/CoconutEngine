using Fantasy.Async;
using Fantasy.Battle;
using Fantasy.Entitas;
using Fantasy.Event;
using Fantasy.Network.Roaming;

namespace Fantasy;

/// <summary>
/// 玩家 Roaming Link 到 Map Scene 时：创建 BattlePlayerUnit 并 Link 到 Terminus。
/// <para>
/// 此后 C2M_* Handler 的第一个参数即为该 BattlePlayerUnit；
/// M2C 单播通过 player.TryGetLinkTerminus 回客户端。
/// </para>
/// </summary>
public sealed class OnCreateTerminus_LinkBattlePlayer : AsyncEventSystem<OnCreateTerminus>
{
    protected override async FTask Handler(OnCreateTerminus self)
    {
        if (!IsMapScene(self.Scene) || self.Type != CreateTerminusType.Link)
        {
            await FTask.CompletedTask;
            return;
        }

        var room = BattleRoomHelper.GetOrCreate(self.Scene);
        var accountId = self.Args is BattleRoamingArgs battleArgs ? battleArgs.AccountId : 0L;
        var player = Entity.Create<BattlePlayerUnit>(self.Scene);
        player.AccountId = accountId;
        BattleRoomHelper.RegisterPlayer(room, player);
        await self.Terminus.LinkTerminusEntity(player, true);
        Log.Info($"[OnCreateTerminus] Battle player linked AccountId={accountId}");
        await FTask.CompletedTask;
    }

    private static bool IsMapScene(Scene scene)
    {
        return Scene.SceneTypeDictionary.TryGetValue("Map", out var mapType)
               && scene.SceneType == mapType;
    }
}
