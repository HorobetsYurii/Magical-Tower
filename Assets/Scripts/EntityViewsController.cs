using System;
using System.Collections.Generic;
using UnityEngine;

// One generic, pooled view pipeline (synced by id) for enemies, projectiles and visuals alike.
public class EntityViewsController<TModel, TView>
    where TModel : class, IEntity
    where TView : EntityView<TModel>
{
    private readonly Dictionary<int, TView> activeViews = new Dictionary<int, TView>();
    private readonly HashSet<int> liveIds = new HashSet<int>();
    private readonly List<int> staleIds = new List<int>();
    private readonly Dictionary<GameObject, Stack<TView>> pools = new Dictionary<GameObject, Stack<TView>>();
    private readonly Transform parent;
    private readonly Camera camera;

    public EntityViewsController(Transform parent, Camera camera = null)
    {
        this.parent = parent;
        this.camera = camera;
    }

    public void Sync(IReadOnlyList<TModel> models)
    {
        liveIds.Clear();

        foreach (var model in models)
        {
            liveIds.Add(model.Id);

            if (!activeViews.TryGetValue(model.Id, out var view))
            {
                view = Rent(model.Prefab, model.Position);
                view.Bind(model);
                activeViews.Add(model.Id, view);
            }

            view.SyncFromModel();
        }

        ReleaseStaleViews();
    }

    private void ReleaseStaleViews()
    {
        staleIds.Clear();

        foreach (var pair in activeViews)
        {
            if (!liveIds.Contains(pair.Key))
            {
                staleIds.Add(pair.Key);
            }
        }

        foreach (var id in staleIds)
        {
            Return(activeViews[id]);
            activeViews.Remove(id);
        }
    }

    private TView Rent(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
        {
            throw new ArgumentNullException(nameof(prefab), $"{typeof(TModel).Name} has no prefab assigned in its config.");
        }

        if (!pools.TryGetValue(prefab, out var pool))
        {
            pool = new Stack<TView>();
            pools.Add(prefab, pool);
        }

        var view = pool.Count > 0 ? pool.Pop() : Create(prefab);
        var viewTransform = view.transform;
        viewTransform.SetParent(parent, false);
        viewTransform.position = position;
        viewTransform.rotation = Quaternion.identity;
        view.gameObject.SetActive(true);
        return view;
    }

    private void Return(TView view)
    {
        view.Unbind();
        view.gameObject.SetActive(false);

        if (!pools.TryGetValue(view.PoolKey, out var pool))
        {
            pool = new Stack<TView>();
            pools.Add(view.PoolKey, pool);
        }

        pool.Push(view);
    }

    private TView Create(GameObject prefab)
    {
        var instance = UnityEngine.Object.Instantiate(prefab, parent);
        var view = instance.GetComponent<TView>();
        if (view == null)
        {
            throw new MissingComponentException($"{prefab.name} prefab must have a {typeof(TView).Name} component on its root object.");
        }

        view.Configure(prefab, camera);
        return view;
    }
}
