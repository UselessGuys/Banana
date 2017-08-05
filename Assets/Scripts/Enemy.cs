using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Assets.Scripts;
using UnityEngine;

enum EnemyStates
{
    Stay,
    Patrol,
    Attack
}

internal class Enemy : MonoBehaviour
{
    private static System.Random Random = new System.Random();

    private const float HorizontalRaySpacing = 0.2f;
    private const float RayLength = 1f;
   

    public int LeftBorder;
    public int RightBorder;
    public EnemyStates State;

    private CharacterStats _stats;
    private DynamicObject _controller;
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
        _controller = GetComponent<DynamicObject>();
    }

    void Start()
    {
        
        _rightDirection = Random.Next(0, 1) == 1;

        _currentAngle = 0;
        _currentSpeed = _stats.MoveSpeed;


        rayCount = (float)Math.Round(_controller.ObjectHeight / HorizontalRaySpacing);
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
        _currentAngle += 360 * Time.deltaTime;

        if (_currentAngle > _stats.VisionAngle / 2)
            _currentAngle -= _stats.VisionAngle;

        _rayDirection = new Vector2(
            _rightDirection ? (float)Math.Cos(_currentAngle * Math.PI / 180) : -(float)Math.Cos(_currentAngle * Math.PI / 180),
            (float)Math.Sin(_currentAngle * Math.PI / 180));

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _rayDirection * _stats.VisionRange, (int)_stats.VisionRange);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            _target = hit.collider.gameObject;
            State = EnemyStates.Attack;
        }

        Debug.DrawRay(transform.position, _rayDirection * _stats.VisionRange, Color.red, .1f);

    }

    private void CheckNoise()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (Math.Abs(player.transform.position.x - this.transform.position.x) 
                - player.GetComponent<DynamicObject>().ObjectWeght / 2 
                - this._controller.ObjectWeght / 2 <
                _stats.HearRange * player.GetComponent<CharacterStats>().Noise) 
            {
                State = EnemyStates.Attack;
                _target = player;
            }
            Debug.Log(Math.Abs(player.transform.position.x - this.transform.position.x)
                - player.GetComponent<DynamicObject>().ObjectWeght / 2
                - this._controller.ObjectWeght / 2);
        }
    }

    private void Move(float speed)
    {
        if (Math.Abs(speed) > 0) //ToDo не должен прыгать при достижении игрока
            CheckObstacles();

        if (_rightDirection)
            _controller.Velocity.x = speed;
        else
            _controller.Velocity.x = -speed;



        _renderer.flipX = !_rightDirection;
    }

    private void CheckObstacles()
    {
        for (int i = 0; i < rayCount + 1; i++)
        {
            Vector2 rayOrigin = (!_rightDirection)
                ? _controller.RaycastOrigin.BottomLeft
                : _controller.RaycastOrigin.BottomRight;
            rayOrigin += Vector2.up * (realRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * (_rightDirection ? 1 : -1), RayLength);

            Debug.DrawRay(rayOrigin, Vector2.right * (_rightDirection ? 1 : -1), Color.blue);

            if (hit.collider != null && hit.collider.CompareTag("Ground")
                && hit.collider.gameObject.transform.lossyScale.y > DynamicObject.MaxHorisontalLadder)            
            {
                _controller.Jump(_stats.JumpHeight);
            }

        }
    }
}