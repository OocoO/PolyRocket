using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;
using MonsterLove.StateMachine;
using UnityEngine;

namespace PolyRocket.Game.Actor
{
    public class PrPlayer : PrActor
    {
        public enum State
        {
            Idle,
            Launch,
            SuperRocket,
        }

        public class EventDriver
        {
            public StateEvent OnUpdate;
            public StateEvent OnFixedUpdate;
            public StateEvent OnTriggerWithWaterMelon;
        }

        public Rigidbody2D m_rb;
        public SpriteRenderer m_Sr;
        
        private PrPlayerInput _input;
        private PrPlayerCamera _cameraModule;
        private GeneratorManager _generator;
        private Camera _levelCamera;
        private List<RocketModule> _rocketModules;
        private float _initHeight;

        public RocketSideModule SideLeft;
        public RocketSideModule SideRight;
        public RocketMainModule Main;

        public StateMachine<State, EventDriver> StateMachine;

        public override Vector2 Position => m_rb.position;
        public Vector2 Velocity => m_rb.velocity;
        
        public override void Start()
        {
            base.Start();
            
            _levelCamera = Level.m_LevelCamera;
            _input = new PrPlayerInput(this);
            _cameraModule = new PrPlayerCamera(this, _levelCamera);
            _generator = new GeneratorManager(this);
            _rocketModules = new List<RocketModule>();

            var config = Level.Config;
            // init rocket modules
            Main = new RocketMainModule(RocketModule.Name.Main, this);
            SideLeft = new RocketSideModule(RocketModule.Name.Left, this, config.m_LeftForceDirect);
            SideRight = new RocketSideModule(RocketModule.Name.Right, this, 180f - config.m_LeftForceDirect);
            
            _rocketModules.Add(Main);
            _rocketModules.Add(SideLeft);
            _rocketModules.Add(SideRight);

            StateMachine = new StateMachine<State, EventDriver>(this);
            StateMachine.ChangeState(State.Idle);

            m_rb.gravityScale = Level.Config.GravityScale;

            StatisticInit();
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

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            _input.OnDestroy();
            _generator.OnDestroy();
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
            if (Main.IsActive)
            {
                // launch
                StateMachine.ChangeState(State.Launch);
            }
        }
        private void Launch_Enter()
        {
            m_rb.AddForce(Level.Config.LaunchSpeed * Vector2.up, ForceMode2D.Impulse);
        }

        private void Launch_OnUpdate()
        {
            OnFlyUpdate();
        }
        
        private void Launch_OnFixedUpdate()
        {
            OnFlyFixedUpdate();
        }

        private void Launch_OnTriggerWithWaterMelon()
        {
            // enter super jump
            StateMachine.ChangeState(State.SuperRocket);
        }

        private void SuperRocket_Enter()
        {
            MonoHelper.Instance.DispatchAfterSeconds(() =>
            {
                StateMachine.ChangeState(State.Launch);
            }, 3f);
        }
        
        private void SuperRocket_OnUpdate()
        {
            OnFlyUpdate();
        }

        private void SuperRocket_OnFixedUpdate()
        {
            m_rb.AddForce(Level.Config.m_SuperForce * Vector2.up, ForceMode2D.Force);

            OnFlyFixedUpdate();
        }

        private void OnFlyUpdate()
        {
            _cameraModule.Update();
            _generator.Update();
            StatisticUpdate();
        }

        private void OnFlyFixedUpdate()
        {
            foreach (var module in _rocketModules)
            {
                module.OnFixedUpdate();
            }
            _cameraModule.FixedUpdate();
        }

        private void LateFixedUpdate()
        {
            // velocity dcc
            var velocity = m_rb.velocity;
            velocity *= Level.Config.SpeedDcc;
            m_rb.velocity = velocity;
            EventTrack.LogParam("PlayerVelocity.y", velocity.y);

            // Move Player to Another Screen Border When out of screen
            var camBounds = Level.m_LevelCamera.GetViewBound(1.05f);
            var currentPos = m_rb.position;
            if (currentPos.x > camBounds.min.x && currentPos.x < camBounds.max.x)
            {
                return;
            }

            var speedX = m_rb.velocity.x;
            var moveLeft = speedX > 0f;
            var targetX = moveLeft ? camBounds.min.x : camBounds.max.x;
            var targetPos = new Vector2(targetX, currentPos.y);
            m_rb.position = targetPos;
        }
        
        private void StatisticInit()
        {
            _initHeight = m_rb.position.y;
        }

        private void StatisticUpdate()
        {
            var height = Level.Height.Value;
            var currentHeight = m_rb.position.y - _initHeight;
            if (height < currentHeight)
            {
                Level.Height.Value = currentHeight;
            }
            
            Level.LaunchTime.Value += Time.deltaTime;
        }
    }
}
