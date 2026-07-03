using Fantasy.Battle;

namespace Fantasy;

/// <summary>
/// 服务端怪物 AI + 技能时间轴 + HitBox 判定（权威逻辑全在此）。
/// <para>
/// 每帧由 BattleRoomUpdateSystem 调用 TickRoom。
/// 状态机：Idle →（发现玩家）→ Chase / Attack；Attack 播完回 Idle。
/// 客户端只收 M2C_PlayAction / Sync / AttackRole，不跑本 FSM。
/// </para>
/// </summary>
public static class MonsterFsm
{
    public static void TickRoom(BattleRoomComponent room, float deltaTime)
    {
        foreach (var kv in room.Monsters)
        {
            TickMonster(room, kv.Value, deltaTime);
        }
    }

    public static void TickMonster(BattleRoomComponent room, MonsterUnit monster, float deltaTime)
    {
        // 先推进当前技能（含移动、HitBox），再决定是否切 AI 状态
        TickAction(room, monster, deltaTime);
        UpdateState(room, monster, deltaTime);
    }

    /// <summary>
    /// 切换技能：重置时间轴、清空 HitSessions、预加载 JSON。
    /// </summary>
    /// <param name="returnIdle">非循环技能结束后是否 EnterState(Idle)。</param>
    public static void PlayAction(MonsterUnit monster, string actionName, bool returnIdle = true)
    {
        monster.CurrentActionName = actionName;
        monster.ActionTime = 0f;
        monster.ActionPlaying = true;
        monster.PendingReturnIdle = returnIdle;
        monster.HitSessions.Clear();
        _ = ServerActionLoader.Get(actionName);
    }

    /// <summary>
    /// 技能时间轴：累加 ActionTime、按 moveSpeed 位移、处理 HitBox、判断技能结束。
    /// </summary>
    private static void TickAction(BattleRoomComponent room, MonsterUnit monster, float deltaTime)
    {
        if (!monster.ActionPlaying)
        {
            return;
        }

        var action = ServerActionLoader.Get(monster.CurrentActionName);
        monster.ActionTime += deltaTime;

        // Run 等 loop 技能：服务端根据 JSON moveSpeed 改 PosX/PosZ（非 Root Motion）
        if (action.MoveSpeed > 0f)
        {
            var yaw = monster.RotY * (MathF.PI / 180f);
            monster.PosX += MathF.Sin(yaw) * action.MoveSpeed * deltaTime;
            monster.PosZ += MathF.Cos(yaw) * action.MoveSpeed * deltaTime;
        }

        ProcessHitBoxes(room, monster, action);

        if (action.IsLoop)
        {
            // 循环技能：到 Duration 重置时间轴与 HitSessions（如 Idle/Run）
            if (action.Duration > 0f && monster.ActionTime >= action.Duration)
            {
                monster.ActionTime = 0f;
                monster.HitSessions.Clear();
            }

            return;
        }

        if (monster.ActionTime < action.Duration)
        {
            return;
        }

        monster.ActionPlaying = false;
        if (monster.PendingReturnIdle)
        {
            EnterState(room, monster, MonsterFsmState.Idle);
        }
    }

    /// <summary>
    /// 服务端 HitBox：在 ActionTime 窗口内对最近玩家做球体距离检测，命中则 M2C_MonsterAttackRole。
    /// </summary>
    private static void ProcessHitBoxes(BattleRoomComponent room, MonsterUnit monster, ServerActionData action)
    {
        if (action.HitBoxes.Count == 0 || !BattleRoomHelper.TryGetClosestPlayer(room, monster, out var target, out _))
        {
            return;
        }

        foreach (var box in action.HitBoxes)
        {
            if (monster.ActionTime < box.Start || monster.ActionTime > box.End)
            {
                continue;
            }

            // 同一 HitId 在当前技能播放周期内只结算一次
            if (!monster.HitSessions.Add(box.HitId))
            {
                continue;
            }

            if (!TestHit(monster, target, box))
            {
                continue;
            }

            var damage = ResolveDamage(box.HitId);
            BattleRoomHelper.NotifyMonsterAttackRole(room, monster, target, box.HitId, damage);
        }
    }

    /// <summary>局部 HitBox 中心转世界坐标，与玩家 FrontPos 做球体距离判定。</summary>
    private static bool TestHit(MonsterUnit monster, BattlePlayerUnit target, ServerHitBoxWindow box)
    {
        var yaw = monster.RotY * (MathF.PI / 180f);
        var centerX = monster.PosX + box.CenterX * MathF.Cos(yaw) + box.CenterZ * MathF.Sin(yaw);
        var centerY = monster.PosY + box.CenterY;
        var centerZ = monster.PosZ - box.CenterX * MathF.Sin(yaw) + box.CenterZ * MathF.Cos(yaw);

        var dx = target.FrontPosX - centerX;
        var dy = target.FrontPosY - centerY;
        var dz = target.FrontPosZ - centerZ;
        var distSqr = dx * dx + dy * dy + dz * dz;
        return distSqr <= box.Radius * box.Radius;
    }

    /// <summary>Demo 伤害公式；正式项目应读 Luban 配置表。</summary>
    private static float ResolveDamage(int hitId)
    {
        return hitId > 0 ? 50f + hitId * 0.1f : 30f;
    }

    /// <summary>AI 状态切换：Idle 冷却检测距离；Chase 追击；Attack 期间不打断。</summary>
    private static void UpdateState(BattleRoomComponent room, MonsterUnit monster, float deltaTime)
    {
        if (monster.State == MonsterFsmState.Attack && monster.ActionPlaying)
        {
            return;
        }

        switch (monster.State)
        {
            case MonsterFsmState.Idle:
            {
                monster.IdleColdTime += deltaTime;
                if (monster.IdleColdTime < BattleDefaults.IdleColdSeconds)
                {
                    return;
                }

                monster.IdleColdTime = 0f;
                if (!BattleRoomHelper.TryGetClosestPlayer(room, monster, out _, out var distance))
                {
                    return;
                }

                if (distance < BattleDefaults.AttackDistance)
                {
                    EnterState(room, monster, MonsterFsmState.Attack);
                }
                else if (distance < BattleDefaults.ChaseDistance)
                {
                    EnterState(room, monster, MonsterFsmState.Chase);
                }

                break;
            }
            case MonsterFsmState.Chase:
            {
                if (!BattleRoomHelper.TryGetClosestPlayer(room, monster, out var target, out var distance))
                {
                    EnterState(room, monster, MonsterFsmState.Idle);
                    return;
                }

                FaceTarget(monster, target);
                if (distance < BattleDefaults.AttackDistance)
                {
                    EnterState(room, monster, MonsterFsmState.Attack);
                }
                else if (distance > BattleDefaults.ChaseDistance)
                {
                    EnterState(room, monster, MonsterFsmState.Idle);
                }

                break;
            }
        }
    }

    /// <summary>进入新状态：切技能、广播 M2C_MonsterPlayAction。</summary>
    public static void EnterState(BattleRoomComponent room, MonsterUnit monster, MonsterFsmState state)
    {
        if (monster.State == state && state != MonsterFsmState.Attack)
        {
            return;
        }

        monster.State = state;
        switch (state)
        {
            case MonsterFsmState.Idle:
                PlayAction(monster, MonsterActionNames.Idle, returnIdle: false);
                BattleRoomHelper.NotifyMonsterPlayAction(room, monster, MonsterActionNames.Idle);
                break;
            case MonsterFsmState.Chase:
                PlayAction(monster, MonsterActionNames.Run, returnIdle: false);
                BattleRoomHelper.NotifyMonsterPlayAction(room, monster, MonsterActionNames.Run);
                break;
            case MonsterFsmState.Attack:
                if (BattleRoomHelper.TryGetClosestPlayer(room, monster, out var target, out _))
                {
                    FaceTarget(monster, target);
                }

                var skill = Random.Shared.Next(0, 2) == 0
                    ? MonsterActionNames.SkillA
                    : MonsterActionNames.SkillB;
                if (ServerActionLoader.Get(skill).ActionName != skill)
                {
                    skill = MonsterActionNames.SkillA;
                }

                PlayAction(monster, skill, returnIdle: true);
                BattleRoomHelper.NotifyMonsterPlayAction(room, monster, skill);
                break;
        }
    }

    private static void FaceTarget(MonsterUnit monster, BattlePlayerUnit target)
    {
        var dx = target.FrontPosX - monster.PosX;
        var dz = target.FrontPosZ - monster.PosZ;
        if (dx * dx + dz * dz < 0.0001f)
        {
            return;
        }

        monster.RotY = MathF.Atan2(dx, dz) * (180f / MathF.PI);
    }
}
