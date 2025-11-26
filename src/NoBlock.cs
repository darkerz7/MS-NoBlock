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
        bool bActivate = false;

        public bool Init()
        {
            g_cvar_teammates = _convars.FindConVar("mp_solid_teammates");
            if (g_cvar_teammates != null)
            {
                g_cvar_teammates.Set(false);
                _convars.InstallChangeHook(g_cvar_teammates, OnCvarTeammatesChanged);
            }
            g_cvar_enemies = _convars.FindConVar("mp_solid_enemies");
            if (g_cvar_enemies != null)
            {
                g_cvar_enemies.Set(false);
                _convars.InstallChangeHook(g_cvar_enemies, OnCvarEnemiesChanged);
            }
            _modSharp.InstallGameListener(this);
            _entities.InstallEntityListener(this);
            return true;
        }

        private void OnCvarTeammatesChanged(IConVar conVar)
        {
            if (bActivate && conVar.GetBool()) conVar.Set(false);
        }

        private void OnCvarEnemiesChanged(IConVar conVar)
        {
            if (bActivate && conVar.GetBool()) conVar.Set(false);
        }

        public void OnGameActivate()
        {
            bActivate = true;
            g_cvar_teammates?.Set(false);
            g_cvar_enemies?.Set(false);
        }

        public void OnGameDeactivate()
        {
            bActivate = false;
        }

        public void Shutdown()
        {
            if (g_cvar_teammates != null) _convars.RemoveChangeHook(g_cvar_teammates, OnCvarTeammatesChanged);
            if (g_cvar_enemies != null) _convars.RemoveChangeHook(g_cvar_enemies, OnCvarEnemiesChanged);
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
