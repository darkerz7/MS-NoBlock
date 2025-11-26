using Microsoft.Extensions.Configuration;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.HookParams;
using Sharp.Shared.Listeners;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;

namespace MS_NoBlock
{
    public class NoBlock : IModSharpModule, IEntityListener, IGameListener
    {
        public string DisplayName => "NoBlock";
        public string DisplayAuthor => "DarkerZ[RUS]";
        public NoBlock(ISharedSystem sharedSystem, string dllPath, string sharpPath, Version version, IConfiguration coreConfiguration, bool hotReload)
        {
            _modSharp = sharedSystem.GetModSharp();
            _entities = sharedSystem.GetEntityManager();
            _convars = sharedSystem.GetConVarManager();
        }
        private readonly IModSharp _modSharp;
        private readonly IEntityManager _entities;
        private readonly IConVarManager _convars;

        private IConVar? g_cvar_teammates;
        private IConVar? g_cvar_enemies;

        public bool Init()
        {
            g_cvar_teammates = _convars.FindConVar("mp_solid_teammates");
            g_cvar_teammates?.Set(false);
            g_cvar_enemies = _convars.FindConVar("mp_solid_enemies");
            g_cvar_enemies?.Set(false);

            _modSharp.InstallGameListener(this);
            _entities.InstallEntityListener(this);
            return true;
        }

        public void OnRoundRestarted()
        {
            g_cvar_teammates?.Set(false);
            g_cvar_enemies?.Set(false);
        }

        public void Shutdown()
        {
            _modSharp!.RemoveGameListener(this);
            _entities.RemoveEntityListener(this);
        }

        public void OnEntitySpawned(IBaseEntity entity)
        {
            if (entity.Classname.EndsWith("_projectile", StringComparison.OrdinalIgnoreCase))
            {
                entity.SetCollisionGroup(CollisionGroupType.Debris);
            }
        }

        int IGameListener.ListenerVersion => IGameListener.ApiVersion;
        int IGameListener.ListenerPriority => 0;
        int IEntityListener.ListenerVersion => IEntityListener.ApiVersion;
        int IEntityListener.ListenerPriority => 0;
    }
}
