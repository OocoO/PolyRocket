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
        private PrGameLauncher _launcher;

        private List<PrForceField> _fields;

        public Vector2 Position => rb.position;

        public void Init(PrBall ball, PrGameLauncher launcher)
        {
            _ball = ball;
            _launcher = launcher;
            _fields = new List<PrForceField>();
            
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("ForceField"))
            {
                _fields.Add(other.GetComponent<PrForceField>());
            }
            _ball.OnPhysicsTrigger(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("ForceField"))
            {
                _fields.Remove(other.GetComponent<PrForceField>());
            }
        }

        private void FixedUpdate()
        {
            var velocity = rb.velocity;
            rb.AddForce(-velocity * _launcher.speedDecrease, ForceMode2D.Force);

            foreach (var field in _fields)
            {
                field.ApplyForce(rb);
            }
        }
    }
}
