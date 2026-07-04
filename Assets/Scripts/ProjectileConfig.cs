using UnityEngine;

public abstract class ProjectileConfig : ScriptableObject
{
    [SerializeField] private GameObject prefab;

    public GameObject Prefab => prefab;
    public abstract Projectile GetConfiguredProjectile();
}

public abstract class Projectile : IPositionable, IEntity
{
    protected Projectile(ProjectileConfig config)
    {
        Config = config;
    }

    public int Id { get; internal set; }
    public Vector3 Position { get; set; }
    public ProjectileConfig Config { get; }
    public GameObject Prefab => Config.Prefab;
    public bool IsExpired { get; private set; }

    public abstract void Initialize(ProjectileSpawnData spawnData);
    public abstract void Tick(GameplayContext context, float deltaTime);

    protected void MarkExpired()
    {
        IsExpired = true;
    }
}

public readonly struct ProjectileSpawnData
{
    private ProjectileSpawnData(Vector3 position, Vector3 direction, Vector3 targetPosition, Enemy target)
    {
        Position = position;
        Direction = direction;
        TargetPosition = targetPosition;
        Target = target;
    }

    public Vector3 Position { get; }
    public Vector3 Direction { get; }
    public Vector3 TargetPosition { get; }
    public Enemy Target { get; }

    public static ProjectileSpawnData Directed(Vector3 position, Vector3 direction, Vector3 targetPosition, Enemy target)
    {
        return new ProjectileSpawnData(position, direction, targetPosition, target);
    }

    public static ProjectileSpawnData Targeted(Vector3 position, Enemy target)
    {
        return new ProjectileSpawnData(position, Vector3.zero, target.Position, target);
    }
}
