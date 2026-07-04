using UnityEngine;

// Camera-aware screen queries: on-screen visibility and off-screen spawn points on the ground.
public class ViewportService
{
    private readonly Camera camera;

    public ViewportService(Camera camera)
    {
        this.camera = camera;
    }

    public bool IsVisible(IPositionable target)
    {
        var viewportPosition = camera.WorldToViewportPoint(target.Position);
        return viewportPosition.z > 0f &&
               viewportPosition.x >= 0f && viewportPosition.x <= 1f &&
               viewportPosition.y >= 0f && viewportPosition.y <= 1f;
    }

    // A random ground point just past a screen edge, so enemies enter from off-screen on every side
    // regardless of camera angle (a fixed radius wouldn't). False if no edge projected onto the ground.
    public bool TryGetOffscreenGroundPoint(float groundY, float edgeMargin, float maxDistance, out Vector3 point)
    {
        var ground = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));

        for (var attempt = 0; attempt < 8; attempt++)
        {
            var ray = camera.ViewportPointToRay(RandomEdgePoint(edgeMargin));
            if (ground.Raycast(ray, out var distance) && distance <= maxDistance)
            {
                point = ray.GetPoint(distance);
                point.y = groundY;
                return true;
            }
        }

        point = default;
        return false;
    }

    private static Vector3 RandomEdgePoint(float margin)
    {
        var along = Mathf.Lerp(-margin, 1f + margin, Random.value);
        switch (Random.Range(0, 4))
        {
            case 0: return new Vector3(-margin, along, 0f);       // left
            case 1: return new Vector3(1f + margin, along, 0f);   // right
            case 2: return new Vector3(along, -margin, 0f);       // bottom
            default: return new Vector3(along, 1f + margin, 0f);  // top
        }
    }
}
