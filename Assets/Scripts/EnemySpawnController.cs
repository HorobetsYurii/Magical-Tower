using System.Collections.Generic;

public class EnemySpawnController
{
    private readonly EnemySpawnConfig config;
    private readonly List<EnemyConfig> spawnedThisTick = new List<EnemyConfig>();
    private float spawnTimer;

    public EnemySpawnController(EnemySpawnConfig config)
    {
        this.config = config;
    }

    public IReadOnlyList<EnemyConfig> Tick(float elapsedTime, float deltaTime)
    {
        spawnedThisTick.Clear();

        var period = config.GetPeriod(elapsedTime);
        if (period == null)
        {
            return spawnedThisTick;
        }

        spawnTimer += deltaTime;
        while (spawnTimer >= period.SpawnInterval)
        {
            spawnTimer -= period.SpawnInterval;
            for (var i = 0; i < period.EnemiesPerSpawn; i++)
            {
                var enemyConfig = period.PickEnemy();
                if (enemyConfig != null)
                {
                    spawnedThisTick.Add(enemyConfig);
                }
            }
        }

        return spawnedThisTick;
    }
}
