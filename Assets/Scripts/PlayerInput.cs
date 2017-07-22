using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;
    Animator anim;
    private Controller2D controller;
    private SpriteRenderer rend;
    private void Start()
    {
        player = GetComponent<Player>();
        rend = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if(Input.GetAxisRaw("Horizontal") != 0 && controller.collisions.below)
        {
            anim.SetBool("PlayerMove", true);
        }
        else
        {
            anim.SetBool("PlayerMove", false);
        }
        anim.SetBool("Grounded", controller.collisions.below);

        if (Input.GetButtonDown("Jump"))
        {
            player.OnJumpInputDown();
        }

        rend.flipX = (Input.GetAxisRaw("Horizontal") < 0);

        if (Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }
    }
}
