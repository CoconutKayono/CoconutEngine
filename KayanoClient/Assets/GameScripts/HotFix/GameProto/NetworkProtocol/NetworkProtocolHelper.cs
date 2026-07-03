using System.Runtime.CompilerServices;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using System.Collections.Generic;
#pragma warning disable CS8618
namespace Fantasy
{
   public static class NetworkProtocolHelper
   {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_ConnectRoamingResponse> C2G_ConnectRoamingRequest(this Session session, C2G_ConnectRoamingRequest C2G_ConnectRoamingRequest_request)
		{
			return (G2C_ConnectRoamingResponse)await session.Call(C2G_ConnectRoamingRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_ConnectRoamingResponse> C2G_ConnectRoamingRequest(this Session session)
		{
			using var C2G_ConnectRoamingRequest_request = Fantasy.C2G_ConnectRoamingRequest.Create();
			return (G2C_ConnectRoamingResponse)await session.Call(C2G_ConnectRoamingRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<M2C_EnterMapResponse> C2M_EnterMapRequest(this Session session, C2M_EnterMapRequest C2M_EnterMapRequest_request)
		{
			return (M2C_EnterMapResponse)await session.Call(C2M_EnterMapRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<M2C_EnterMapResponse> C2M_EnterMapRequest(this Session session, int mapId)
		{
			using var C2M_EnterMapRequest_request = Fantasy.C2M_EnterMapRequest.Create();
			C2M_EnterMapRequest_request.MapId = mapId;
			return (M2C_EnterMapResponse)await session.Call(C2M_EnterMapRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void C2M_SyncFrontRole(this Session session, C2M_SyncFrontRole C2M_SyncFrontRole_message)
		{
			session.Send(C2M_SyncFrontRole_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void C2M_SyncFrontRole(this Session session, Vec3 position, float rotY)
		{
			using var C2M_SyncFrontRole_message = Fantasy.C2M_SyncFrontRole.Create();
			C2M_SyncFrontRole_message.Position = position;
			C2M_SyncFrontRole_message.RotY = rotY;
			session.Send(C2M_SyncFrontRole_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<M2C_MonsterTakeDamageResponse> C2M_MonsterTakeDamageRequest(this Session session, C2M_MonsterTakeDamageRequest C2M_MonsterTakeDamageRequest_request)
		{
			return (M2C_MonsterTakeDamageResponse)await session.Call(C2M_MonsterTakeDamageRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<M2C_MonsterTakeDamageResponse> C2M_MonsterTakeDamageRequest(this Session session, int monsterId, int hitId, float damage)
		{
			using var C2M_MonsterTakeDamageRequest_request = Fantasy.C2M_MonsterTakeDamageRequest.Create();
			C2M_MonsterTakeDamageRequest_request.MonsterId = monsterId;
			C2M_MonsterTakeDamageRequest_request.HitId = hitId;
			C2M_MonsterTakeDamageRequest_request.Damage = damage;
			return (M2C_MonsterTakeDamageResponse)await session.Call(C2M_MonsterTakeDamageRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void M2C_MonsterSpawn(this Session session, M2C_MonsterSpawn M2C_MonsterSpawn_message)
		{
			session.Send(M2C_MonsterSpawn_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void M2C_MonsterSpawn(this Session session, int monsterId, int configId, Vec3 position, float rotY, double hp, double maxHp)
		{
			using var M2C_MonsterSpawn_message = Fantasy.M2C_MonsterSpawn.Create();
			M2C_MonsterSpawn_message.MonsterId = monsterId;
			M2C_MonsterSpawn_message.ConfigId = configId;
			M2C_MonsterSpawn_message.Position = position;
			M2C_MonsterSpawn_message.RotY = rotY;
			M2C_MonsterSpawn_message.Hp = hp;
			M2C_MonsterSpawn_message.MaxHp = maxHp;
			session.Send(M2C_MonsterSpawn_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void M2C_MonsterSync(this Session session, M2C_MonsterSync M2C_MonsterSync_message)
		{
			session.Send(M2C_MonsterSync_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void M2C_MonsterSync(this Session session, int monsterId, Vec3 position, float rotY, double hp)
		{
			using var M2C_MonsterSync_message = Fantasy.M2C_MonsterSync.Create();
			M2C_MonsterSync_message.MonsterId = monsterId;
			M2C_MonsterSync_message.Position = position;
			M2C_MonsterSync_message.RotY = rotY;
			M2C_MonsterSync_message.Hp = hp;
			session.Send(M2C_MonsterSync_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void M2C_MonsterPlayAction(this Session session, M2C_MonsterPlayAction M2C_MonsterPlayAction_message)
		{
			session.Send(M2C_MonsterPlayAction_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void M2C_MonsterPlayAction(this Session session, int monsterId, string actionName)
		{
			using var M2C_MonsterPlayAction_message = Fantasy.M2C_MonsterPlayAction.Create();
			M2C_MonsterPlayAction_message.MonsterId = monsterId;
			M2C_MonsterPlayAction_message.ActionName = actionName;
			session.Send(M2C_MonsterPlayAction_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void M2C_MonsterAttackRole(this Session session, M2C_MonsterAttackRole M2C_MonsterAttackRole_message)
		{
			session.Send(M2C_MonsterAttackRole_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void M2C_MonsterAttackRole(this Session session, int monsterId, int hitId, float damage, Vec3 hitPoint)
		{
			using var M2C_MonsterAttackRole_message = Fantasy.M2C_MonsterAttackRole.Create();
			M2C_MonsterAttackRole_message.MonsterId = monsterId;
			M2C_MonsterAttackRole_message.HitId = hitId;
			M2C_MonsterAttackRole_message.Damage = damage;
			M2C_MonsterAttackRole_message.HitPoint = hitPoint;
			session.Send(M2C_MonsterAttackRole_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void C2G_TestMessage(this Session session, C2G_TestMessage C2G_TestMessage_message)
		{
			session.Send(C2G_TestMessage_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void C2G_TestMessage(this Session session, string tag)
		{
			using var C2G_TestMessage_message = Fantasy.C2G_TestMessage.Create();
			C2G_TestMessage_message.Tag = tag;
			session.Send(C2G_TestMessage_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_TestResponse> C2G_TestRequest(this Session session, C2G_TestRequest C2G_TestRequest_request)
		{
			return (G2C_TestResponse)await session.Call(C2G_TestRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_TestResponse> C2G_TestRequest(this Session session, string tag)
		{
			using var C2G_TestRequest_request = Fantasy.C2G_TestRequest.Create();
			C2G_TestRequest_request.Tag = tag;
			return (G2C_TestResponse)await session.Call(C2G_TestRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LoginResponse> C2G_LoginRequest(this Session session, C2G_LoginRequest C2G_LoginRequest_request)
		{
			return (G2C_LoginResponse)await session.Call(C2G_LoginRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LoginResponse> C2G_LoginRequest(this Session session, string account, string password)
		{
			using var C2G_LoginRequest_request = Fantasy.C2G_LoginRequest.Create();
			C2G_LoginRequest_request.Account = account;
			C2G_LoginRequest_request.Password = password;
			return (G2C_LoginResponse)await session.Call(C2G_LoginRequest_request);
		}

   }
}