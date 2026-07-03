namespace Fantasy.Battle;

/// <summary>怪物 AI 有限状态机状态。</summary>
public enum MonsterFsmState
{
    Idle = 0,
    Chase = 1,
    Attack = 2,
}

/// <summary>
/// 技能动作名常量。必须与以下两处完全一致：
/// 1) KayanoServer/Entity/ServerAction/{Name}.json 文件名
/// 2) 客户端 KayanoActionCatalogSO 中的 ActionName
/// </summary>
public static class MonsterActionNames
{
    public const string Idle = "Idle";
    public const string Run = "Run";
    public const string SkillA = "Skill_A";
    public const string SkillB = "Skill_B";
}

/// <summary>战斗 Demo 默认数值，调 AI 手感主要改距离与 Idle 冷却。</summary>
public static class BattleDefaults
{
    public const int DefaultMonsterConfigId = 1;
    public const float AttackDistance = 5f;
    public const float ChaseDistance = 15f;
    public const float IdleColdSeconds = 2f;
    public const float RunMoveSpeed = 3.5f;
    public const double DefaultMonsterHp = 1000d;
    public const float SpawnX = 18f;
    public const float SpawnY = 0f;
    public const float SpawnZ = -12f;
}
