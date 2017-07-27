using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerInput : MonoBehaviour
{
    public enum ClimbingSide
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
    public ClimbingSide CS;

    private void Start()
    {
        CS = ClimbingSide.None;
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

        if (Climbing)
        {
            controller.Gravity = 0;
            controller.Velocity = Vector2.zero;
            controller.Velocity.y = Input.GetAxisRaw("Vertical") * controller.ClimbSpeed;
            controller.Velocity.x = 0;
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
            anim.SetBool("Climbing", true);
            Climbing = true;
            if (collision.transform.position.x < transform.position.x)
                CS = ClimbingSide.Left;
            else if (collision.transform.position.x > transform.position.x)
                CS = ClimbingSide.Right;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Ladder")
        {
            anim.SetBool("Climbing", false);
            Climbing = false;
        }
    }
}