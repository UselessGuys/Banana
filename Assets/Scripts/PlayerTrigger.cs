using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour {

    public bool Triggered = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D obj)
    {
        Destroy(gameObject);
        Destroy(obj.gameObject);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Triggered = false;
    }
}
