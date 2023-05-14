using System;
using System.Collections;
using System.Collections.Generic;
using PolyRocket.Game.Actor;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLevelCamera : MonoBehaviour
    {
        public Transform m_SkyTran;

        public void Start()
        {
            var level = GetComponentInParent<PrLevel>();
            var aspect = level.m_LevelCamera.aspect;
            var maxHeight = PrLevel.MaxCameraSize * 2f;
            var maxWidth = maxHeight * aspect;
            m_SkyTran.localScale = new Vector3(maxWidth, maxHeight, 1f);
        }
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
