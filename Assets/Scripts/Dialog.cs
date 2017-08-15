using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.UIElements;


public class Dialog : MonoBehaviour
{
    [HideInInspector] public Node _tmp;

    [HideInInspector] public List<Node> _dialogs;

    [HideInInspector] public List<Rect> _windows;

    public Dictionary<int, List<int>> _links = new Dictionary<int, List<int>>();

    void Start ()
    {

        for (int i = 0; i < _dialogs.Count; i++)
        {
            if (_dialogs[i].begin)
            {
                _tmp = _dialogs[i];
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
	{

	}

    void OnGUI()
    {
        
        GUI.Label(new Rect(10, 10, 100, 20), _tmp.Text);

    }
}
