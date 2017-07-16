using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

#region Player Stats
    public float PlayerMoveSpeed;
    public float PlayerFallSpeed;
    public float PlayerJumpSpeed;
 #endregion

#region Triggers States
    public bool PlayerDownTrigger;
    public bool PlayerUpTrigger;
    public bool PlayerLeftTrigger;
    public bool PlayerRightTrigger;
    #endregion

    private BoxCollider2D ObjDownTrigger;
    private BoxCollider2D ObjUpTrigger;
    private BoxCollider2D ObjLeftTrigger;
    private BoxCollider2D ObjRightTrigger;
    void Start () {
        foreach (Transform t in transform)
        {
            if (t.name == "DownTrigger")
            {
                ObjDownTrigger = t.GetComponent<BoxCollider2D>();
            }
            else if (t.name == "UpTrigger")
            {
                ObjUpTrigger = t.GetComponent<BoxCollider2D>();
            }
            else if (t.name == "LeftTrigger")
            {
                ObjLeftTrigger = t.GetComponent<BoxCollider2D>();
            }
            else if (t.name == "RightTrigger")
            {
                ObjRightTrigger = t.GetComponent<BoxCollider2D>();
            }
        }
    }
	
	// Update is called once per frame
	void Update () {


        if(!PlayerDownTrigger)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - PlayerFallSpeed, this.transform.position.z);
        }

        if(Input.GetKey(KeyCode.A) && !PlayerLeftTrigger)
        {
            this.transform.position = new Vector3(this.transform.position.x - PlayerMoveSpeed, this.transform.position.y, this.transform.position.z);
        }
        if (Input.GetKey(KeyCode.D) && !PlayerRightTrigger)
        {
            this.transform.position = new Vector3(this.transform.position.x + PlayerMoveSpeed, this.transform.position.y, this.transform.position.z);
        }


    }

    void FixedUpdate()
    {
        

    }

}
