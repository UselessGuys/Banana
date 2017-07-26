using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    enum EnemyStates
    {
        Stay,
        Patrol,
        Attack
    }

    internal class Enemy : MonoBehaviour
    {
        public int LeftBorder;
        public int RightBorder;
        public bool RightDirection;
        public EnemyStates State;
        public Animator Animator;

        private EdgeCollider2D _visionArea;
        private CharacterStats _stats;
        private Controller2D _controller;   
        private SpriteRenderer _renderer;

        private bool _rightDirection;
        private int _currentSpeed;

        

        void Awake()
        {
            _visionArea = GetComponent<EdgeCollider2D>();
            _stats = GetComponent<CharacterStats>();
            _controller = this.GetComponent<Controller2D>();
        }

        void Start()
        {
            var random = new System.Random();
            _rightDirection = random.Next(1) == 1;
            //ToDo Все мобы пойдут в одну сторону может быть.

            _currentSpeed = _stats.MoveSpeed;
        }

        void Update()
        {
<<<<<<< HEAD
            
            
=======
            if (transform.position.x > RightBorder)
                _rightDirection = false;
            if (transform.position.x < LeftBorder)
                _rightDirection = true;

            CheckNoise();

>>>>>>> 150fd966fd4761304fe50942e27ae75633602a74
        }

        void FixedUpdate()
        {
            if (State == EnemyStates.Patrol)
                Move();
        }

        

        void OnTriggerEnter2D(Collider2D trigger)
        {
            if (trigger.gameObject.tag == "Player")
            {
                State = EnemyStates.Attack;
            }
        }

        private void Move()
        {

            Vector2 directionalInput = new Vector2(_currentSpeed, 0);
            _controller.SetDirectionalInput(directionalInput);

            if (_rightDirection)
            {
                _currentSpeed = -_currentSpeed;
                _rightDirection = false;
            }

            if (!_rightDirection)
            {
                _currentSpeed = -_currentSpeed;
                _rightDirection = true;
            }

            Animator.SetFloat("_currentSpeed", Mathf.Abs(_controller.Velocity.x));
            Animator.SetBool("Grounded", _controller.Collisions.Below);

            _renderer.flipX = !_rightDirection;
        }

        private void CheckNoise()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                if (Math.Abs(player.transform.position.x - this.transform.position.x) < _stats.HearRange)
                    if (true) // ToDo Добавить Коэффицент слышимости у игрока
                        State = EnemyStates.Attack;
            }
        }

        
    }
}
