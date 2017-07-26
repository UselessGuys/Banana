using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerInput : MonoBehaviour
{
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

        _rend.flipX = _flip;

        if (Input.GetButtonUp("Jump"))
        {
            _controller.OnJumpInputUp();
        }

        if(_climbing)
        {
            _controller.Gravity = 0;
            _controller.Velocity = Vector2.zero;
            _controller.Velocity.y = Input.GetAxisRaw("Vertical") * _controller.ClimbSpeed;
            _anim.speed = Mathf.Abs(Input.GetAxisRaw("Vertical")/ _controller.ClimbSpeed*1.2f);           
        }
        else
        {
            _controller.Gravity = -(2 * _controller.MaxJumpHeight) / Mathf.Pow(_controller.TimeToJumpApex, 2);
            _anim.speed = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.CompareTag("Ladder"))
        {
            _anim.SetBool("Climbing", true);
            _climbing = true;
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