using UnityEngine;

[CreateAssetMenu(fileName = "MeleeEnemyConfig", menuName = "Magical Tower/Enemies/Melee Enemy Config")]
public class MeleeEnemyConfig : EnemyConfig
{
    [SerializeField, Min(1f)] private float maxHealth = 20f;
    [SerializeField, Min(0.1f)] private float speed = 2f;
    [SerializeField, Min(0.1f)] private float attackRange = 1.25f;
    [SerializeField, Min(0.1f)] private float attackDamage = 5f;
    [SerializeField, Min(0.05f)] private float attackCooldown = 1f;

    public float MaxHealth => maxHealth;
    public float Speed => speed;
    public float AttackRange => attackRange;
    public float AttackDamage => attackDamage;
    public float AttackCooldown => attackCooldown;

    public override Enemy GetConfiguredEnemy()
    {
        return new MeleeEnemy(this);
    }
}
