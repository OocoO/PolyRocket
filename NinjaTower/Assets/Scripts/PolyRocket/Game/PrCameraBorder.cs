using Carotaa.Code;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrCameraBorder: PrTrigger
    {
        public override void OnTriggerWithPlayer(PrPlayer player)
        {
            // Move Player to Another Screen Border When out of screen
            var speedX = player.m_rb.velocity.x;
            var moveLeft = speedX > 0f;
            var camBounds = Level.m_LevelCamera.GetViewBound(1.1f);
            var targetX = moveLeft ? camBounds.min.x : camBounds.max.x;
            var targetPos = new Vector2(targetX, player.m_rb.position.y);
            player.m_rb.position = targetPos;
        }
    }
}