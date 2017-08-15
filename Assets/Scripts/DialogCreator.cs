using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Dialog))]
public class DialogCreator : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Dialog go = (Dialog) target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Open Dialog Window"))
        {
            DialogCreateWindows window = (DialogCreateWindows)EditorWindow.GetWindow(typeof(DialogCreateWindows));
            window.Show();
            
        }
        GUILayout.EndHorizontal();
        if(go._links != null)
        foreach (KeyValuePair<int, List<int>> entry in go._links)
        {
            string str = "Key " + entry.Key +" Values";
            if (entry.Value.Count > 0)
            {
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    str += " " + entry.Value[i];
                }
            }
            GUILayout.Label(str);

        }
    }
}
