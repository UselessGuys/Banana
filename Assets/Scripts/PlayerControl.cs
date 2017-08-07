using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(DynamicObject))]
public class PlayerControl : MonoBehaviour
{
    public enum LadderSide
    {
        Right,
        Left
    }
    private Animator _animator;
    private DynamicObject _controller;
    private CharacterStats _stats;
    private SpriteRenderer _renderer;
    private LadderSide _ladderSide;
    private bool _climbing;
    private bool _climbingV2;
    private bool _exitLadder;
    private bool _enterLadder;
    private bool _endLadder;

    private void Awake()
    {
        _stats = GetComponent<CharacterStats>();
        _renderer = GetComponent<SpriteRenderer>();
        _controller = GetComponent<DynamicObject>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        _animator.SetFloat("Speed", Mathf.Abs(_controller.Velocity.x));
        _animator.SetBool("Grounded", _controller.Grounded);
        _animator.SetBool("Climbing", _climbing);
        _animator.SetBool("ClimbingV2", _climbingV2);


        if (!_climbing)
        {
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                _renderer.flipX = true;
            }
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                _renderer.flipX = false;
            }
        }


        if (Input.GetButtonDown("Jump"))
        {
            _controller.Jump(_stats.JumpHeight);
        }

        if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Horizontal") != 0) && (_exitLadder || _enterLadder) && _climbingV2)
        {
            _climbingV2 = false;
            _controller.Velocity.y = _stats.JumpHeight;
        }


        if ((Input.GetButtonDown("Jump") || ((Input.GetAxisRaw("Horizontal") > 0) && (_ladderSide == LadderSide.Left)) || ((Input.GetAxisRaw("Horizontal") < 0) && (_ladderSide == LadderSide.Right))) && (_exitLadder || _enterLadder) && _climbing)
        {
            _climbing = false;
            _controller.Velocity.y = _stats.JumpHeight;
        }

        if ((_climbing || _climbingV2))
        {

            _controller.Gravity = 0;
            _controller.Velocity.x = 0;
            if (_endLadder)
            {
               _controller.Velocity.y = 0;
            }
            if (!_endLadder || Input.GetAxisRaw("Vertical") < 0)
            {
                _controller.Velocity.y = Input.GetAxisRaw("Vertical") * _stats.MoveSpeed;
            }


            _animator.speed = Mathf.Abs(_controller.Velocity.y / 2.2f / _stats.MoveSpeed);
        }
        else
        {
            _controller.Gravity = -25;
            _animator.speed = 1;
            _controller.MoveAcrossPlatform = Input.GetAxisRaw("Vertical") == -1;
            _controller.Velocity.x = Input.GetAxisRaw("Horizontal") * _stats.MoveSpeed;
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            if (collision.transform.position.x < this.transform.position.x)
            {
                _ladderSide = LadderSide.Left;
                
            }
            else
            {
                _ladderSide = LadderSide.Right;
            }
        }

        if (collision.gameObject.CompareTag("LadderV2"))
        {
           
        }

        if (collision.gameObject.CompareTag("ExitLadder"))
        {
            _exitLadder = true;
        }

        if (collision.gameObject.CompareTag("EnterLadder"))
        {
            _enterLadder = true;
        }

        if (collision.gameObject.CompareTag("EndLadder"))
        {
            _endLadder = true;
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            if ((((Input.GetAxisRaw("Horizontal") < 0) && (_ladderSide == LadderSide.Left)) || ((Input.GetAxisRaw("Horizontal") > 0) && (_ladderSide == LadderSide.Right))) && !_exitLadder)
            {
                _climbing = true;
            }
        }



        if (collision.gameObject.CompareTag("LadderV2"))
        {
            if ((Input.GetAxisRaw("Vertical") > 0.1f) && !_exitLadder)
            {
                _climbingV2 = true;
                this.transform.position = new Vector3(collision.gameObject.transform.position.x, this.transform.position.y, this.transform.position.z);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {

            _climbing = false;
           
        }

        if (collision.gameObject.CompareTag("LadderV2"))
        {
            _climbingV2 = false;
            
        }

        if (collision.gameObject.CompareTag("ExitLadder"))
        {
            _exitLadder = false;
        }

        if (collision.gameObject.CompareTag("EnterLadder"))
        {
            _enterLadder = false;
        }

        if (collision.gameObject.CompareTag("EndLadder"))
        {
            _endLadder = false;

        }

    }

    private void OnGUI()
    {
       
    }
}