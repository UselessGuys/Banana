using Assets.Scripts;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(DynamicObject))]
public class PlayerInput : MonoBehaviour
{
    private Animator _animator;
    private DynamicObject _controller;
    private CharacterStats _stats;
    private SpriteRenderer _renderer;
    private bool _flip;
    private bool _showUseMsg;

    public bool Climbing;
    public bool ClimbingV2;
    public bool ExitLadder;
    public bool EndLadder;

    private void Awake()
    {
        _stats = GetComponent<CharacterStats>();
        _renderer = GetComponent<SpriteRenderer>();
        _controller = GetComponent<DynamicObject>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _showUseMsg = false;
    }

    private void Update()
    {
        _animator.SetFloat("Speed", Mathf.Abs(_controller.Velocity.x));
        _animator.SetBool("Grounded", _controller.Grounded);
        _animator.SetBool("Climbing", Climbing);
        _animator.SetBool("ClimbingV2", ClimbingV2);

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            _flip = true;
        }

        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            _flip = false;
        }

        if (!Climbing)
            _renderer.flipX = _flip;
        

        if (Input.GetButtonDown("Jump"))
            _controller.Jump(_stats.JumpHeight); 

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
            _controller.Gravity = 0;
            _controller.Velocity.x = 0;
            if (!EndLadder || Input.GetAxisRaw("Vertical") < 0)
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
        Debug.Log("Enter " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Ladder"))
        {
            _showUseMsg = true;
        }

        if (collision.gameObject.CompareTag("LadderV2"))
        {
            _showUseMsg = true;
        }

        if (collision.gameObject.CompareTag("ExitLadder"))
        {
            ExitLadder = true;
        }

        if (collision.gameObject.CompareTag("EndLadder") && (Climbing || ClimbingV2))
        {
            EndLadder = true;
            _controller.Velocity.y = 0;
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            if ((Input.GetButtonUp("Use")))
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
            _showUseMsg = false;
        }

        if (collision.gameObject.CompareTag("LadderV2"))
        {
            ClimbingV2 = false;
            _showUseMsg = false;
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
        if (_showUseMsg)
            GUI.Label(new Rect(0, 0, 100, 100), "Нажмите E чтобы взаимодействовать");
    }
}