using Fantasy.Async;
using Fantasy.Battle;
using Fantasy.Network.Interface;
using Fantasy.Network.Roaming;

namespace Fantasy;

/// <summary>
/// 客户端 EnterMap：注册玩家，首进时刷默认怪并 M2C_MonsterSpawn。
/// RoamingRPC 在 Map 进程执行，entity 为 BattlePlayerUnit。
/// </summary>
public sealed class C2M_EnterMapRequestHandler : RoamingRPC<BattlePlayerUnit, C2M_EnterMapRequest, M2C_EnterMapResponse>
{
    protected override async FTask Run(
        BattlePlayerUnit player,
        C2M_EnterMapRequest request,
        M2C_EnterMapResponse response,
        Action reply)
    {
        var room = BattleRoomHelper.GetOrCreate(player.Scene);
        BattleRoomHelper.RegisterPlayer(room, player);

        if (!room.MonsterSpawned)
        {
            var monster = BattleRoomHelper.SpawnDefaultMonster(room);
            BattleRoomHelper.NotifyMonsterSpawn(room, monster);
        }

        await FTask.CompletedTask;
    }
}

/// <summary>
/// 客户端上报编队前台位置，供 MonsterFsm 寻敌与 HitBox 判命中。
/// 单向 Roaming 消息，无回包。
/// </summary>
public sealed class C2M_SyncFrontRoleHandler : Roaming<BattlePlayerUnit, C2M_SyncFrontRole>
{
    protected override async FTask Run(BattlePlayerUnit player, C2M_SyncFrontRole message)
    {
        if (message.Position == null)
        {
            await FTask.CompletedTask;
            return;
        }

        player.FrontPosX = message.Position.X;
        player.FrontPosY = message.Position.Y;
        player.FrontPosZ = message.Position.Z;
        player.FrontRotY = message.RotY;
        await FTask.CompletedTask;
    }
}

/// <summary>
/// 客户端 HitBox 命中怪物后上报伤害；服务端扣 HP 并 Sync。
/// 【注意】当前 Demo 信任客户端 Damage，正式项目需服务端校验。
/// </summary>
public sealed class C2M_MonsterTakeDamageRequestHandler
    : RoamingRPC<BattlePlayerUnit, C2M_MonsterTakeDamageRequest, M2C_MonsterTakeDamageResponse>
{
    protected override async FTask Run(
        BattlePlayerUnit player,
        C2M_MonsterTakeDamageRequest request,
        M2C_MonsterTakeDamageResponse response,
        Action reply)
    {
        var room = BattleRoomHelper.GetOrCreate(player.Scene);
        if (!room.Monsters.TryGetValue(request.MonsterId, out var monster))
        {
            response.ErrorCode = 1;
            return;
        }

        if (request.Damage <= 0f)
        {
            response.ErrorCode = 2;
            return;
        }

        monster.Hp = Math.Max(0d, monster.Hp - request.Damage);
        response.MonsterId = monster.MonsterId;
        response.Hp = monster.Hp;
        BattleRoomHelper.NotifyMonsterSync(room, monster);
        await FTask.CompletedTask;
    }
}
