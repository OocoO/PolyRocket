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
            SideLeft = new RocketSideModule(RocketModule.Name.Left, this, 60f);
            SideRight = new RocketSideModule(RocketModule.Name.Right, this, 120f);
            
            _rocketModules.Add(Main);
            _rocketModules.Add(SideLeft);
            _rocketModules.Add(SideRight);

            StateMachine = new StateMachine<State, EventDriver>(this);
            StateMachine.ChangeState(State.Idle);

            m_rb.gravityScale = Level.Config.GravityScale;
        }

        private void Update()
        {
            foreach (var module in _rocketModules)
            {
                module.OnUpdate();
            }
            
            StateMachine.Driver.OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            StateMachine.Driver.OnFixedUpdate?.Invoke();
            
            m_rb.velocity *= Level.Config.SpeedDcc;
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
        private void Idle_Update()
        {
            if (SideLeft.IsActive && SideRight.IsActive)
            {
                // launch
                StateMachine.ChangeState(State.Launch);
            }
        }
        private void Launch_Enter()
        {
            Main.SetActive(true);
            _cameraModule.StartZoomOutAnim();
        }
        
        private void Launch_OnFixedUpdate()
        {
            foreach (var module in _rocketModules)
            {
                module.OnFixedUpdate();
            }
            
            _cameraModule.FixedUpdate();
        }

        private void Launch_OnPointerClick(PointerEventData data)
        {
            var clickWorldPos = _levelCamera.ScreenToWorldPoint(data.position);

            var playerPos = transform.position;
            var direct = ((Vector2) (clickWorldPos - playerPos)).normalized;

            m_rb.AddForce(direct * Level.GetClickPower(), ForceMode2D.Impulse);
        }
    }
}
