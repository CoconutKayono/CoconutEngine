using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace Fantasy;

/// <summary>
/// Gate 登录：校验账号密码（首版为桩实现，非空即通过）。
/// </summary>
public sealed class C2G_LoginRequestHandler : MessageRPC<C2G_LoginRequest, G2C_LoginResponse>
{
    protected override async FTask Run(Session session, C2G_LoginRequest request, G2C_LoginResponse response, Action reply)
    {
        if (string.IsNullOrWhiteSpace(request.Account) || string.IsNullOrWhiteSpace(request.Password))
        {
            response.ErrorCode = 1;
            return;
        }

        // TODO: 接入 MongoDB / Account 实体后替换为真实鉴权
        response.AccountId = request.Account.GetHashCode();
        var gateSession = session.GetComponent<GateSessionComponent>();
        if (gateSession == null)
        {
            gateSession = session.AddComponent<GateSessionComponent>();
        }

        gateSession.AccountId = response.AccountId;
        await FTask.CompletedTask;
    }
}
