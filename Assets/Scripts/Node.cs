using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node
{
    [SerializeField] public string ButtonName;

    [SerializeField] public string Text;
    [SerializeField] public bool end = false;
    [SerializeField] public bool begin = false;

    public Node(string btn, string t, bool end, bool begin)
    {
        this.ButtonName = btn;
        this.Text = t;
        this.end = end;
        this.begin = begin;
    }
    public Node()
    {
    }
}
