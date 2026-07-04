// Base for cosmetic views. Shares the pooled entity-view pipeline; concrete visuals (damage
// numbers, explosions) subclass this and override the render hooks.
public abstract class VisualView : EntityView<Visual>
{
}
