using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController
{
    private readonly List<Enemy> enemies = new List<Enemy>();
    private int nextEnemyId = 1;

    public IReadOnlyList<Enemy> Enemies => enemies;

    public Enemy SpawnEnemy(EnemyConfig config, Vector3 position)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        var enemy = config.GetConfiguredEnemy();
        enemy.Id = nextEnemyId++;
        enemy.Position = position;
        enemies.Add(enemy);

        return enemy;
    }

    public void Tick(GameplayContext context, float deltaTime)
    {
        for (var i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];
            if (!enemy.IsAlive)
            {
                RemoveEnemyAt(i);
                continue;
            }

            enemy.Tick(context, deltaTime);
        }
    }

    private void RemoveEnemyAt(int index)
    {
        enemies.RemoveAt(index);
    }

    // Reservoir sampling: a single pass with no allocation, uniform pick among matching enemies.
    public Enemy GetRandomEnemy(Func<Enemy, bool> predicate)
    {
        Enemy chosen = null;
        var matches = 0;

        foreach (var enemy in enemies)
        {
            if (!predicate(enemy))
            {
                continue;
            }

            matches++;
            if (UnityEngine.Random.Range(0, matches) == 0)
            {
                chosen = enemy;
            }
        }

        return chosen;
    }
}
