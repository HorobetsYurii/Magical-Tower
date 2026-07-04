using System;
using System.Collections.Generic;

public class ProjectilesController
{
    private readonly List<Projectile> projectiles = new List<Projectile>();
    private int nextProjectileId = 1;

    public IReadOnlyList<Projectile> Projectiles => projectiles;

    public Projectile Spawn(ProjectileConfig config, ProjectileSpawnData spawnData)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        var projectile = config.GetConfiguredProjectile();
        projectile.Id = nextProjectileId++;
        projectile.Initialize(spawnData);
        projectiles.Add(projectile);
        return projectile;
    }

    public void Tick(GameplayContext context, float deltaTime)
    {
        for (var i = projectiles.Count - 1; i >= 0; i--)
        {
            var projectile = projectiles[i];
            if (projectile.IsExpired)
            {
                RemoveProjectileAt(i);
                continue;
            }

            projectile.Tick(context, deltaTime);

            if (projectile.IsExpired)
            {
                RemoveProjectileAt(i);
            }
        }
    }

    private void RemoveProjectileAt(int index)
    {
        projectiles.RemoveAt(index);
    }
}
