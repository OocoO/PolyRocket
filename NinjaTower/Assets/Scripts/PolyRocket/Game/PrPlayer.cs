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

        private PrRocketGenerator _generator;
        private PrPlayerInput _input;
        private PrPlayerCamera _camera;

        private void Awake()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/RoundRocket");
            _generator = new PrRocketGenerator(prefab, null, 1f);
            _input = new PrPlayerInput(this);

            PrCameraManager.Instance.FollowTarget = transform;
        }

        public void MoveLeft()
        {
            var acc = -1f * m_acc.x;
            m_rb.AddForce(new Vector2(acc, 0f));
        }

        public void MoveRight()
        {
            var acc = m_acc.x;
            m_rb.AddForce(new Vector2(acc, 0f));
        }

        private void Update()
        {
            _generator.Update();
        }

        private void FixedUpdate()
        {
            m_rb.AddForce(new Vector2(0f, m_acc.y));
        }

        // private void LateUpdate()
        // {
        //     // clamp transform position within camera view
        //     var mainCam = PrCameraManager.Instance.m_MainCam;
        //     var rect = mainCam.GetViewBox();
        //     var pos = transform.position;
        //     pos.x = Mathf.Clamp(pos.x, rect.xMin, rect.xMax);
        //     transform.position = pos;
        // }

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
    }
}
