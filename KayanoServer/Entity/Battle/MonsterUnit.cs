using Fantasy.Entitas;

namespace Fantasy.Battle;

/// <summary>
/// 服务端怪物实体（权威数据，无 Unity 依赖）。
/// <para>
/// 与客户端 MonsterDriver 通过 MonsterId 对应；位置/HP/技能状态以本 Entity 为准。
/// AI 与 HitBox 在 Hotfix 的 MonsterFsm 中每帧更新。
/// </para>
/// </summary>
public sealed class MonsterUnit : Entity
{
    public int MonsterId;
    public int ConfigId;
    public double Hp;
    public double MaxHp;

    /// <summary>世界坐标与 Y 轴朝向（度），Chase 时 Run 的 moveSpeed 会改 PosX/PosZ。</summary>
    public float PosX;
    public float PosY;
    public float PosZ;
    public float RotY;

    /// <summary>AI 状态：Idle / Chase / Attack。</summary>
    public MonsterFsmState State = MonsterFsmState.Idle;

    /// <summary>Idle 状态下累计时间，达到 IdleColdSeconds 后才检测玩家距离。</summary>
    public float IdleColdTime;

    /// <summary>当前播放的技能名，对应 ServerAction/{ActionName}.json。</summary>
    public string CurrentActionName = MonsterActionNames.Idle;

    /// <summary>当前技能已播放时间（秒），用于 HitBox 窗口与 Duration 判断。</summary>
    public float ActionTime;

    /// <summary>是否仍在播放非循环技能的时长中。</summary>
    public bool ActionPlaying = true;

    /// <summary>非循环技能结束后是否自动回到 Idle 状态。</summary>
    public bool PendingReturnIdle;

    /// <summary>
    /// 当前技能已触发过的 HitId 集合，防止同一 HitBox 窗口重复造成伤害。
    /// PlayAction / 循环技能重置时会 Clear。
    /// </summary>
    public readonly HashSet<int> HitSessions = new(8);
}
