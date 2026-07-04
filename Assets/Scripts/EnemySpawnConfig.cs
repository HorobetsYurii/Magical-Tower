using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnConfig", menuName = "Magical Tower/Enemy Spawn Config")]
public class EnemySpawnConfig : ScriptableObject
{
    [SerializeField] private List<EnemySpawnPeriod> periods = new List<EnemySpawnPeriod>();

    public EnemySpawnPeriod GetPeriod(float elapsedTime)
    {
        EnemySpawnPeriod result = null;
        foreach (var period in periods)
        {
            if (period != null && elapsedTime >= period.StartTime &&
                (result == null || period.StartTime >= result.StartTime))
            {
                result = period;
            }
        }

        return result;
    }
}

[Serializable]
public class EnemySpawnPeriod
{
    [SerializeField, Min(0f)] private float startTime;
    [SerializeField, Min(0.05f)] private float spawnInterval = 1f;
    [SerializeField, Min(1)] private int enemiesPerSpawn = 1;
    [SerializeField] private List<EnemySpawnEntry> enemies = new List<EnemySpawnEntry>();

    public float StartTime => startTime;
    public float SpawnInterval => Mathf.Max(0.05f, spawnInterval);
    public int EnemiesPerSpawn => Mathf.Max(1, enemiesPerSpawn);

    public EnemyConfig PickEnemy()
    {
        var totalWeight = 0f;
        foreach (var enemy in enemies)
        {
            if (enemy?.Config != null)
            {
                totalWeight += Mathf.Max(0f, enemy.Weight);
            }
        }

        if (totalWeight <= 0f)
        {
            return null;
        }

        var roll = UnityEngine.Random.Range(0f, totalWeight);
        foreach (var enemy in enemies)
        {
            if (enemy?.Config == null)
            {
                continue;
            }

            roll -= Mathf.Max(0f, enemy.Weight);
            if (roll <= 0f)
            {
                return enemy.Config;
            }
        }

        for (var i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i]?.Config != null)
            {
                return enemies[i].Config;
            }
        }

        return null;
    }
}

[Serializable]
public class EnemySpawnEntry
{
    [SerializeField] private EnemyConfig config;
    [SerializeField, Min(0f)] private float weight = 1f;

    public EnemyConfig Config => config;
    public float Weight => weight;
}
