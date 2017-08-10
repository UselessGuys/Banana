#if UNITY_EDITOR
using UnityEditor;
#endif 
using UnityEngine;

namespace Tools
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Enemy))]
    public class MonsterEditor : Editor
    {
        Vector2 _scrollPos = new Vector2(0, Mathf.Infinity);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Enemy monster = (Enemy)target;

            if (Application.isPlaying)
            {
                _scrollPos = GUILayout.BeginScrollView(
                    _scrollPos, GUILayout.Height(250));

                GUILayout.Label(monster.name + " " + monster.State);

                GUILayout.EndScrollView();

                if (GUILayout.Button("Clear"))
                    monster.name = "";
            }

            serializedObject.ApplyModifiedProperties();
            DrawDefaultInspector();
        }
    }
#endif
}
