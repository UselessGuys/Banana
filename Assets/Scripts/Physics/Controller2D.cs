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
        private const float MaxClimbAngle = 80f;
        private const float MaxDescendAngle = 80f;
        private const float TimeToJumpApex = 0.4f;
        private const float FallingThroughPlatformResetTimer = 0.1f;

        private int _horizontalRayCount;
        private int _verticalRayCount;
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;            
<<<<<<< HEAD
        private bool _isDoubleJumping;      
        private BoxCollider2D _collider;
        private CollisionInfo _collisions;
       




        public LayerMask CollisionMask;        
        public float MaxJumpHeight = 4f;
        public float ClimbSpeed = 3f;
        public bool CanDoubleJump;

        [HideInInspector] public float Gravity;
        [HideInInspector] public Vector2 Velocity;
<<<<<<< HEAD
        [HideInInspector] public float Gravity = -25;
        [HideInInspector] public bool MoveAcrossPlatform { get; set; }

        [HideInInspector] public float ObjectHeight => transform.localScale.y * _collider.size.y; 
        [HideInInspector] public bool Grounded;
        [HideInInspector] public RaycastOrigins RaycastOrigin;
        


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

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();

<<<<<<< HEAD
            Grounded = _collisions.Below;

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

        private void Move(Vector2 moveAmount, Vector2 input = default(Vector2), bool standingOnPlatform = false)
        {
            UpdateRaycastOrigins();
            Collisions.Reset();
            Collisions.MoveAmountOld = moveAmount;
            _playerInput = input;

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

        private void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = Collisions.FaceDirection;
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

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i == 0 && slopeAngle <= MaxClimbAngle)
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

                    if (!Collisions.ClimbingSlope || slopeAngle > MaxClimbAngle)
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
                        if (Collisions.FallingThroughPlatform)
                        {
                            continue;
                        }
                        if (_playerInput.y == -1)
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
            Velocity.x = DirectionalInput.x;
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
}