using System;
using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;
using PathCreation;
using UnityEngine;

namespace PolyRocket.Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PrTrapDynamic : PrCollision
    {
        public PathCreator creator;

        public float speed;

        private Rigidbody2D _rb;
        private float _distance;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            _rb.bodyType = RigidbodyType2D.Kinematic;
            _distance = 0f;
        }

        private void FixedUpdate()
        {
            _distance += speed * Time.fixedDeltaTime;

            var pos = (Vector2) creator.path.GetPointAtDistance(_distance, EndOfPathInstruction.Reverse);
            _rb.MovePosition(pos);
        }
    }
}