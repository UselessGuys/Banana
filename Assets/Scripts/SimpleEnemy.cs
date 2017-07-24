using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class SimpleEnemy : MonoBehaviour {

    private Player player;
    Animator anim;
    private Controller2D controller;
    private SpriteRenderer rend;
<<<<<<< HEAD
    public float Range = 10f;
    public float Speed = 2f;
    private float ZeroPos;
    private float RightPos;
    private float LeftPos;
    private bool RF = true;

=======
    public float walk;
    public float range = 20f;
    public float t = 2f;
    public float ZeroPos;
    public float LeftPos;
    public bool RF = true;
    public float RightPos;
>>>>>>> 17f6e1095535d21d7f067f8ee4f89229707286b9

    private void Start()
    {
        ZeroPos = this.transform.position.x;
        LeftPos = ZeroPos - Range;
        RightPos = ZeroPos + Range;
        player = this.GetComponent<Player>();
        rend = this.GetComponent<SpriteRenderer>();
        controller = this.GetComponent<Controller2D>();
        anim = this.GetComponent<Animator>();
    }

    private void Update()
    {
        
        Vector2 directionalInput = new Vector2(Speed, 0);
        player.SetDirectionalInput(directionalInput);
        if(this.transform.position.x >= RightPos && RF)
        {
            Speed = -Speed;
            RF = false;
        }

        if (this.transform.position.x <= LeftPos && !RF)
        {
            Speed = -Speed;

            RF = true;
        }
        anim.SetFloat("Speed", Mathf.Abs(player.velocity.x));
        anim.SetBool("Grounded", controller.collisions.below); 

<<<<<<< HEAD
        rend.flipX = !RF;
=======

            rend.flipX = (t < 0);
>>>>>>> 17f6e1095535d21d7f067f8ee4f89229707286b9

    }

}
