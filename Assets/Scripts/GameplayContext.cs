public class GameplayContext
{
    public GameplayContext(
        Tower tower,
        EnemiesController enemies,
        SpellsController spells,
        ProjectilesController projectiles,
        EffectsController effects,
        VisualsController visuals,
        ViewportService viewport)
    {
        Tower = tower;
        Enemies = enemies;
        Spells = spells;
        Projectiles = projectiles;
        Effects = effects;
        Visuals = visuals;
        Viewport = viewport;
    }

    public Tower Tower { get; }
    public EnemiesController Enemies { get; }
    public SpellsController Spells { get; }
    public ProjectilesController Projectiles { get; }
    public EffectsController Effects { get; }
    public VisualsController Visuals { get; }
    public ViewportService Viewport { get; }
}
