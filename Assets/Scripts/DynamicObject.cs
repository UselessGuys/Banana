// Velocity, Gravity, MoveAcrossPlatform
// Method "Jump"

using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DynamicObject : MonoBehaviour
{
    public struct RaycastOrigins
    {
        public Vector2 TopLeft, TopRight;
        public Vector2 BottomLeft, BottomRight;
    }

    public struct CollisionInfo
    {
        public bool Above, Below;
        public bool Left, Right;
        public bool ClimbingSlope;
        public bool DescendingSlope;
        public float SlopeAngle, SlopeAngleOld;
        public Vector2 MoveAmountOld;
        public int FaceDirection;
        public bool FallingThroughPlatform;

        public void Reset()
        {
            Above = Below = false;
            Left = Right = false;
            ClimbingSlope = false;
            DescendingSlope = false;

            SlopeAngleOld = SlopeAngle;
            SlopeAngle = 0f;
        }
    }
    private const float SkinWidth = .015f;
    private const float DstBetweenRays = .20f;
    private const float MaxClimbAngle = 60f;
    private const float MaxDescendAngle = 80f;
    private const float FallingThroughPlatformResetTimer = 0.1f;

    private int _horizontalRayCount;
    private int _verticalRayCount;
    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;
    private bool _isDoubleJumping;
    private BoxCollider2D _collider;
    private CollisionInfo _collisions;

    public const float MaxHorisontalLadder = .4f;

    public LayerMask CollisionMask; 
    public bool CanDoubleJump;

    [HideInInspector] public Vector2 Velocity;
    [HideInInspector] public float Gravity;
    [HideInInspector] public bool MoveAcrossPlatform { get; set; }

    [HideInInspector] public float ObjectHeight { get; private set; } //ToDo проект в 6 Шарп, кттс
    [HideInInspector] public bool Grounded;
    [HideInInspector] public RaycastOrigins RaycastOrigin;


    public void Jump(float jumpHeight)
    {
        if (_collisions.Below)
        {
            Velocity.y = jumpHeight;
            _isDoubleJumping = false;
        }
        if (CanDoubleJump && !_collisions.Below && !_isDoubleJumping)
        {
            Velocity.y = jumpHeight;
            _isDoubleJumping = true;
        }
    }

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();

        ObjectHeight = transform.localScale.y * _collider.size.y;       
    }

    private void Start()
    {
        CalculateRaySpacing();
        _collisions.FaceDirection = 1;

        Gravity = -25;

    }

    private void Update()
    {
        CalculateVelocity();
        Grounded = _collisions.Below;

        Move(Velocity * Time.deltaTime);

        if (_collisions.Above || _collisions.Below)
            Velocity.y = 0f;
    }

    private void Move(Vector2 moveAmount)
    {
        UpdateRaycastOrigins();
        _collisions.Reset();
        _collisions.MoveAmountOld = moveAmount;

        if (moveAmount.x != 0)
        {
            _collisions.FaceDirection = (int)Mathf.Sign(moveAmount.x);
        }

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        HorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);
    }

    private void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = _collisions.FaceDirection;
        float rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;

        if (Mathf.Abs(moveAmount.x) < SkinWidth)
        {
            rayLength = 2 * SkinWidth;
        }


        for (int i = 0; i < _horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? RaycastOrigin.BottomLeft : RaycastOrigin.BottomRight;
            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit && hit.distance > 0)
            {
                if (hit.distance < SkinWidth * 2)
                    if (hit.collider.gameObject.transform.lossyScale.y < MaxHorisontalLadder && i == 0 && Grounded)
                           transform.Translate(directionX * SkinWidth, hit.collider.gameObject.transform.lossyScale.y, 0);



                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= MaxClimbAngle)
                {
                    if (_collisions.DescendingSlope)
                    {
                        _collisions.DescendingSlope = false;
                        moveAmount = _collisions.MoveAmountOld;
                    }
                    float distanceToSlopeStart = 0f;
                    if (slopeAngle != _collisions.SlopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - SkinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!_collisions.ClimbingSlope || slopeAngle > MaxClimbAngle)
                {
                    moveAmount.x = (hit.distance - SkinWidth) * directionX;
                    rayLength = hit.distance;

                    if (_collisions.ClimbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(_collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    _collisions.Left = directionX == -1;
                    _collisions.Right = directionX == 1;
                }
            }
        }
    }

    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            _collisions.Below = true;
            _collisions.ClimbingSlope = true;
            _collisions.SlopeAngle = slopeAngle;
        }

    }

    private void DescendSlope(ref Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        Vector2 rayOrigin = (directionX == -1) ? RaycastOrigin.BottomRight : RaycastOrigin.BottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, CollisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= MaxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                    {
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descendmoveAmountY;

                        _collisions.SlopeAngle = slopeAngle;
                        _collisions.DescendingSlope = true;
                        _collisions.Below = true;
                    }
                }
            }
        }
    }

    private void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + SkinWidth;

        for (int i = 0; i < _verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? RaycastOrigin.BottomLeft : RaycastOrigin.TopLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, CollisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.CompareTag("Through"))
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (_collisions.FallingThroughPlatform)
                    {
                        continue;
                    }
                    if (MoveAcrossPlatform)
                    {
                        _collisions.FallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", FallingThroughPlatformResetTimer);
                        continue;
                    }
                }
                moveAmount.y = (hit.distance - SkinWidth) * directionY;
                rayLength = hit.distance;

                if (_collisions.ClimbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(_collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                _collisions.Below = directionY == -1;
                _collisions.Above = directionY == 1;
            }
        }

        if (_collisions.ClimbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? RaycastOrigin.BottomLeft : RaycastOrigin.BottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != _collisions.SlopeAngle)
                {
                    moveAmount.x = (hit.distance * SkinWidth) * directionX;
                    _collisions.SlopeAngle = slopeAngle;
                }
            }
        }
    }

    private void ResetFallingThroughPlatform()
    {
        _collisions.FallingThroughPlatform = false;
    }

    private void CalculateVelocity()
    {
        Velocity.y += Gravity * Time.deltaTime;
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(SkinWidth * -2);

        RaycastOrigin.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        RaycastOrigin.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        RaycastOrigin.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        RaycastOrigin.TopRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(SkinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;


        _horizontalRayCount = Mathf.RoundToInt(boundsHeight / DstBetweenRays);
        _verticalRayCount = Mathf.RoundToInt(boundsWidth / DstBetweenRays);

        _horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
    }
}