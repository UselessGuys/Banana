using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask CollisionMask;

    public const float SkinWidth = .015f;
    private const float DstBetweenRays = .25f;

    [HideInInspector]
    public int HorizontalRayCount;
    [HideInInspector]
    public int VerticalRayCount;

    [HideInInspector]
    public float HorizontalRaySpacing;
    [HideInInspector]
    public float VerticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D Coll;

    [HideInInspector]
    public RaycastOrigins raycastOrigins;

    public virtual void Awake()
    {
        Coll = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = Coll.bounds;
        bounds.Expand(SkinWidth * -2);

        raycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = Coll.bounds;
        bounds.Expand(SkinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;


        HorizontalRayCount = Mathf.RoundToInt(boundsHeight / DstBetweenRays);
        VerticalRayCount = Mathf.RoundToInt(boundsWidth / DstBetweenRays);

        HorizontalRaySpacing = bounds.size.y / (HorizontalRayCount - 1);
        VerticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 TopLeft, TopRight;
        public Vector2 BottomLeft, BottomRight;
    }
}
