using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrBallPhysics : MonoBehaviour
    {
        // todo move all physics stuff into this class
        public CircleCollider2D physicsCollider;

        private PrBall _ball;

        public void Init(PrBall ball)
        {
            _ball = ball;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            _ball.OnPhysicsTrigger(other);
        }
    }
}
