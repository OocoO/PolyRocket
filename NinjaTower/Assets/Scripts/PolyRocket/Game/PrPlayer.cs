using System;
using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrPlayer : PrActor
    {
        public Rigidbody2D m_rb;

        public Vector2 m_acc;

        private void FixedUpdate()
        {
            m_rb.AddForce(m_acc);
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
    }
}
