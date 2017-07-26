using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
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
    }

    private void Update()
        
    {
        grounded = controller.collisions.below;
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);
        anim.SetFloat("Speed", Mathf.Abs(player.velocity.x));
            anim.SetBool("Grounded", controller.collisions.below);
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            flip = true;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            flip = false;
        }
            if (Input.GetButtonDown("Jump"))
        {
            player.OnJumpInputDown();
        }
        if (!Climbing) {
            rend.flipX = flip;
        }
        

        if (Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }

        if(Climbing)
        {
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.tag == "Ladder")
        {
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