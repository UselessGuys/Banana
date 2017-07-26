using UnityEngine;

public class Controller2D : RaycastController
{
    public float FallingThroughPlatformResetTimer = 0.1f;
    private float _maxClimbAngle = 80f;
    private float _maxDescendAngle = 80f;

    public CollisionInfo Collisions;
    [HideInInspector] public Vector2 PlayerInput;

    public float MaxJumpHeight = 4f;
    public float ClimbSpeed = 3f;
    public float MinJumpHeight = 1f;
    public float TimeToJumpApex = .4f;
    private float _accelerationTimeAirborne = .2f;
    private float _accelerationTimeGrounded = .1f;
    private float _moveSpeed = 6f;

    private Vector2 _wallJumpClimb;
    private Vector2 _wallJumpOff;
    private Vector2 _wallLeap;

    public bool CanDoubleJump;
    private bool _isDoubleJumping = false;

    private float _wallSlideSpeedMax = 100f;
    private float _wallStickTime = 0f;
    private float _timeToWallUnstick;

    public float Gravity;
    private float _maxJumpVelocity;
    private float _minJumpVelocity;
    public Vector3 Velocity;
    private float _velocityXSmoothing;

    private Vector2 _directionalInput;
    private bool _wallSliding;
    private int _wallDirX;

    public override void Start()
    {
        base.Start();
        Collisions.FaceDir = 1;

        Gravity = -(2 * MaxJumpHeight) / Mathf.Pow(TimeToJumpApex, 2);
        _maxJumpVelocity = Mathf.Abs(Gravity) * TimeToJumpApex;
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Gravity) * MinJumpHeight);
    }

    private void Update()
    {
        CalculateVelocity();

        Move(Velocity * Time.deltaTime, _directionalInput);

        if (Collisions.Above || Collisions.Below)
        {
            Velocity.y = 0f;
        }
    }

    public void Move(Vector2 moveAmount, bool standingOnPlatform = false)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        Collisions.Reset();
        Collisions.MoveAmountOld = moveAmount;
        PlayerInput = input;

        if (moveAmount.x != 0)
        {
            Collisions.FaceDir = (int)Mathf.Sign(moveAmount.x);
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
        float directionX = Collisions.FaceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;

        if (Mathf.Abs(moveAmount.x) < SkinWidth)
        {
            rayLength = 2 * SkinWidth;
        }

        for (int i = 0; i < HorizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.BottomLeft : raycastOrigins.BottomRight;
            rayOrigin += Vector2.up * (HorizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if (hit.distance == 0)
                {
                    continue;
                }

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
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.BottomRight : raycastOrigins.BottomLeft;
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
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.BottomLeft : raycastOrigins.TopLeft;
            rayOrigin += Vector2.right * (VerticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, CollisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "Through")
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
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.BottomLeft : raycastOrigins.BottomRight) + Vector2.up * moveAmount.y;
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

    public struct CollisionInfo
    {
        public bool Above, Below;
        public bool Left, Right;

        public bool ClimbingSlope;
        public bool DescendingSlope;
        public float SlopeAngle, SlopeAngleOld;
        public Vector2 MoveAmountOld;
        public int FaceDir;
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

    public void SetDirectionalInput(Vector2 input)
    {
        _directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (_wallSliding)
        {
            if (_wallDirX == _directionalInput.x)
            {
                Velocity.x = -_wallDirX * _wallJumpClimb.x;
                Velocity.y = _wallJumpClimb.y;
            }
            else if (_directionalInput.x == 0)
            {
                Velocity.x = -_wallDirX * _wallJumpOff.x;
                Velocity.y = _wallJumpOff.y;
            }
            else
            {
                Velocity.x = -_wallDirX * _wallLeap.x;
                Velocity.y = _wallLeap.y;
            }
            _isDoubleJumping = false;
        }
        if (Collisions.Below)
        {
            Velocity.y = _maxJumpVelocity;
            _isDoubleJumping = false;
        }
        if (CanDoubleJump && !Collisions.Below && !_isDoubleJumping && !_wallSliding)
        {
            Velocity.y = _maxJumpVelocity;
            _isDoubleJumping = true;
        }
    }

    public void OnJumpInputUp()
    {
        if (Velocity.y > _minJumpVelocity)
        {
            Velocity.y = _minJumpVelocity;
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = _directionalInput.x * _moveSpeed;
        Velocity.x = targetVelocityX;
        Velocity.y += Gravity * Time.deltaTime;
    }
}