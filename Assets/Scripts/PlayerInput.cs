using Assets.Scripts;
using Physics;
using UnityEngine;
[RequireComponent(typeof(Controller2D))]
public class PlayerInput : MonoBehaviour
{
    private Animator anim;
    private Controller2D controller;
    private CharacterStats _stats;
    private SpriteRenderer rend;
    private bool flip;
    private bool ShowUseMsg;

    public bool Climbing;
    public bool ClimbingV2;
    public bool ExitLadder;
    public bool EndLadder;

    private void Awake()
    {
        _stats = GetComponent<CharacterStats>();
        rend = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        ShowUseMsg = false;   
    }

    private void Update()
    {

        anim.SetFloat("Speed", Mathf.Abs(controller.Velocity.x));
        anim.SetBool("Grounded", controller.Grounded);
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
            controller.Jump(_stats.JumpHeight);

        if (!Climbing)
            rend.flipX = flip;


        if ((Input.GetButtonUp("Use")))
        {
            Debug.Log("Use");
        }

        if ((Climbing || ClimbingV2)) 
        {
            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Horizontal") != 0) && ExitLadder)
            {
                Climbing = false;
                ClimbingV2 = false;
            }
            controller.Gravity = 0;
            controller.Velocity.x = 0;
            if (!EndLadder || Input.GetAxisRaw("Vertical") < 0)
            {
                controller.Velocity.y = Input.GetAxisRaw("Vertical") * _stats.MoveSpeed;
            }
            
            
            anim.speed = Mathf.Abs(controller.Velocity.y /2.2f / _stats.MoveSpeed);
        }
        else
        {
            controller.Gravity = -25;
            anim.speed = 1;
            controller.MoveAcrossPlatform = Input.GetAxisRaw("Vertical") == -1;
            controller.Velocity.x = Input.GetAxisRaw("Horizontal") * _stats.MoveSpeed;
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