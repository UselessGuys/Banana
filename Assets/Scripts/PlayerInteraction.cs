﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private bool one_click = false;
    private bool timer_running;

    public float timer_for_double_click;
   
    void Start()
    {

    }
    
    void Update()
    {
        float delay = 0.5f;

        if (Input.GetMouseButtonDown(0))
        {
            if (!one_click) 
            {
                one_click = true;
                timer_for_double_click = Time.time; 
            }
            else
            {

                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.01f);
                one_click = false;
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("NPC"))
                    {
                        hit.collider.GetComponent<NPC>().Interaction();
                    }
                }
            }
        }
        if (one_click)
        {
            if ((Time.time - timer_for_double_click) > delay)
            {
                one_click = false;
            }
        }


    }
}
