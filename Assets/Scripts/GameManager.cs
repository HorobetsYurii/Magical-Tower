using System.Collections.Generic;
using UnityEngine;

// Composition root + main loop. Builds every system, wires them through a shared GameplayContext,
// and drives them each frame in a fixed order (see Update).
public class GameManager : MonoBehaviour
{
    [Header("Tower")]
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Camera gameplayCamera;
    [SerializeField, Min(1f)] private float towerMaxHealth = 100f;
    [SerializeField] private Vector3 towerPosition = Vector3.zero;

    [Header("Spawn")]
    [SerializeField] private EnemySpawnConfig spawnConfig;
    [SerializeField] private Transform enemyRoot;
    [SerializeField] private Transform projectileRoot;
    [Tooltip("How far beyond the screen edge enemies spawn, in viewport units.")]
    [SerializeField, Min(0f)] private float spawnEdgeMargin = 0.06f;
    [SerializeField, Min(1f)] private float spawnMaxDistance = 80f;
    [Tooltip("Fallback ring radius, used only if the camera projection fails.")]
    [SerializeField, Min(1f)] private float spawnRadius = 24f;

    [Header("Spells")]
    [SerializeField] private List<SpellConfig> spellConfigs = new List<SpellConfig>();

    [Header("Visuals")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private DamageNumberVisualConfig damageNumberConfig;

    private EnemiesController enemiesController;
    private EntityViewsController<Enemy, EnemyView> enemyViewsController;
    private EnemySpawnController spawnController;
    private SpellsController spellsController;
    private ProjectilesController projectilesController;
    private EntityViewsController<Projectile, ProjectileView> projectileViewsController;
    private EffectsController effectsController;
    private VisualsController visualsController;
    private EntityViewsController<Visual, VisualView> visualViewsController;
    private ViewportService viewport;
    private GameplayContext gameplayContext;
    private Tower tower;
    private float gameTimer;
    private bool isGameOver;

    public float GameTimer => gameTimer;
    public bool IsGameOver => isGameOver;

    private void Start()
    {
        if (towerPrefab == null)
        {
            throw new MissingReferenceException("Tower prefab is not assigned.");
        }

        if (gameplayCamera == null)
        {
            throw new MissingReferenceException("Gameplay camera is not assigned.");
        }

        if (spawnConfig == null)
        {
            throw new MissingReferenceException("Enemy spawn config is not assigned.");
        }

        tower = new Tower(towerMaxHealth, towerPosition);
        var towerView = Instantiate(towerPrefab, towerPosition, Quaternion.identity).GetComponent<TowerView>();
        if (towerView == null)
        {
            throw new MissingComponentException($"{towerPrefab.name} prefab must have a TowerView component on its root object.");
        }

        towerView.Bind(tower);

        spawnController = new EnemySpawnController(spawnConfig);
        enemiesController = new EnemiesController();
        enemyViewsController = new EntityViewsController<Enemy, EnemyView>(enemyRoot);
        spellsController = new SpellsController();
        projectilesController = new ProjectilesController();
        projectileViewsController = new EntityViewsController<Projectile, ProjectileView>(projectileRoot);
        effectsController = new EffectsController();
        visualsController = new VisualsController(damageNumberConfig);
        visualViewsController = new EntityViewsController<Visual, VisualView>(visualRoot, gameplayCamera);
        viewport = new ViewportService(gameplayCamera);
        gameplayContext = new GameplayContext(
            tower,
            enemiesController,
            spellsController,
            projectilesController,
            effectsController,
            visualsController,
            viewport);

        foreach (var spellConfig in spellConfigs)
        {
            spellsController.AddSpell(spellConfig);
        }
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        var deltaTime = Time.deltaTime;
        gameTimer += deltaTime;

        foreach (var enemyConfig in spawnController.Tick(gameTimer, deltaTime))
        {
            enemiesController.SpawnEnemy(enemyConfig, GetSpawnPosition());
        }

        // Simulation: spells cast, projectiles/effects/enemies advance, then views mirror the models.
        spellsController.Tick(gameplayContext, deltaTime);
        projectilesController.Tick(gameplayContext, deltaTime);
        effectsController.Tick(gameplayContext, deltaTime);
        enemiesController.Tick(gameplayContext, deltaTime);
        visualsController.Tick(deltaTime);

        enemyViewsController.Sync(enemiesController.Enemies);
        projectileViewsController.Sync(projectilesController.Projectiles);
        visualViewsController.Sync(visualsController.Visuals);

        if (tower.Health.IsDead)
        {
            GameOver();
        }
    }

    private Vector3 GetSpawnPosition()
    {
        if (viewport.TryGetOffscreenGroundPoint(towerPosition.y, spawnEdgeMargin, spawnMaxDistance, out var point))
        {
            return point;
        }

        // Fallback: a ring around the tower if every edge sample failed to project onto the ground.
        var angle = Random.Range(0f, Mathf.PI * 2f);
        var offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * spawnRadius;
        return tower.Position + offset;
    }

    private void GameOver()
    {
        isGameOver = true;
    }
}
