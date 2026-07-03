using LightProto;
using System;
using MemoryPack;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Pool;
using Fantasy.Network.Interface;
using Fantasy.Serialize;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618
// ReSharper disable InconsistentNaming
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable RedundantTypeArgumentsOfMethod
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PreferConcreteValueOverDefault
// ReSharper disable RedundantNameQualifier
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable RedundantUsingDirective
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Fantasy
{
    [Serializable]
    [ProtoContract]
    public partial class Vec3 : AMessage, IDisposable
    {
        public static Vec3 Create(bool autoReturn = true)
        {
            var vec3 = MessageObjectPool<Vec3>.Rent();
            vec3.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                vec3.SetIsPool(false);
            }
            
            return vec3;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            X = default;
            Y = default;
            Z = default;
            MessageObjectPool<Vec3>.Return(this);
        }
        [ProtoMember(1)]
        public float X { get; set; }
        [ProtoMember(2)]
        public float Y { get; set; }
        [ProtoMember(3)]
        public float Z { get; set; }
    }
    /// <summary>
    /// Gate 建立 Map 漫游（单 Session，KCP 连 Gate）
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2G_ConnectRoamingRequest : AMessage, IRequest
    {
        public static C2G_ConnectRoamingRequest Create(bool autoReturn = true)
        {
            var c2G_ConnectRoamingRequest = MessageObjectPool<C2G_ConnectRoamingRequest>.Rent();
            c2G_ConnectRoamingRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_ConnectRoamingRequest.SetIsPool(false);
            }
            
            return c2G_ConnectRoamingRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MessageObjectPool<C2G_ConnectRoamingRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_ConnectRoamingRequest; } 
        [ProtoIgnore]
        public G2C_ConnectRoamingResponse ResponseType { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class G2C_ConnectRoamingResponse : AMessage, IResponse
    {
        public static G2C_ConnectRoamingResponse Create(bool autoReturn = true)
        {
            var g2C_ConnectRoamingResponse = MessageObjectPool<G2C_ConnectRoamingResponse>.Rent();
            g2C_ConnectRoamingResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_ConnectRoamingResponse.SetIsPool(false);
            }
            
            return g2C_ConnectRoamingResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MessageObjectPool<G2C_ConnectRoamingResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_ConnectRoamingResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
    }
    /// <summary>
    /// 进入 Map 战斗房
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2M_EnterMapRequest : AMessage, IRoamingRequest
    {
        public static C2M_EnterMapRequest Create(bool autoReturn = true)
        {
            var c2M_EnterMapRequest = MessageObjectPool<C2M_EnterMapRequest>.Rent();
            c2M_EnterMapRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2M_EnterMapRequest.SetIsPool(false);
            }
            
            return c2M_EnterMapRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MapId = default;
            MessageObjectPool<C2M_EnterMapRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2M_EnterMapRequest; } 
        [ProtoIgnore]
        public M2C_EnterMapResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.MapRoamingType;
        [ProtoMember(1)]
        public int MapId { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class M2C_EnterMapResponse : AMessage, IRoamingResponse
    {
        public static M2C_EnterMapResponse Create(bool autoReturn = true)
        {
            var m2C_EnterMapResponse = MessageObjectPool<M2C_EnterMapResponse>.Rent();
            m2C_EnterMapResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                m2C_EnterMapResponse.SetIsPool(false);
            }
            
            return m2C_EnterMapResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MessageObjectPool<M2C_EnterMapResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.M2C_EnterMapResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
    }
    /// <summary>
    /// 客户端上报编队前台 Transform（AI 用）
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2M_SyncFrontRole : AMessage, IRoamingMessage
    {
        public static C2M_SyncFrontRole Create(bool autoReturn = true)
        {
            var c2M_SyncFrontRole = MessageObjectPool<C2M_SyncFrontRole>.Rent();
            c2M_SyncFrontRole.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2M_SyncFrontRole.SetIsPool(false);
            }
            
            return c2M_SyncFrontRole;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            if (Position != null)
            {
                Position.Dispose();
                Position = null;
            }
            RotY = default;
            MessageObjectPool<C2M_SyncFrontRole>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2M_SyncFrontRole; } 
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.MapRoamingType;
        [ProtoMember(1)]
        public Vec3 Position { get; set; }
        [ProtoMember(2)]
        public float RotY { get; set; }
    }
    /// <summary>
    /// 客户端 HitBox 上报怪物伤害
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2M_MonsterTakeDamageRequest : AMessage, IRoamingRequest
    {
        public static C2M_MonsterTakeDamageRequest Create(bool autoReturn = true)
        {
            var c2M_MonsterTakeDamageRequest = MessageObjectPool<C2M_MonsterTakeDamageRequest>.Rent();
            c2M_MonsterTakeDamageRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2M_MonsterTakeDamageRequest.SetIsPool(false);
            }
            
            return c2M_MonsterTakeDamageRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MonsterId = default;
            HitId = default;
            Damage = default;
            MessageObjectPool<C2M_MonsterTakeDamageRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2M_MonsterTakeDamageRequest; } 
        [ProtoIgnore]
        public M2C_MonsterTakeDamageResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.MapRoamingType;
        [ProtoMember(1)]
        public int MonsterId { get; set; }
        [ProtoMember(2)]
        public int HitId { get; set; }
        [ProtoMember(3)]
        public float Damage { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class M2C_MonsterTakeDamageResponse : AMessage, IRoamingResponse
    {
        public static M2C_MonsterTakeDamageResponse Create(bool autoReturn = true)
        {
            var m2C_MonsterTakeDamageResponse = MessageObjectPool<M2C_MonsterTakeDamageResponse>.Rent();
            m2C_MonsterTakeDamageResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                m2C_MonsterTakeDamageResponse.SetIsPool(false);
            }
            
            return m2C_MonsterTakeDamageResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MonsterId = default;
            Hp = default;
            MessageObjectPool<M2C_MonsterTakeDamageResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.M2C_MonsterTakeDamageResponse; } 
        [ProtoMember(3)]
        public uint ErrorCode { get; set; }
        [ProtoMember(1)]
        public int MonsterId { get; set; }
        [ProtoMember(2)]
        public double Hp { get; set; }
    }
    /// <summary>
    /// Map 推送：怪物生成
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class M2C_MonsterSpawn : AMessage, IRoamingMessage
    {
        public static M2C_MonsterSpawn Create(bool autoReturn = true)
        {
            var m2C_MonsterSpawn = MessageObjectPool<M2C_MonsterSpawn>.Rent();
            m2C_MonsterSpawn.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                m2C_MonsterSpawn.SetIsPool(false);
            }
            
            return m2C_MonsterSpawn;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MonsterId = default;
            ConfigId = default;
            if (Position != null)
            {
                Position.Dispose();
                Position = null;
            }
            RotY = default;
            Hp = default;
            MaxHp = default;
            MessageObjectPool<M2C_MonsterSpawn>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.M2C_MonsterSpawn; } 
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.MapRoamingType;
        [ProtoMember(1)]
        public int MonsterId { get; set; }
        [ProtoMember(2)]
        public int ConfigId { get; set; }
        [ProtoMember(3)]
        public Vec3 Position { get; set; }
        [ProtoMember(4)]
        public float RotY { get; set; }
        [ProtoMember(5)]
        public double Hp { get; set; }
        [ProtoMember(6)]
        public double MaxHp { get; set; }
    }
    /// <summary>
    /// Map 推送：怪物状态同步
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class M2C_MonsterSync : AMessage, IRoamingMessage
    {
        public static M2C_MonsterSync Create(bool autoReturn = true)
        {
            var m2C_MonsterSync = MessageObjectPool<M2C_MonsterSync>.Rent();
            m2C_MonsterSync.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                m2C_MonsterSync.SetIsPool(false);
            }
            
            return m2C_MonsterSync;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MonsterId = default;
            if (Position != null)
            {
                Position.Dispose();
                Position = null;
            }
            RotY = default;
            Hp = default;
            MessageObjectPool<M2C_MonsterSync>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.M2C_MonsterSync; } 
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.MapRoamingType;
        [ProtoMember(1)]
        public int MonsterId { get; set; }
        [ProtoMember(2)]
        public Vec3 Position { get; set; }
        [ProtoMember(3)]
        public float RotY { get; set; }
        [ProtoMember(4)]
        public double Hp { get; set; }
    }
    /// <summary>
    /// Map 推送：怪物播动作
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class M2C_MonsterPlayAction : AMessage, IRoamingMessage
    {
        public static M2C_MonsterPlayAction Create(bool autoReturn = true)
        {
            var m2C_MonsterPlayAction = MessageObjectPool<M2C_MonsterPlayAction>.Rent();
            m2C_MonsterPlayAction.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                m2C_MonsterPlayAction.SetIsPool(false);
            }
            
            return m2C_MonsterPlayAction;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MonsterId = default;
            ActionName = default;
            MessageObjectPool<M2C_MonsterPlayAction>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.M2C_MonsterPlayAction; } 
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.MapRoamingType;
        [ProtoMember(1)]
        public int MonsterId { get; set; }
        [ProtoMember(2)]
        public string ActionName { get; set; }
    }
    /// <summary>
    /// Map 推送：怪物命中前台角色
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class M2C_MonsterAttackRole : AMessage, IRoamingMessage
    {
        public static M2C_MonsterAttackRole Create(bool autoReturn = true)
        {
            var m2C_MonsterAttackRole = MessageObjectPool<M2C_MonsterAttackRole>.Rent();
            m2C_MonsterAttackRole.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                m2C_MonsterAttackRole.SetIsPool(false);
            }
            
            return m2C_MonsterAttackRole;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MonsterId = default;
            HitId = default;
            Damage = default;
            if (HitPoint != null)
            {
                HitPoint.Dispose();
                HitPoint = null;
            }
            MessageObjectPool<M2C_MonsterAttackRole>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.M2C_MonsterAttackRole; } 
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.MapRoamingType;
        [ProtoMember(1)]
        public int MonsterId { get; set; }
        [ProtoMember(2)]
        public int HitId { get; set; }
        [ProtoMember(3)]
        public float Damage { get; set; }
        [ProtoMember(4)]
        public Vec3 HitPoint { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class C2G_TestMessage : AMessage, IMessage
    {
        public static C2G_TestMessage Create(bool autoReturn = true)
        {
            var c2G_TestMessage = MessageObjectPool<C2G_TestMessage>.Rent();
            c2G_TestMessage.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_TestMessage.SetIsPool(false);
            }
            
            return c2G_TestMessage;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Tag = default;
            MessageObjectPool<C2G_TestMessage>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_TestMessage; } 
        [ProtoMember(1)]
        public string Tag { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class C2G_TestRequest : AMessage, IRequest
    {
        public static C2G_TestRequest Create(bool autoReturn = true)
        {
            var c2G_TestRequest = MessageObjectPool<C2G_TestRequest>.Rent();
            c2G_TestRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_TestRequest.SetIsPool(false);
            }
            
            return c2G_TestRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Tag = default;
            MessageObjectPool<C2G_TestRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_TestRequest; } 
        [ProtoIgnore]
        public G2C_TestResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Tag { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class G2C_TestResponse : AMessage, IResponse
    {
        public static G2C_TestResponse Create(bool autoReturn = true)
        {
            var g2C_TestResponse = MessageObjectPool<G2C_TestResponse>.Rent();
            g2C_TestResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_TestResponse.SetIsPool(false);
            }
            
            return g2C_TestResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            Tag = default;
            MessageObjectPool<G2C_TestResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_TestResponse; } 
        [ProtoMember(2)]
        public uint ErrorCode { get; set; }
        [ProtoMember(1)]
        public string Tag { get; set; }
    }
    /// <summary>
    /// 客户端登录 Gate
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2G_LoginRequest : AMessage, IRequest
    {
        public static C2G_LoginRequest Create(bool autoReturn = true)
        {
            var c2G_LoginRequest = MessageObjectPool<C2G_LoginRequest>.Rent();
            c2G_LoginRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_LoginRequest.SetIsPool(false);
            }
            
            return c2G_LoginRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Account = default;
            Password = default;
            MessageObjectPool<C2G_LoginRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_LoginRequest; } 
        [ProtoIgnore]
        public G2C_LoginResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Account { get; set; }
        [ProtoMember(2)]
        public string Password { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class G2C_LoginResponse : AMessage, IResponse
    {
        public static G2C_LoginResponse Create(bool autoReturn = true)
        {
            var g2C_LoginResponse = MessageObjectPool<G2C_LoginResponse>.Rent();
            g2C_LoginResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_LoginResponse.SetIsPool(false);
            }
            
            return g2C_LoginResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            AccountId = default;
            MessageObjectPool<G2C_LoginResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_LoginResponse; } 
        [ProtoMember(2)]
        public uint ErrorCode { get; set; }
        [ProtoMember(1)]
        public long AccountId { get; set; }
    }
}