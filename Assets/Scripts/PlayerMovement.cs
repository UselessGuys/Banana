using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

#region Player Stats
    public float PlayerMoveSpeed;
    public float PlayerFallSpeed;
    public float PlayerJumpSpeed;
    #endregion

#region Triggers State
    public bool PlayerDownTrigger;
    public bool PlayerUpTrigger;
    public bool PlayerLeftTrigger;
    public bool PlayerRightTrigger;
#endregion

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
