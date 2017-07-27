using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerInput : MonoBehaviour
{
    private enum ClimbingSide
    {
        None,
        Left,
        Right
    };

    private Animator anim;
    private Controller2D controller;
    private SpriteRenderer rend;
    private bool flip;
    public bool grounded;
    private bool Climbing = false;
    private bool ClimbingV2 = false;
    private ClimbingSide CS;
    private bool CanClimbing = false;

    private void Start()
    {
        CS = ClimbingSide.None;
        if (!Climbing)
        {
            CanClimbing = true;
        }
        rend = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()

    {
        grounded = controller.Collisions.Below;
        var directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        controller.SetDirectionalInput(directionalInput);
        anim.SetFloat("Speed", Mathf.Abs(controller.Velocity.x));
        anim.SetBool("Grounded", controller.Collisions.Below);
        anim.SetBool("Climbing", Climbing);
        anim.SetBool("ClimbingV2", ClimbingV2);
        if (Input.GetAxisRaw("Horizontal") < 0)
            flip = true;
        else if (Input.GetAxisRaw("Horizontal") > 0)
            flip = false;
        if (Input.GetButtonDown("Jump"))
            controller.OnJumpInputDown();
        if (!Climbing)
            rend.flipX = flip;


        if (Input.GetButtonUp("Jump"))
            controller.OnJumpInputUp();

        if ((Climbing || ClimbingV2) && !CanClimbing )
        {
            if ((Climbing)&&(Input.GetButtonDown("Jump") || (Input.GetAxisRaw("Horizontal") > 0 && CS == ClimbingSide.Left) || (Input.GetAxisRaw("Horizontal") < 0 && CS == ClimbingSide.Right)))
            {
                Climbing = false;
                CanClimbing = true;
            }

            if ((ClimbingV2) && (Input.GetButtonDown("Jump") || Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f ))
            {
                ClimbingV2 = false;
                CanClimbing = true;
            }


            controller.Gravity = 0;
            controller.Velocity.x = 0;
            controller.Velocity.y = Input.GetAxisRaw("Vertical") * controller.ClimbSpeed;
            controller.SetDirectionalInput(new Vector2(0, Input.GetAxisRaw("Vertical")));
            anim.speed = Mathf.Abs(Input.GetAxisRaw("Vertical") / controller.ClimbSpeed * 1.2f);
        }
        else
        {
            controller.Gravity = -(2 * controller.MaxJumpHeight) / Mathf.Pow(controller.TimeToJumpApex, 2);
            anim.speed = 1;
            controller.SetDirectionalInput(directionalInput);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Ladder")
        {
            
            Climbing = true;
            CanClimbing = false;
            if (collision.transform.position.x < transform.position.x)
                CS = ClimbingSide.Left;
            else if (collision.transform.position.x > transform.position.x)
                CS = ClimbingSide.Right;
        }

        if (collision.gameObject.tag == "LadderV2")
        {

            ClimbingV2 = true;
            CanClimbing = false;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Ladder")
        {
            Climbing = false;
            CanClimbing = true;
        }
        if (collision.gameObject.tag == "LadderV2")
        {
            ClimbingV2 = false;
            CanClimbing = true;
        }
    }
}