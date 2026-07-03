using Fantasy.Async;
using Fantasy.Battle;
using Fantasy.Event;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Map Scene 创建时初始化战斗房（空房间，尚无怪物）。
/// 怪物在首个玩家 C2M_EnterMapRequest 时 Spawn。
/// </summary>
public sealed class OnCreateSceneEvent : AsyncEventSystem<OnCreateScene>
{
    protected override async FTask Handler(OnCreateScene self)
    {
        var scene = self.Scene;
        if (!IsMapScene(scene))
        {
            await FTask.CompletedTask;
            return;
        }

        BattleRoomHelper.GetOrCreate(scene);
        Log.Info("[OnCreateSceneEvent] Map battle room ready.");
        await FTask.CompletedTask;
    }

    private static bool IsMapScene(Scene scene)
    {
        return Scene.SceneTypeDictionary.TryGetValue("Map", out var mapType)
               && scene.SceneType == mapType;
    }
}
