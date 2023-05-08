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

        public Vector2 m_acc;

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
        }

        private void Update()
        {
            _cameraModule.Update();

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
                Debug.Log("Collision");
                PrUIPop.Show("Game Over", "Restart", () =>
                {
                    var info = PrLevelInfo.Find(1);
                    info.JumpToLevel();
                });
            }
        }

        // reflection: state machine
        private void Idle_OnPointerClick(PointerEventData data)
        {
            StateMachine.ChangeState(State.Launch);
        }
        
        private void Launch_Enter()
        {
            _cameraModule.StartZoomOutAnim();
        }
        
        
        private void Launch_OnFixedUpdate()
        {
            m_rb.AddForce(new Vector2(0f, m_acc.y));
        }

        private void Launch_OnPointerClick(PointerEventData data)
        {
            var clickWorldPos = _levelCamera.ScreenToWorldPoint(data.position);

            var playerX = transform.position.x;
            var isLeft = playerX > clickWorldPos.x;
            MoveLeftRight(isLeft);
        }

        private void MoveLeftRight(bool isLeft)
        {
            var acc = m_acc.x;
            if (isLeft)
            {
                acc *= -1f;
            }
            m_rb.AddForce(new Vector2(acc, 0f), ForceMode2D.Impulse);
        }
    }
}
