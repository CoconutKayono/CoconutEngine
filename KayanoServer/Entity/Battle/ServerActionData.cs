namespace Fantasy.Battle;

/// <summary>
/// 从 ServerAction/*.json 反序列化的技能数据（服务端权威时间轴）。
/// 由 Unity Timeline 烘焙或手写，与客户端 KayanoActionRuntimeSO 的 ActionName 对齐。
/// </summary>
public sealed class ServerActionData
{
    public string ActionName = string.Empty;
    public float Duration = 1f;
    public bool IsLoop;
    public float MoveSpeed;
    public List<ServerHitBoxWindow> HitBoxes = new();
}

/// <summary>
/// 技能时间轴上的一个 HitBox 窗口（相对怪物局部坐标系的球体判定）。
/// </summary>
public sealed class ServerHitBoxWindow
{
    public int HitId;
    public float Start;
    public float End;
    public string Shape = "Sphere";
    public float CenterX;
    public float CenterY;
    public float CenterZ;
    public float Radius = 1f;
    public float SizeX = 1f;
    public float SizeY = 1f;
    public float SizeZ = 1f;
}
