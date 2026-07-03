using Fantasy.Entitas;

namespace Fantasy.Battle;

/// <summary>
/// Map 战斗房组件：挂在 Map Scene 上，持有本房间所有怪物与玩家。
/// <para>
/// 【Fantasy ECS】Entity 子类即 Component；由 BattleRoomHelper.GetOrCreate 在 OnCreateScene 时添加。
/// 每帧由 <see cref="BattleRoomUpdateSystem"/> Tick AI 并定期 Sync。
/// </para>
/// </summary>
public sealed class BattleRoomComponent : Entity
{
    /// <summary>下一只怪的 MonsterId（自增，房间局部唯一）。</summary>
    public int NextMonsterId = 1;

    /// <summary>是否已刷过默认 Demo 怪（EnterMap 时首玩家触发 Spawn）。</summary>
    public bool MonsterSpawned;

    /// <summary>上一帧 UTC 时间戳（秒），用于 UpdateSystem 计算 deltaTime。</summary>
    public double LastTickSeconds;

    /// <summary>位置 Sync 累加器，满 0.1s 广播 M2C_MonsterSync。</summary>
    public float SyncAccumulator;

    /// <summary>本房间所有服务端权威怪物，key = MonsterId。</summary>
    public readonly Dictionary<int, MonsterUnit> Monsters = new(4);

    /// <summary>本房间所有玩家，key = AccountId；位置由 C2M_SyncFrontRole 更新。</summary>
    public readonly Dictionary<long, BattlePlayerUnit> Players = new(8);
}
