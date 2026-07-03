using GameConfig.Main;
using GameLogic;
using TEngine;

/// <summary>
/// 意图辅助工具 — 提供配置查询等工具方法
/// </summary>
public static class IntentHelper
{
    /// <summary>
    /// 从 Luban 配置表获取招式配置
    /// </summary>
    public static ChActionConfig GetActionConfig(int actionId)
    {
        var config = ConfigSystem.Instance.Tables.TbChActionConfig.GetOrDefault(actionId);
        if (config == null)
        {
            Log.Warning($"[IntentHelper] 未找到 ActionId: {actionId} 的配置");
            return null;
        }
        return config;
    }

    /// <summary>
    /// 从 Luban 配置表获取动作优先级
    /// </summary>
    public static int GetPriorityFromConfig(int actionId)
    {
        var config = GetActionConfig(actionId);
        return config?.Priority ?? -1;
    }

    /// <summary>
    /// 从 Luban 配置表获取能量消耗
    /// </summary>
    public static float GetEnergyCostFromConfig(int actionId)
    {
        var config = GetActionConfig(actionId);
        return config?.EnergyCost ?? 0f;
    }

    /// <summary>
    /// 从 Luban 配置表获取喧响值消耗
    /// </summary>
    public static float GetDecibelCostFromConfig(int actionId)
    {
        var config = GetActionConfig(actionId);
        return config?.DecibelCost ?? 0f;
    }

    /// <summary>
    /// 从 Luban 配置表获取连携槽消耗
    /// </summary>
    public static float GetChainGaugeCostFromConfig(int actionId)
    {
        var config = GetActionConfig(actionId);
        return config?.ChainGaugeCost ?? 0f;
    }

    /// <summary>
    /// 从 Luban 配置表获取闪避体力消耗
    /// </summary>
    public static float GetDodgeStaminaCostFromConfig(int actionId)
    {
        var config = GetActionConfig(actionId);
        return config?.DodgeStaminaCost ?? 0f;
    }
}