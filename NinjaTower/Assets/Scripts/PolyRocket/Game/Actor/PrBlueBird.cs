using Carotaa.Code;
using PathCreation;
using UnityEngine;

namespace PolyRocket.Game.Actor
{
    public class PrBlueBird : PrTrap
    {
        public PathCreator m_Creator;
        public SpriteRenderer m_SpriteRenderer;

        public float speed;

        private float _distance;
        private Vector3 _posLast;

        private void Awake()
        {
            _distance = 0f;
            m_Creator.transform.SetParent(Level.transform);
        }

        private void Update()
        {
            _distance += speed * Time.deltaTime;
            var trans = transform;
            _posLast = trans.position;

            var pos = m_Creator.path.GetPointAtDistance(_distance, EndOfPathInstruction.Reverse);
            trans.position = pos;
            
            var dir = pos - _posLast;
            var isFlip = dir.x > 0f;
            m_SpriteRenderer.flipX = isFlip;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            if (m_Creator) Destroy(m_Creator.gameObject);
        }

        public override void OnTriggerWithPlayer(PrPlayer player)
        {
            var cam = player.Level.m_LevelCamera;
            // addition check: both internal camera view
            var bounds = cam.GetViewBound();
            var isInSide = bounds.Contains(transform.position);
            isInSide &= bounds.Contains(player.Position);
            if (isInSide)
            {
                ShowGameOver();
            }
        }
    }
}