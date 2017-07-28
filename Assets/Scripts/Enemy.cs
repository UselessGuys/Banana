using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
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

        public EnemyStates State;
        //public Animator Animator;

        private CharacterStats _stats;
        private Controller2D _controller;   
        private SpriteRenderer _renderer;

        private bool _rightDirection;
        private float _currentSpeed;
        private Vector2 _rayDirection;
        private double _currentAngle;



        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _stats = GetComponent<CharacterStats>();
            _controller = this.GetComponent<Controller2D>();
        }

        void Start()
        {
            var random = new System.Random();
            _rightDirection = random.Next(1) == 1;
            //ToDo Все мобы пойдут в одну сторону может быть.8

            _currentAngle = 0;

            _currentSpeed = _stats.MoveSpeed;
        }

        void Update()
        {
            Scan();


            if (transform.position.x > RightBorder)
                _rightDirection = false;
            if (transform.position.x < LeftBorder)
                _rightDirection = true;

            CheckNoise();

        }

        private void Scan()
        {

            _currentAngle += 5;
            //ToDo Сделать независимым от фпс
            if (_currentAngle > _stats.VisionAngle / 2)
                _currentAngle -= _stats.VisionAngle;

            _rayDirection = new Vector2(_rightDirection ? (float)Math.Cos(_currentAngle * 3.14 / 180) : -(float)Math.Cos(_currentAngle * 3.14 / 180), (float)Math.Sin(_currentAngle * 3.14 / 180));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, _rayDirection * _stats.VisionRange, (int)_stats.VisionRange, LayerMask.GetMask("Player", "Ground"));
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                _renderer.color = Color.magenta;
            }
            //if (hit.collider.CompareTag("Player")) _renderer.color = Color.magenta;
           
            Debug.DrawRay(transform.position, _rayDirection *_stats.VisionRange, Color.red, .1f);

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
                _renderer.color = Color.red;
            }
        }

        private void Move()
        {
            Vector2 directionalInput;
            if (_rightDirection)
            {
                directionalInput = new Vector2(_currentSpeed, 0);
            }
            else
            {
                directionalInput = new Vector2(-_currentSpeed, 0);
            }
            _controller.SetDirectionalInput(directionalInput);



            //Animator.SetFloat("_currentSpeed", Mathf.Abs(_controller.Velocity.x));
            //Animator.SetBool("Grounded", _controller.Collisions.Below);


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