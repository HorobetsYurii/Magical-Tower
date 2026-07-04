using UnityEngine;

// Pooled view that mirrors a model onto its transform; subclasses override the render hooks.
public abstract class EntityView<TModel> : MonoBehaviour where TModel : class, IEntity
{
    private TModel model;

    public GameObject PoolKey { get; private set; }
    protected Camera ViewCamera { get; private set; }

    public void Configure(GameObject poolKey, Camera camera)
    {
        PoolKey = poolKey;
        ViewCamera = camera;
    }

    public void Bind(TModel value)
    {
        model = value;
        OnBind(value);
        SyncFromModel();
    }

    public void Unbind()
    {
        model = null;
    }

    public void SyncFromModel()
    {
        if (model == null)
        {
            return;
        }

        transform.position = model.Position;
        OnSync(model);
    }

    protected virtual void OnBind(TModel value) { }
    protected virtual void OnSync(TModel value) { }
}
