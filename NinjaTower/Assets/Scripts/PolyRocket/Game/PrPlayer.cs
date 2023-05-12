using System;
using System.Collections.Generic;
using Carotaa.Code;
using MonsterLove.StateMachine;
using PolyRocket.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyRocket.Game
{
    public class PrPlayer : PrActor
    {
        public enum State
        {
            Idle,
            Launch,
        }

        public class EventDriver
        {
            public StateEvent OnUpdate;
            public StateEvent OnFixedUpdate;
        }

        public Rigidbody2D m_rb;
        
        private PrPlayerInput _input;
        private PrPlayerCamera _cameraModule;
        private Camera _levelCamera;
        private List<RocketModule> _rocketModules;

        public RocketSideModule SideLeft;
        public RocketSideModule SideRight;
        public RocketMainModule Main;

        public StateMachine<State, EventDriver> StateMachine;

        public Vector2 Position => m_rb.position;
        public Vector2 Velocity => m_rb.velocity;
        
        private void Start()
        {
            _levelCamera = Level.m_LevelCamera;
            _input = new PrPlayerInput(this);
            _cameraModule = new PrPlayerCamera(this, _levelCamera);
            _rocketModules = new List<RocketModule>();
            
            // init rocket modules
            Main = new RocketMainModule(RocketModule.Name.Main, this);
            SideLeft = new RocketSideModule(RocketModule.Name.Left, this, 45f);
            SideRight = new RocketSideModule(RocketModule.Name.Right, this, 135f);
            
            _rocketModules.Add(Main);
            _rocketModules.Add(SideLeft);
            _rocketModules.Add(SideRight);

            StateMachine = new StateMachine<State, EventDriver>(this);
            StateMachine.ChangeState(State.Idle);

            m_rb.gravityScale = Level.Config.GravityScale;
        }

        private void Update()
        {
            _input.Update();

            foreach (var module in _rocketModules)
            {
                module.OnUpdate();
            }
            
            StateMachine.Driver.OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            StateMachine.Driver.OnFixedUpdate?.Invoke();
            LateFixedUpdate();
        }

        private void OnDestroy()
        {
            _input.OnDestroy();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var control = other.gameObject.GetComponent<PrActor>();
            if (control is PrTrigger trigger)
            {
                trigger.OnTriggerWithPlayer(this);
            }
        }

        // reflection: state machine
        private void Idle_OnUpdate()
        {
            if (SideLeft.IsActive && SideRight.IsActive)
            {
                // launch
                StateMachine.ChangeState(State.Launch);
            }
        }
        private void Launch_Enter()
        {
            m_rb.AddForce(Level.Config.LaunchSpeed * Vector2.up, ForceMode2D.Impulse);
            
            _cameraModule.StartZoomOutAnim();
        }

        private void Launch_OnUpdate()
        {
            _cameraModule.Update();
        }
        
        private void Launch_OnFixedUpdate()
        {
            foreach (var module in _rocketModules)
            {
                module.OnFixedUpdate();
            }
        }

        private void Launch_OnPointerClick(PointerEventData data)
        {
            var clickWorldPos = _levelCamera.ScreenToWorldPoint(data.position);

            var playerPos = transform.position;
            var direct = ((Vector2) (clickWorldPos - playerPos)).normalized;

            m_rb.AddForce(direct * Level.GetClickPower(), ForceMode2D.Impulse);
        }

        private void LateFixedUpdate()
        {
            // velocity dcc
            var velocity = m_rb.velocity;
            velocity.x *= Level.Config.SpeedDcc;
            m_rb.velocity = velocity;

            // Move Player to Another Screen Border When out of screen
            var camBounds = Level.m_LevelCamera.GetViewBound(1.05f);
            var currentPos = m_rb.position;
            if (camBounds.Contains(currentPos))
            {
                return;
            }

            var speedX = m_rb.velocity.x;
            var moveLeft = speedX > 0f;
            var targetX = moveLeft ? camBounds.min.x : camBounds.max.x;
            var targetPos = new Vector2(targetX, currentPos.y);
            m_rb.position = targetPos;
        }
    }
}
