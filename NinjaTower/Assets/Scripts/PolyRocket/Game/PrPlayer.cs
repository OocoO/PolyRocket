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
            
            public StateEvent<PointerEventData> OnPointerClick;
        }

        public Rigidbody2D m_rb;
        
        private PrPlayerInput _input;
        private PrPlayerCamera _cameraModule;
        private Camera _levelCamera;
        public StateMachine<State, EventDriver> StateMachine;

        public Vector2 Position => m_rb.position;
        public Vector2 Velocity => m_rb.velocity;
        
        private void Start()
        {
            _levelCamera = Level.m_LevelCamera;
            _input = new PrPlayerInput(this);
            _cameraModule = new PrPlayerCamera(this, _levelCamera);

            StateMachine = new StateMachine<State, EventDriver>(this);
            StateMachine.ChangeState(State.Idle);

            m_rb.gravityScale = Level.Config.PlayerGravityScale;
        }

        private void Update()
        {
            StateMachine.Driver.OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            StateMachine.Driver.OnFixedUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            _input.OnDestroy();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var control = other.gameObject.GetComponent<PrActor>();
            if (control is PrTrap)
            {
                PrUIPop.Show("Game Over", "Restart", () =>
                {
                    var info = PrLevelInfo.Find(1);
                    info.JumpToLevel();
                    UIManager.Instance.Pop<PrUIPop>();
                });
            }
            else if (control is PrCameraBorder)
            {
                // Move Player to Another Screen Border When out of screen
                var speedX = m_rb.velocity.x;
                var moveLeft = speedX > 0f;
                var camBounds = Level.m_LevelCamera.GetViewBound(1.1f);
                var targetX = moveLeft ? camBounds.min.x : camBounds.max.x;
                var targetPos = new Vector2(targetX, m_rb.position.y);
                m_rb.position = targetPos;
            }
        }

        // reflection: state machine
        private void Idle_OnPointerClick(PointerEventData data)
        {
            StateMachine.ChangeState(State.Launch);
        }
        
        private void Launch_Enter()
        {
            m_rb.AddForce(Vector2.up * Level.Config.PlayerLaunchSpeed, ForceMode2D.Impulse);
            
            _cameraModule.StartZoomOutAnim();
        }
        
        
        private void Launch_OnFixedUpdate()
        {
            _cameraModule.FixedUpdate();
            
            m_rb.velocity *= Level.Config.PlayerSpeedDcc;
        }

        private void Launch_OnPointerClick(PointerEventData data)
        {
            var clickWorldPos = _levelCamera.ScreenToWorldPoint(data.position);

            var playerPos = transform.position;
            var direct = ((Vector2) (clickWorldPos - playerPos)).normalized;

            m_rb.AddForce(direct * Level.Config.PlayerClickPower, ForceMode2D.Impulse);
        }
    }
}
