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

        private PrRocketGenerator _generator;
        private PrPlayerInput _input;
        private PrPlayerCamera _cameraModule;
        private Camera _mainCam;
        public StateMachine<State, EventDriver> StateMachine;

        public Vector2 Position => m_rb.position;
        public Vector2 Velocity => m_rb.velocity;
        
        private void Awake()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/RoundRocket");
            var mainCam = PrCameraManager.Instance.m_MainCam;
            
            _mainCam = mainCam;
            _generator = new PrRocketGenerator(prefab, null, mainCam, 1f);
            _input = new PrPlayerInput(this);
            _cameraModule = new PrPlayerCamera(this, mainCam);

            StateMachine = new StateMachine<State, EventDriver>(this);
            StateMachine.ChangeState(State.Idle);
        }

        private void Update()
        {
            _generator.Update();
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
            if (control is PrCollision)
            {
                Debug.Log("Collision");
                PrUIPop.Show("Game Over", "Restart", () =>
                {
                    Debug.Log("DoSomething");
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
            var clickWorldPos = _mainCam.ScreenToWorldPoint(data.position);

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
