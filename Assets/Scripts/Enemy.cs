using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    enum EnemyStates
    {
        Idle,
        Patrol,
        Attack
    }

    internal class Enemy : MonoBehaviour
    {
        public int LeftBorder;
        public int RightBorder;
        public bool RightDirection;

        private EdgeCollider2D _visionArea;

        void Awake()
        {
            _visionArea = GetComponent<EdgeCollider2D>();
        }

        void Update()
        {
            
            if (_visionArea.)
        }

        void FixedUpdate()
        {
            
        }
    }
}
