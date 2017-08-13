using UnityEditor;
using UnityEngine;

namespace Tools
{
    [CustomEditor(typeof(IEnemy))]
    public class MonsterEditor : Editor
    {
        Vector2 _scrollPos = new Vector2(0, Mathf.Infinity);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            IEnemy monster = (IEnemy)target;

            if (Application.isPlaying)
            {
                _scrollPos = GUILayout.BeginScrollView(
                    _scrollPos, GUILayout.Height(250));

                GUILayout.Label("Something Info");

                GUILayout.EndScrollView();

                if (GUILayout.Button("Clear"))
                    monster.name = "";
            }

            serializedObject.ApplyModifiedProperties();
            DrawDefaultInspector();
        }
    }
}
