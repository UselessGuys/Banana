using UnityEngine;

namespace Physics
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Controller2D : MonoBehaviour
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
        private const float DstBetweenRays = .25f;
        private float _maxClimbAngle = 80f;
        private float _maxDescendAngle = 80f;
        private float _moveSpeed = 6f;  
        private bool _isDoubleJumping;
        private float _maxJumpVelocity;
        

        public LayerMask CollisionMask;
        public float FallingThroughPlatformResetTimer = 0.1f;
        public float MaxJumpHeight = 4f;
        public float ClimbSpeed = 3f;
        public float MinJumpHeight = 1f;
        public float TimeToJumpApex = .4f;
        public bool CanDoubleJump;

        [HideInInspector] public RaycastOrigins RaycastOrigin;
        [HideInInspector] public Vector2 DirectionalInput { set; get; }
        [HideInInspector] public CollisionInfo Collisions;
        [HideInInspector] public float Gravity;
        [HideInInspector] public Vector2 Velocity;
        [HideInInspector] public int HorizontalRayCount;
        [HideInInspector] public int VerticalRayCount;
        [HideInInspector] public float HorizontalRaySpacing;
        [HideInInspector] public float VerticalRaySpacing;
        [HideInInspector] public BoxCollider2D Collider;
        [HideInInspector] public Vector2 PlayerInput;
        [HideInInspector] public float ObjectHeight;


        private void Awake()
        {
            Collider = GetComponent<BoxCollider2D>();
            ObjectHeight = transform.localScale.y * Collider.size.y;
        }

        private void Start()
        {
            CalculateRaySpacing();
            Collisions.FaceDirection = 1;

            Gravity = -(2 * MaxJumpHeight) / Mathf.Pow(TimeToJumpApex, 2);
            _maxJumpVelocity = Mathf.Abs(Gravity) * TimeToJumpApex;
        }

        private void Update()
        {
            CalculateVelocity();

            Move(Velocity * Time.deltaTime, DirectionalInput);

            if (Collisions.Above || Collisions.Below)
                Velocity.y = 0f;
        }

        public void Move(Vector2 moveAmount, Vector2 input = default(Vector2), bool standingOnPlatform = false)
        {
            UpdateRaycastOrigins();
            Collisions.Reset();
            Collisions.MoveAmountOld = moveAmount;
            PlayerInput = input;

            if (moveAmount.x != 0)
            {
                Collisions.FaceDirection = (int)Mathf.Sign(moveAmount.x);
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

            if (standingOnPlatform)
            {
                Collisions.Below = true;
            }
        }

        public void Jump()
        {  
            if (Collisions.Below)
            {
                Velocity.y = _maxJumpVelocity;
                _isDoubleJumping = false;
            }
            if (CanDoubleJump && !Collisions.Below && !_isDoubleJumping)
            {
                Velocity.y = _maxJumpVelocity;
                _isDoubleJumping = true;
            }
        }

        private void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = Collisions.FaceDirection;
            float rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;

            if (Mathf.Abs(moveAmount.x) < SkinWidth)
            {
                rayLength = 2 * SkinWidth;
            }

            for (int i = 0; i < HorizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? RaycastOrigin.BottomLeft : RaycastOrigin.BottomRight;
                rayOrigin += Vector2.up * (HorizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

                if (hit && hit.distance > 0)
                {

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i == 0 && slopeAngle <= _maxClimbAngle)
                    {
                        if (Collisions.DescendingSlope)
                        {
                            Collisions.DescendingSlope = false;
                            moveAmount = Collisions.MoveAmountOld;
                        }
                        float distanceToSlopeStart = 0f;
                        if (slopeAngle != Collisions.SlopeAngleOld)
                        {
                            distanceToSlopeStart = hit.distance - SkinWidth;
                            moveAmount.x -= distanceToSlopeStart * directionX;
                        }
                        ClimbSlope(ref moveAmount, slopeAngle);
                        moveAmount.x += distanceToSlopeStart * directionX;
                    }

                    if (!Collisions.ClimbingSlope || slopeAngle > _maxClimbAngle)
                    {
                        moveAmount.x = (hit.distance - SkinWidth) * directionX;
                        rayLength = hit.distance;

                        if (Collisions.ClimbingSlope)
                        {
                            moveAmount.y = Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                        }

                        Collisions.Left = directionX == -1;
                        Collisions.Right = directionX == 1;
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
                Collisions.Below = true;
                Collisions.ClimbingSlope = true;
                Collisions.SlopeAngle = slopeAngle;
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
                if (slopeAngle != 0 && slopeAngle <= _maxDescendAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            Collisions.SlopeAngle = slopeAngle;
                            Collisions.DescendingSlope = true;
                            Collisions.Below = true;
                        }
                    }
                }
            }
        }

        private void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + SkinWidth;

            for (int i = 0; i < VerticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? RaycastOrigin.BottomLeft : RaycastOrigin.TopLeft;
                rayOrigin += Vector2.right * (VerticalRaySpacing * i + moveAmount.x);
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
                        if (Collisions.FallingThroughPlatform)
                        {
                            continue;
                        }
                        if (PlayerInput.y == -1)
                        {
                            Collisions.FallingThroughPlatform = true;
                            Invoke("ResetFallingThroughPlatform", FallingThroughPlatformResetTimer);
                            continue;
                        }
                    }
                    moveAmount.y = (hit.distance - SkinWidth) * directionY;
                    rayLength = hit.distance;

                    if (Collisions.ClimbingSlope)
                    {
                        moveAmount.x = moveAmount.y / Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                    }

                    Collisions.Below = directionY == -1;
                    Collisions.Above = directionY == 1;
                }
            }

            if (Collisions.ClimbingSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;
                Vector2 rayOrigin = ((directionX == -1) ? RaycastOrigin.BottomLeft : RaycastOrigin.BottomRight) + Vector2.up * moveAmount.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != Collisions.SlopeAngle)
                    {
                        moveAmount.x = (hit.distance * SkinWidth) * directionX;
                        Collisions.SlopeAngle = slopeAngle;
                    }
                }
            }
        }

        private void ResetFallingThroughPlatform()
        {
            Collisions.FallingThroughPlatform = false;
        } 

        private void CalculateVelocity()
        {
            float targetVelocityX = DirectionalInput.x * _moveSpeed;
            Velocity.x = targetVelocityX;
            Velocity.y += Gravity * Time.deltaTime;
        }

        private void UpdateRaycastOrigins()
        {
            Bounds bounds = Collider.bounds;
            bounds.Expand(SkinWidth * -2);

            RaycastOrigin.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            RaycastOrigin.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
            RaycastOrigin.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
            RaycastOrigin.TopRight = new Vector2(bounds.max.x, bounds.max.y);
        }
   
        private void CalculateRaySpacing()
        {
            Bounds bounds = Collider.bounds;
            bounds.Expand(SkinWidth * -2);

            float boundsWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;


            HorizontalRayCount = Mathf.RoundToInt(boundsHeight / DstBetweenRays);
            VerticalRayCount = Mathf.RoundToInt(boundsWidth / DstBetweenRays);

            HorizontalRaySpacing = bounds.size.y / (HorizontalRayCount - 1);
            VerticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
        }
    }
}