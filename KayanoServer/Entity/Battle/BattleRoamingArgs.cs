using Fantasy.Entitas;
using MemoryPack;

namespace Fantasy.Battle;

[MemoryPackable]
public sealed partial class BattleRoamingArgs : Entity
{
    public long AccountId;

    public override void Dispose()
    {
        AccountId = 0;
        base.Dispose();
    }
}
