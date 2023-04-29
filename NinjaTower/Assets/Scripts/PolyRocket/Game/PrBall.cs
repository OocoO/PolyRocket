using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;
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
    
    public class PrBall : PrActorBase
    {
        public class EventDiver
        {
            public StateEvent<PointerEventData> OnPointerClick;
            public StateEvent<Collider2D> OnPhysicsTriggerEnter;
            public StateEvent<Collider2D> OnPhysicsTriggerExit;
            public StateEvent<Collision2D> OnCollisionEnter;
            public StateEvent<Collision2D> OnCollisionStay;
            public StateEvent<Collision2D> OnCollisionExit;

            public StateEvent OnFixedUpdate;
        }
        public CircleCollider2D col;
        public PrBallPhysics ballPhysics;
        
        private PrGlobal _global;
        public StateMachine<PrBallState, EventDiver> StateMachine;

        private LinkedList<DashInfo> _dashQueue;
        private bool _isChainDash;

        public void Init(PrGlobal global)
        {
            _global = global;
            _dashQueue = new LinkedList<DashInfo>();
            
            ballPhysics.Init(this, global);

            StateMachine = new StateMachine<PrBallState, EventDiver>(this);
            StateMachine.ChangeState(PrBallState.Idle);
            
            // init data
            _global.VPlayerDashLevel.Value = 0;
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
            var mag = direct.magnitude - _global.BallScreenRadius;
            if (mag < 0f)
            {
                return scale;
            }

            return _global.BallScreenRadius / direct.magnitude;
        }

        private void ResetSpeed()
        {
            ballPhysics.rb.velocity = Vector2.zero;
        }

        // global: do this check in all state
        public void OnPhysicsTrigger(Collider2D other)
        {
            var actor = other.gameObject.GetComponent<PrActorBase>();
            if (actor is PrFlag)
            {
                // mark game over
                _global.EPlayerMoveTriggerFlag?.Invoke(actor);
            }
            else if (IsTrapTag(actor))
            {
                _global.EPlayerTriggerTrap?.Invoke();
            }
        }

        private static bool IsTrapTag(PrActorBase actor)
        {
            return actor is PrTrapDynamic || actor is PrTrapStatic;
        }
        
        
        // reflection: State Machine
        public void Idle_OnPointerClick(PointerEventData eventData)
        {
            // EventTrack.Log("Idle Pointer Click");
            
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
            // EventTrack.Log("DashStart_Enter");
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
            // EventTrack.Log("Dash_Enter");
            _global.VPlayerDashLevel.Value++;
            
            var info = _dashQueue.First.Value;
            info.Progress = 0.1f;
            var complete = false;
            Dash_OnFixedUpdate();
            var tweener = DOTween.To(() => info.Progress, x => { info.Progress = x; }, 1f, 0.2f);
            tweener.SetEase(Ease.Linear);
            tweener.OnComplete(() =>
            {
                // EventTrack.Log("Dash Complete Tween");
                Dash_OnFixedUpdate();
                complete = true;
            });

            yield return new WaitUntil(() => complete);
            // EventTrack.Log("Dash_Enter_Complete");

            // force refresh
            var next = info.Reason == DashInfo.EndReason.Default ? PrBallState.DashChain : PrBallState.DashEnd;
            StateMachine.ChangeState(next);
        }

        public void Dash_OnFixedUpdate()
        {
            var infoNode = _dashQueue.First;
            var info = infoNode.Value;

            var pos = info.GetCurrentPos();
            ballPhysics.rb.velocity = Vector2.zero;
            ballPhysics.rb.MovePosition(pos);
        }

        public void Dash_OnCollisionEnter(Collision2D other)
        {
            if (other.otherCollider.gameObject.CompareTag("StaticWall"))
            {
                var info = _dashQueue.First.Value;
                info.Reason = DashInfo.EndReason.Collision;
            }
        }

        public void DashChain_Enter()
        {
            _isChainDash = false;
            
            CancelInvoke(nameof(StopChainDash));
            Invoke(nameof(StopChainDash), 0.2f);
        }

        private void StopChainDash()
        {
            if (_isChainDash) return;
            
            // EventTrack.Log("Dash Chain End");
            StateMachine.ChangeState(PrBallState.DashEnd);
        }

        public void DashChain_OnPointerClick(PointerEventData eventData)
        {
            // maybe condition check
            // EventTrack.Log("Dash Chain Pointer Click");s

            _isChainDash = true;
            DoDash(eventData, ballPhysics.Position);
        }

        public IEnumerator DashEnd_Enter()
        {
            // do something anime
            ballPhysics.rb.velocity = Vector2.zero;
            _dashQueue.Clear();
            
            // reset dash level
            _global.VPlayerDashLevel.Value = 0;
            
            yield return new WaitForSeconds(0.3f);

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
                
            StateMachine.ChangeState(PrBallState.DashStart, StateTransition.Safe);
        }
    }
}