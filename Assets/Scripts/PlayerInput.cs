using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerInput : MonoBehaviour
{


    private Animator anim;
    private Controller2D controller;
    private SpriteRenderer rend;
    private bool flip;
    public bool grounded;
    private bool ShowUseMsg;
    public bool Climbing = false;
    public bool ClimbingV2 = false;

    private void Start()
    {
        ShowUseMsg = false;
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

        if ((Climbing || ClimbingV2)) //ToDO сделлать так что бы когда добираешься до верху  персонаж не падал
        {
            if ((Climbing)&&(Input.GetButtonDown("Jump")))
            {
                Climbing = false;
            }

            if ((ClimbingV2) && (Input.GetButtonDown("Jump")))
            {
                ClimbingV2 = false;
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
        if (collision.gameObject.tag == "Ladder")
        {
            ShowUseMsg = true;
        }

        if (collision.gameObject.tag == "LadderV2")
        {
            ShowUseMsg = true;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            if((Input.GetButtonUp("Use")))
            {
                Climbing = true;
                
            }
        }

        if (collision.gameObject.tag == "LadderV2")
        {
            if ((Input.GetButtonUp("Use")))
            {
                ClimbingV2 = true;
                this.transform.position = new Vector2(collision.gameObject.transform.position.x, this.transform.position.y);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            
            Climbing = false;
            ShowUseMsg = false;
        }
        if (collision.gameObject.tag == "LadderV2")
        {
            ClimbingV2 = false;
            ShowUseMsg = false;
        }
    }

    private void OnGUI()
    {
        if (ShowUseMsg)
            GUI.Label(new Rect(0, 0, 100, 100), "Нажмите E чтобы взаимодействовать");
    }
}