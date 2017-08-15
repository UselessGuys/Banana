using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class DialogCreateWindows : EditorWindow
{
    public static List<Rect> _windows = new List<Rect>();
    public static List<Node> _dialogs = new List<Node>();
    public static Dictionary<int,List<int>> _links = new Dictionary<int,List<int>>();
    int _choiceIndex = 0;
    void OnEnable()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            Dialog component = go.GetComponent<Dialog>();
            if (component == null)
                continue;

            _dialogs = component._dialogs;
            _windows = component._windows;
            _links = component._links;
            EditorUtility.SetDirty(component);
        }
        if (_links == null || _links.Count != _windows.Count)
        {
			_links = new Dictionary<int,List<int>>();
        }
        if (_dialogs != null)
        {
            if (_windows.Count != _dialogs.Count)
            {
                _windows.Clear();
                for (int i = 0; i < _dialogs.Count; i++)
                {
                    _windows.Add(new Rect(100, 100, 300, 500));
                }
            }
        }
        else
        {
            _dialogs = new List<Node>();
            _windows = new List<Rect>();
			_links = new Dictionary<int,List<int>>();
        }

    }



    private void OnGUI()
    {
        
        if (GUILayout.Button("Create window"))
        {
            _links.Add(_windows.Count, new List<int>());
            _windows.Add(new Rect(100,100,300,500));
            _dialogs.Add(new Node());
        }
        for (int i = 0; i < _windows.Count; i++)
        {
            if (!_links.ContainsKey(i))
            {
                _links.Add(i,new List<int>());
            }
        }
        Handles.BeginGUI();
        foreach (KeyValuePair<int, List<int>> entry in _links)
        {
            if (entry.Value.Count > 0)
            {
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    Handles.DrawLine(_windows[entry.Key].center, _windows[entry.Value[i]].center);
                }
            }

        }

        Handles.EndGUI();

        BeginWindows();
        if(_windows.Count > 0)
        for (int i = 0; i < _windows.Count; i++)
        {
            _windows[i] = GUI.Window(i, _windows[i], WindowFunction, "Box " + i);
        }

        EndWindows();
        foreach (GameObject go in Selection.gameObjects)
        {
            Dialog component = go.GetComponent<Dialog>();


            if (component == null)
                continue;

            component._dialogs = _dialogs;
            component._links = _links;
            component._windows = _windows;
            EditorUtility.SetDirty(component);
        }
    }
    void WindowFunction(int windowID)
    {
        _dialogs[windowID].ButtonName = GUILayout.TextField(_dialogs[windowID].ButtonName);
        _dialogs[windowID].Text = GUILayout.TextArea(_dialogs[windowID].Text, GUILayout.Height(200));
        _dialogs[windowID].begin = GUILayout.Toggle(_dialogs[windowID].begin, "Begin?");
        _dialogs[windowID].end = GUILayout.Toggle(_dialogs[windowID].end,"End?");
        string[] _choices = new string[_windows.Count];
        for (int i = 0; i < _choices.Length; i++)
        {
            _choices[i] = _dialogs[i].ButtonName;
        }
        _choiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);
        if (GUILayout.Button("Add to answers"))
        {

            _links[windowID].Add(_choiceIndex);
        }

        GUILayout.Label("Answers:");
        if(_links.Count > 0)
            if (_links[windowID].Count > 0)
            {
                for (int i = 0; i < _links[windowID].Count; i++)
                {
                    GUILayout.Label(_dialogs[_links[windowID][i]].ButtonName);
                }
            }

        

        if (GUILayout.Button("Delete Window"))
        {
            
			_links.Remove (windowID);
			foreach(KeyValuePair<int, List<int>> entry in _links)
			{
				if (entry.Value.Contains(windowID))
				{
					_links [entry.Key].Remove (windowID);
				}
					
			}

            _windows.RemoveAt(windowID);
            _dialogs.RemoveAt(windowID);
        }
        GUI.DragWindow();
    }
}
