using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyRocket.Game
{
    public enum PrBallState
    {
        Idle,
        DashStart,
        Dash,
        DashChain,
        DashEnd,
    }
    
    public class DashInfo
    {
        public enum EndReason
        {
            Default,
            Collision,
        }
        
        // init
        public Vector2 StartPos;
        public Vector2 ClickPos;
        public PrBallState Source;

        // dynamic
        public float Progress;
        public Vector2 EndPos;
        public EndReason Reason;
        

        public DashInfo()
        {
            Progress = 0f;
            Reason = EndReason.Default;
        }

        public Vector2 GetCurrentPos()
        {
            return StartPos + (EndPos - StartPos) * Progress;
        }
    }
    
    public class PrBall : MonoBehaviour
    {
        public class EventDiver
        {
            public StateEvent<PointerEventData> OnPointerClick;
            public StateEvent<Collider2D> OnPhysicsTriggerEnter;
            public StateEvent<Collider2D> OnPhysicsTriggerExit;
            public StateEvent OnFixedUpdate;
        }
        public CircleCollider2D col;
        public PrBallPhysics ballPhysics;
        
        private PrGlobal _global;
        public StateMachine<PrBallState, EventDiver> StateMachine;

        private LinkedList<DashInfo> _dashQueue;

        private bool _isSuccess;
        public void Init(PrGlobal global)
        {
            _global = global;
            _dashQueue = new LinkedList<DashInfo>();
            
            ballPhysics.Init(this, global);

            StateMachine = new StateMachine<PrBallState, EventDiver>(this);
            StateMachine.ChangeState(PrBallState.Idle);
        }

        private void Update()
        {
            CheckIsSuccess();
        }

        private void CheckIsSuccess()
        {
            if (_isSuccess)
            {
                _global.EPlayerMoveToTarget?.Invoke();
                // stop move
                StopMove();
            }
        }

        private void StopMove()
        {
            ResetSpeed();
            // trigger event
            _isSuccess = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StateMachine.Driver.OnPointerClick.Invoke(eventData);
        }

        private Vector2 GetAimDirect(PointerEventData eventData)
        {
            var ballPos = ballPhysics.Position;
            var pos = _global.Screen2WorldPosition(eventData.position);
            var direct = ballPos - pos;
            return direct;
        }

        private float GetForceScale(PointerEventData eventData)
        {
            var direct = GetAimDirect(eventData);
            var scale = 1f;
            var mag = direct.magnitude - _global.ballScreenRadius;
            if (mag < 0f)
            {
                return scale;
            }

            return _global.ballScreenRadius / direct.magnitude;
        }

        private void ResetSpeed()
        {
            ballPhysics.rb.velocity = Vector2.zero;
        }

        // global: do this check in all state
        public void OnPhysicsTrigger(Collider2D other)
        {
            var otherGo = other.gameObject;
            if (otherGo.CompareTag("Flag"))
            {
                // mark game over
                _isSuccess = true;
            }
            else if (IsTrapTag(otherGo))
            {
                _global.EPlayerTriggerTrap?.Invoke();
            }
        }

        private static bool IsTrapTag(GameObject go)
        {
            return go.CompareTag("StaticTrap") || go.CompareTag("DynamicTrap");
        }
        
        
        // reflection: State Machine
        public void Idle_OnPointerClick(PointerEventData eventData)
        {
            var startPos = ballPhysics.Position;
            DoDash(eventData, startPos);
        }

        public void Idle_OnFixedUpdate()
        {
            foreach (var field in ballPhysics.Fields)
            {
                field.ApplyForce(ballPhysics.rb);
            }
        }
        
        public void DashStart_Enter()
        {
            var infoNode = _dashQueue.First;
            var info = infoNode.Value;
            var direct = info.ClickPos - info.StartPos;
            // var mag = Mathf.Min(direct.magnitude, _global.PlayerDashDistance);
            var mag = _global.PlayerDashDistance;
            info.EndPos = direct.normalized * mag + info.StartPos;

            StateMachine.ChangeState(PrBallState.Dash);
        }

        public IEnumerator Dash_Enter()
        {
            // do something animation stuff
            var info = _dashQueue.First.Value;
            info.Progress = 0.1f;
            var complete = false;
            Dash_OnFixedUpdate();
            var tweener = DOTween.To(() => info.Progress, x => { info.Progress = x; }, 1f, 0.2f);
            tweener.SetEase(Ease.Linear);
            tweener.OnComplete(() =>
            {
                Dash_OnFixedUpdate();
                complete = true;
            });

            yield return new WaitUntil(() => complete);
            
            // force refresh
            var next = info.Reason == DashInfo.EndReason.Default ? PrBallState.DashChain : PrBallState.DashEnd;
            StateMachine.ChangeState(next);
        }

        public void Dash_OnFixedUpdate()
        {
            var infoNode = _dashQueue.First;
            var info = infoNode.Value;

            var pos = info.GetCurrentPos();
            ballPhysics.rb.MovePosition(pos);
        }

        public void Dash_OnCollisionEnter(Collider2D other)
        {
            if (other.gameObject.CompareTag("StaticWall"))
            {
                var info = _dashQueue.First.Value;
                info.Reason = DashInfo.EndReason.Collision;
            }
        }

        public IEnumerator DashChain_Enter()
        {
            yield return new WaitForSeconds(0.2f);
            
            StateMachine.ChangeState(PrBallState.DashEnd);
        }

        public void DashChain_OnPointerClick(PointerEventData eventData)
        {
            // maybe condition check
            var current = _dashQueue.First.Value;
            DoDash(eventData, current.EndPos);
        }

        public void DashEnd_Enter()
        {
            // do something anime
            _dashQueue.Clear();

            StateMachine.ChangeState(PrBallState.Idle);
        }

        private void DoDash(PointerEventData eventData, Vector2 startPos)
        {
            var clickPos = _global.Screen2WorldPosition(eventData.position);

            var dashInfo = new DashInfo()
            {
                StartPos = startPos,
                ClickPos = clickPos,
                Source = StateMachine.State,
            };
            _dashQueue.AddFirst(dashInfo);
                
            StateMachine.ChangeState(PrBallState.DashStart, StateTransition.Overwrite);
        }
    }
}