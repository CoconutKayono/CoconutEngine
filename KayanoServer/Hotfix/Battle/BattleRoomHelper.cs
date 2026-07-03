using Fantasy.Battle;
using Fantasy.Entitas;
using Fantasy.Network.Interface;
using Fantasy.Network.Roaming;

namespace Fantasy;

/// <summary>
/// 战斗房工具类：创建房间/刷怪/寻敌/构造并发送 M2C 怪物消息。
/// </summary>
public static class BattleRoomHelper
{
    /// <summary>获取或创建 Map Scene 上的 BattleRoomComponent。</summary>
    public static BattleRoomComponent GetOrCreate(Scene scene)
    {
        var room = scene.GetComponent<BattleRoomComponent>();
        if (room != null)
        {
            return room;
        }

        room = scene.AddComponent<BattleRoomComponent>();
        room.LastTickSeconds = DateTime.UtcNow.Ticks / (double)TimeSpan.TicksPerSecond;
        return room;
    }

    /// <summary>Demo：在固定坐标刷一只默认怪并加入 room.Monsters。</summary>
    public static MonsterUnit SpawnDefaultMonster(BattleRoomComponent room)
    {
        var monster = Entity.Create<MonsterUnit>(room.Scene);
        monster.MonsterId = room.NextMonsterId++;
        monster.ConfigId = BattleDefaults.DefaultMonsterConfigId;
        monster.Hp = BattleDefaults.DefaultMonsterHp;
        monster.MaxHp = BattleDefaults.DefaultMonsterHp;
        monster.PosX = BattleDefaults.SpawnX;
        monster.PosY = BattleDefaults.SpawnY;
        monster.PosZ = BattleDefaults.SpawnZ;
        monster.State = MonsterFsmState.Idle;
        MonsterFsm.PlayAction(monster, MonsterActionNames.Idle, returnIdle: false);
        room.Monsters[monster.MonsterId] = monster;
        room.MonsterSpawned = true;
        return monster;
    }

    public static void RegisterPlayer(BattleRoomComponent room, BattlePlayerUnit player)
    {
        room.Players[player.AccountId] = player;
    }

    /// <summary>XZ 平面距离最近的玩家（AI 寻敌用）。</summary>
    public static bool TryGetClosestPlayer(BattleRoomComponent room, MonsterUnit monster, out BattlePlayerUnit player, out float distance)
    {
        player = null!;
        distance = float.MaxValue;
        if (room.Players.Count == 0)
        {
            return false;
        }

        foreach (var kv in room.Players)
        {
            var candidate = kv.Value;
            var dx = candidate.FrontPosX - monster.PosX;
            var dz = candidate.FrontPosZ - monster.PosZ;
            var dist = MathF.Sqrt(dx * dx + dz * dz);
            if (dist >= distance)
            {
                continue;
            }

            distance = dist;
            player = candidate;
        }

        return player != null;
    }

    /// <summary>向房间内所有已 Link 的玩家广播 Roaming 消息。</summary>
    public static void BroadcastToAllPlayers<T>(BattleRoomComponent room, T message)
        where T : AMessage, IRoamingMessage
    {
        foreach (var kv in room.Players)
        {
            SendToPlayer(kv.Value, message);
        }
    }

    /// <summary>单播：通过 BattlePlayerUnit 上 Link 的 Terminus 发回客户端 Session。</summary>
    public static void SendToPlayer<T>(BattlePlayerUnit player, T message)
        where T : AMessage, IRoamingMessage
    {
        if (!player.TryGetLinkTerminus(out var terminus))
        {
            return;
        }

        terminus.Send(message);
    }

    public static void NotifyMonsterSpawn(BattleRoomComponent room, MonsterUnit monster)
    {
        var msg = M2C_MonsterSpawn.Create();
        msg.MonsterId = monster.MonsterId;
        msg.ConfigId = monster.ConfigId;
        msg.Position = ToVec3(monster.PosX, monster.PosY, monster.PosZ);
        msg.RotY = monster.RotY;
        msg.Hp = monster.Hp;
        msg.MaxHp = monster.MaxHp;
        BroadcastToAllPlayers(room, msg);
    }

    public static void NotifyMonsterSync(BattleRoomComponent room, MonsterUnit monster)
    {
        var msg = M2C_MonsterSync.Create();
        msg.MonsterId = monster.MonsterId;
        msg.Position = ToVec3(monster.PosX, monster.PosY, monster.PosZ);
        msg.RotY = monster.RotY;
        msg.Hp = monster.Hp;
        BroadcastToAllPlayers(room, msg);
    }

    public static void NotifyMonsterPlayAction(BattleRoomComponent room, MonsterUnit monster, string actionName)
    {
        var msg = M2C_MonsterPlayAction.Create();
        msg.MonsterId = monster.MonsterId;
        msg.ActionName = actionName;
        BroadcastToAllPlayers(room, msg);
    }

    /// <summary>怪打玩家：只发给被命中玩家（非 Broadcast）。</summary>
    public static void NotifyMonsterAttackRole(
        BattleRoomComponent room,
        MonsterUnit monster,
        BattlePlayerUnit target,
        int hitId,
        float damage)
    {
        var msg = M2C_MonsterAttackRole.Create();
        msg.MonsterId = monster.MonsterId;
        msg.HitId = hitId;
        msg.Damage = damage;
        msg.HitPoint = ToVec3(target.FrontPosX, target.FrontPosY, target.FrontPosZ);
        SendToPlayer(target, msg);
    }

    public static Vec3 ToVec3(float x, float y, float z)
    {
        return new Vec3 { X = x, Y = y, Z = z };
    }
}
