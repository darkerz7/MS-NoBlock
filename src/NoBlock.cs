using Microsoft.Extensions.Configuration;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.HookParams;
using Sharp.Shared.Listeners;
using Sharp.Shared.Managers;

namespace MS_NoBlock
{
    public class NoBlock : IModSharpModule, IEntityListener
    {
        public string DisplayName => "NoBlock";
        public string DisplayAuthor => "DarkerZ[RUS]";
        public NoBlock(ISharedSystem sharedSystem, string dllPath, string sharpPath, Version version, IConfiguration coreConfiguration, bool hotReload)
        {
            _hooks = sharedSystem.GetHookManager();
            _entities = sharedSystem.GetEntityManager();
        }
        private readonly IHookManager _hooks;
        private readonly IEntityManager _entities;

        public bool Init()
        {
            _hooks.PlayerSpawnPost.InstallForward(OnPlayerSpawn);
            _entities.InstallEntityListener(this);
            return true;
        }
        public void Shutdown()
        {
            _hooks.PlayerSpawnPost.RemoveForward(OnPlayerSpawn);
            _entities.RemoveEntityListener(this);
        }

        private void OnPlayerSpawn(IPlayerSpawnForwardParams @params)
        {
            var client = @params.Client;
            if (client.IsValid && client.GetPlayerController() is { } player && player.GetPawn() is { } pawn && pawn.GetCollisionProperty() is { } property)
            {
                if (property.CollisionAttribute.CollisionGroup == CollisionGroupType.Debris) return;
                pawn.SetCollisionGroup(CollisionGroupType.Debris);
            }
        }

        public void OnEntitySpawned(IBaseEntity entity)
        {
            if (entity.Classname.EndsWith("_projectile", StringComparison.OrdinalIgnoreCase))
            {
                entity.SetCollisionGroup(CollisionGroupType.Debris);
            }
        }

        int IEntityListener.ListenerVersion => IEntityListener.ApiVersion;
        int IEntityListener.ListenerPriority => 0;
    }
}
