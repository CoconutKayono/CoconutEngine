using Fantasy.Entitas;

namespace Fantasy.Battle;

/// <summary>
/// Map 战斗房中的玩家实体（Roaming Link 时创建）。
/// <para>
/// 仅存「编队前台」Transform，供 MonsterFsm 算距离与 HitBox 判命中。
/// 由客户端 C2M_SyncFrontRole 驱动更新。
/// </para>
/// </summary>
public sealed class BattlePlayerUnit : Entity
{
    public long AccountId;

    /// <summary>前台角色世界坐标，AI 寻敌与 TestHit 使用。</summary>
    public float FrontPosX;
    public float FrontPosY;
    public float FrontPosZ;
    public float FrontRotY;
}
