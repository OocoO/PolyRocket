using System;
using System.Collections;
using System.Collections.Generic;
using PolyRocket.Game.Actor;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLevelCamera : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            var actor = other.GetComponent<PrActor>();
            if (actor && actor is not PrPlayer)
            {
                // TODO: recycle
                Destroy(actor.gameObject);
            }
        }
    }
}
