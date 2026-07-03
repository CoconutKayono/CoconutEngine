using Fantasy.Battle;
using Fantasy.Entitas.Interface;

namespace Fantasy;

/// <summary>
/// 战斗房每帧更新：驱动 MonsterFsm，并按固定间隔广播 M2C_MonsterSync。
/// <para>
/// Fantasy Source Generator 自动注册为 BattleRoomComponent 的 UpdateSystem，
/// 无需在 OnCreateScene 里手动 AddSystem。
/// </para>
/// </summary>
public sealed class BattleRoomUpdateSystem : UpdateSystem<BattleRoomComponent>
{
    protected override void Update(BattleRoomComponent self)
    {
        var now = DateTime.UtcNow.Ticks / (double)TimeSpan.TicksPerSecond;
        if (self.LastTickSeconds <= 0d)
        {
            self.LastTickSeconds = now;
            return;
        }

        var deltaTime = (float)(now - self.LastTickSeconds);
        self.LastTickSeconds = now;
        if (deltaTime <= 0f)
        {
            return;
        }

        // AI + 技能时间轴 + 服务端 HitBox 判怪打玩家
        MonsterFsm.TickRoom(self, deltaTime);

        if (self.Monsters.Count == 0)
        {
            return;
        }

        // 约 10Hz 位置同步，减轻带宽；PlayAction 仍即时单发
        self.SyncAccumulator += deltaTime;
        if (self.SyncAccumulator < 0.1f)
        {
            return;
        }

        self.SyncAccumulator = 0f;
        foreach (var kv in self.Monsters)
        {
            BattleRoomHelper.NotifyMonsterSync(self, kv.Value);
        }
    }
}
