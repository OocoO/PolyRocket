using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrBallPhysics : MonoBehaviour
    {
        // todo move all physics stuff into this class
        public Rigidbody2D rb;
        
        private PrBall _ball;

        public List<PrForceField> Fields { get; private set; }
        public List<PrCollisionBase> CollisionObjs { get; private set; } 
        private PrGlobal _global;

        public Vector2 Position => rb.position;

        public void Init(PrBall ball, PrGlobal global)
        {
            _ball = ball;
            _global = global;
            Fields = new List<PrForceField>();
            CollisionObjs = new List<PrCollisionBase>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("ForceField"))
            {
                Fields.Add(other.GetComponent<PrForceField>());
            }
            
            _ball.OnPhysicsTrigger(other);
            _ball.StateMachine.Driver.OnPhysicsTriggerEnter.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("ForceField"))
            {
                Fields.Remove(other.GetComponent<PrForceField>());
            }
            
            _ball.StateMachine.Driver.OnPhysicsTriggerExit.Invoke(other);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            CollisionObjs.Add(other.otherCollider.GetComponent<PrCollisionBase>());
            _ball.StateMachine.Driver.OnCollisionEnter.Invoke(other);
        }
        
        private void OnCollisionStay2D(Collision2D other)
        {
            _ball.StateMachine.Driver.OnCollisionStay.Invoke(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            CollisionObjs.Remove(other.otherCollider.GetComponent<PrCollisionBase>());
            _ball.StateMachine.Driver.OnCollisionExit.Invoke(other);
        }

        private void FixedUpdate()
        {
            _ball.StateMachine.Driver.OnFixedUpdate.Invoke();
        }
    }
}
