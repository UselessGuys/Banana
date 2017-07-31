using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Physics;
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
        public float HorizontalRaySpacing = 0.5f;
        private const float RayLength = 1f;

        public int LeftBorder;
        public int RightBorder;

        public EnemyStates State;

        private CharacterStats _stats;
        private Controller2D _controller;   
        private SpriteRenderer _renderer;

        private bool _rightDirection;
        private float _currentSpeed;
        private Vector2 _rayDirection;
        private double _currentAngle;
        private GameObject _target;

        private float rayCount;
        private float realRaySpacing;

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
            //ToDo Все мобы пойдут в одну сторону может быть.

            _currentAngle = 0;
            _currentSpeed = _stats.MoveSpeed;


            rayCount = (float) Math.Round(_controller.ObjectHeight / HorizontalRaySpacing);
            realRaySpacing = _controller.ObjectHeight / rayCount;
        }

        void Update()
        {
            Scan();
            CheckNoise();
        }

        void FixedUpdate()
        {
            if (State == EnemyStates.Stay)
                Move(0);

            if (State == EnemyStates.Patrol)
            {
                if (transform.position.x > RightBorder)
                    _rightDirection = false;
                if (transform.position.x < LeftBorder)
                    _rightDirection = true;

                Move(_currentSpeed);
            }
                
            if (State == EnemyStates.Attack)
            {
                _rightDirection = _target.transform.position.x - this.transform.position.x > 0;
                Move(_currentSpeed);
                _renderer.color = Color.red;
            }
        }
        
        private void Scan()
        {
            _currentAngle += 5; //ToDo Сделать независимым от фпс

            if (_currentAngle > _stats.VisionAngle / 2)
                _currentAngle -= _stats.VisionAngle;

            _rayDirection = new Vector2(
                _rightDirection ? (float)Math.Cos(_currentAngle * Math.PI / 180) : -(float)Math.Cos(_currentAngle * Math.PI / 180), 
                (float)Math.Sin(_currentAngle * Math.PI / 180));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, _rayDirection * _stats.VisionRange, (int)_stats.VisionRange);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                State = EnemyStates.Attack;
                _target = hit.collider.gameObject;
            }
           
            Debug.DrawRay(transform.position, _rayDirection *_stats.VisionRange, Color.red, .1f);

        }

        private void CheckNoise()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                if (Math.Abs(player.transform.position.x - this.transform.position.x) <
                    _stats.HearRange * player.GetComponent<CharacterStats>().Noise) //ToDo растояние не из центра персонажа
                {
                    State = EnemyStates.Attack;
                    _target = player;
                }
            }
        }

        private void Move(float speed)
        {
            if (Math.Abs(speed) > 0)
                CheckObstacles();
            else _controller.OnJumpInputUp();

            Vector2 directionalInput;
            if (_rightDirection)
                directionalInput = new Vector2(speed, 0);
            else
                directionalInput = new Vector2(-speed, 0);

            _controller.SetDirectionalInput(directionalInput);

            _renderer.flipX = !_rightDirection;
        }

        private void CheckObstacles()
        {
            for (int i = 0; i < rayCount + 1; i++)
            {
                Vector2 rayOrigin = (!_rightDirection)
                    ? _controller.raycastOrigins.BottomLeft
                    : _controller.raycastOrigins.BottomRight;
                rayOrigin += Vector2.up * (realRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * (_rightDirection ? 1 : -1), RayLength);

                Debug.DrawRay(rayOrigin, Vector2.right * (_rightDirection ? 1 : -1), Color.blue);

                if (hit.collider != null && hit.collider.CompareTag("Ground"))
                {
                    _controller.OnJumpInputDown(); //ToDo Сила прыжка должна зависеть от статов
                }
            }
        }
    }
}