using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrForceField : PrTriggerBase
    {
        // public ParticleSystem.MinMaxCurve forceCurve;
        public enum ForceType
        {
            Spot,
            Rect,
        }

        public ForceType type;
        public Vector2 rectForce;
        public float spotForce;
        
        public void ApplyForce(Rigidbody2D rb)
        {
            switch (type)
            {
                case ForceType.Spot:
                    ApplySpotForce(rb);
                    break;
                case ForceType.Rect:
                    ApplyRectForce(rb);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ApplySpotForce(Rigidbody2D rb)
        {
            var direct = (Vector2) transform.position - rb.position;
            rb.AddForce(direct * spotForce, ForceMode2D.Force);
        }

        private void ApplyRectForce(Rigidbody2D rb)
        {
            rb.AddForce(rectForce, ForceMode2D.Force);
        }
    }
}