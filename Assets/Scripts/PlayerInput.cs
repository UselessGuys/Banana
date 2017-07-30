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
    public bool ExitLadder = false;
    public bool EndLadder = false;
    public GameObject torch;

    private void Start()
    {
        ShowUseMsg = false;
        torch = GameObject.Find("Player/Torch");
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
        {
            flip = true;
        }

        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            flip = false;
        }

        if (Input.GetButtonDown("Jump"))
            controller.OnJumpInputDown();

        if (!Climbing)
            rend.flipX = flip;


        if ((Input.GetButtonUp("Use")))
        {
            Debug.Log("Use");
        }

        if ((Climbing || ClimbingV2)) //ToDO сделлать так что бы когда добираешься до верху  персонаж не падал
        {
            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Horizontal") != 0) && ExitLadder)
            {
                Climbing = false;
                ClimbingV2 = false;
            }
            controller.Gravity = 0;
            controller.Velocity.x = 0;
            controller.SetDirectionalInput(new Vector2(0, Input.GetAxisRaw("Vertical")));
            if (!EndLadder)
            {
                controller.Velocity.y = Input.GetAxisRaw("Vertical") * controller.ClimbSpeed;
            }
            else
            {
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    controller.Velocity.y = Input.GetAxisRaw("Vertical") * controller.ClimbSpeed;
                }
            }
            
            
            anim.speed = Mathf.Abs(controller.Velocity.y /2.2f / controller.ClimbSpeed);
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
        Debug.Log("Enter " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Ladder"))
        {
            ShowUseMsg = true;
        }

        if (collision.gameObject.CompareTag("LadderV2"))
        {
            ShowUseMsg = true;
        }

        if (collision.gameObject.CompareTag("ExitLadder"))
        {
            ExitLadder = true;
        }

        if (collision.gameObject.CompareTag("EndLadder")  && (Climbing || ClimbingV2))
        {
            EndLadder = true;
            controller.Velocity.y = 0;
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            if((Input.GetButtonUp("Use")))
            {
                Climbing = true;
                Debug.Log("Use");
            }
        }



        if (collision.gameObject.CompareTag("LadderV2"))
        {
            if ((Input.GetButtonUp("Use")))
            {
                ClimbingV2 = true;
                this.transform.position = new Vector3(collision.gameObject.transform.position.x, this.transform.position.y, this.transform.position.z);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Ladder"))
        {
            
            Climbing = false;
            ShowUseMsg = false;
        }

        if (collision.gameObject.CompareTag("LadderV2"))
        {
            ClimbingV2 = false;
            ShowUseMsg = false;
        }

        if (collision.gameObject.CompareTag("ExitLadder"))
        {
            ExitLadder = false;
        }

        if (collision.gameObject.CompareTag("EndLadder"))
        {
            EndLadder = false;
            
        }


    }

    private void OnGUI()
    {
        if (ShowUseMsg)
            GUI.Label(new Rect(0, 0, 100, 100), "Нажмите E чтобы взаимодействовать");
    }
}