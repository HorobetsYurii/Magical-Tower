using UnityEngine;

public abstract class EnemyConfig : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField, Min(0.1f)] private float hitRadius = 0.55f;
    [SerializeField, Min(0.1f)] private float hitHeight = 1.1f;

    public GameObject Prefab => prefab;
    public float HitRadius => hitRadius;
    public float HitHeight => hitHeight;
    public float AimHeight => hitHeight * 0.5f;

    public abstract Enemy GetConfiguredEnemy();
}
