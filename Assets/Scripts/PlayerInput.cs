using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerInput : MonoBehaviour
{
<<<<<<< HEAD
    enum ClimbingSide
    {
        None,
        Left,
        Right
    };

    private Player player;
    Animator anim;
    private Controller2D controller;
    private SpriteRenderer rend;
    private bool flip;
    public bool grounded;
    private bool Climbing = false;
    private ClimbingSide CS;
    private void Start()
    {
        CS = ClimbingSide.None;
        player = GetComponent<Player>();
        rend = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
=======
    Animator _anim;
    private Controller2D _controller;
    private SpriteRenderer _rend;
    private bool _flip;
    public bool Grounded;
    private bool _climbing = false;

    private void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
        _controller = GetComponent<Controller2D>();
        _anim = GetComponent<Animator>();
>>>>>>> 150fd966fd4761304fe50942e27ae75633602a74
    }

    private void Update()
        
    {
        Grounded = _controller.Collisions.Below;
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _controller.SetDirectionalInput(directionalInput);
        _anim.SetFloat("Speed", Mathf.Abs(_controller.Velocity.x));
            _anim.SetBool("Grounded", _controller.Collisions.Below);
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            _flip = true;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            _flip = false;
        }
            if (Input.GetButtonDown("Jump"))
        {
            _controller.OnJumpInputDown();
        }
<<<<<<< HEAD
        if (!Climbing) {
            rend.flipX = flip;
        }
        
=======

        _rend.flipX = _flip;
>>>>>>> 150fd966fd4761304fe50942e27ae75633602a74

        if (Input.GetButtonUp("Jump"))
        {
            _controller.OnJumpInputUp();
        }

        if(_climbing)
        {
<<<<<<< HEAD
            player.gravity = 0;
            player.velocity = Vector2.zero;
            player.velocity.y = Input.GetAxisRaw("Vertical") * player.ClimbSpeed;
            player.velocity.x = 0;
            player.SetDirectionalInput(new Vector2(0, Input.GetAxisRaw("Vertical")));
            anim.speed = Mathf.Abs(Input.GetAxisRaw("Vertical")/ player.ClimbSpeed*1.2f);           
        }
        else
        {
            player.gravity = -(2 * player.maxJumpHeight) / Mathf.Pow(player.timeToJumpApex, 2);
            anim.speed = 1;
            player.SetDirectionalInput(directionalInput);
=======
            _controller.Gravity = 0;
            _controller.Velocity = Vector2.zero;
            _controller.Velocity.y = Input.GetAxisRaw("Vertical") * _controller.ClimbSpeed;
            _anim.speed = Mathf.Abs(Input.GetAxisRaw("Vertical")/ _controller.ClimbSpeed*1.2f);           
        }
        else
        {
            _controller.Gravity = -(2 * _controller.MaxJumpHeight) / Mathf.Pow(_controller.TimeToJumpApex, 2);
            _anim.speed = 1;
>>>>>>> 150fd966fd4761304fe50942e27ae75633602a74
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.CompareTag("Ladder"))
        {
<<<<<<< HEAD
            anim.SetBool("Climbing", true);
            Climbing = true;
            if(collision.transform.position.x < this.transform.position.x)
            {
                CS = ClimbingSide.Left;
            }
            else if (collision.transform.position.x > this.transform.position.x)
            {
                CS = ClimbingSide.Right;
            }
=======
            _anim.SetBool("Climbing", true);
            _climbing = true;
>>>>>>> 150fd966fd4761304fe50942e27ae75633602a74
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            _anim.SetBool("Climbing", false);
            _climbing = false;
        }
    }
}