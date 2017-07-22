using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class SimpleEnemy : MonoBehaviour {

    private Player player;
    Animator anim;
    private Controller2D controller;
    private SpriteRenderer rend;
    public float walk;
    public float range = 20f;
    public float t = 2f;
    public float ZeroPos;
    public float LeftPos;
    public bool RF = true;
    public float RightPos;
    private void Start()
    {
        ZeroPos = this.transform.position.x;
        LeftPos = ZeroPos - range;
        RightPos = ZeroPos + range;
        player = this.GetComponent<Player>();
        rend = this.GetComponent<SpriteRenderer>();
        controller = this.GetComponent<Controller2D>();
        anim = this.GetComponent<Animator>();
    }

    private void Update()
    {
        
        Vector2 directionalInput = new Vector2(t, 0);
        player.SetDirectionalInput(directionalInput);
        Debug.Log(Mathf.Abs(this.transform.position.x - ZeroPos));
        if(this.transform.position.x >= RightPos && RF)
        {
            t = -t;
            RF = false;
        }

        if (this.transform.position.x <= LeftPos && !RF)
        {
            t = -t;

            RF = true;
        }
        anim.SetFloat("Speed", Mathf.Abs(player.velocity.x));
        anim.SetBool("Grounded", controller.collisions.below);

       

        rend.flipX = (t < 0);

    }
}
