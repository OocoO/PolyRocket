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

        public List<PrForceField> Fields { get; set; }
        private PrGlobal _global;

        public Vector2 Position => rb.position;

        public void Init(PrBall ball, PrGlobal global)
        {
            _ball = ball;
            _global = global;
            Fields = new List<PrForceField>();
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

        private void FixedUpdate()
        {
            _ball.StateMachine.Driver.OnFixedUpdate.Invoke();
        }
    }
}
