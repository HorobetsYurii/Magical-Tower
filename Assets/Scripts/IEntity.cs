using UnityEngine;

// Implemented by Enemy, Projectile and Visual so one pooled view pipeline serves all three.
public interface IEntity
{
    int Id { get; }
    Vector3 Position { get; }
    GameObject Prefab { get; }
}
