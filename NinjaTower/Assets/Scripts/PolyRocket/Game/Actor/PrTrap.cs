using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine;

namespace PolyRocket.Game.Actor
{
    // Hit the trap, game over
    public abstract class PrTrap: PrTrigger
    {
        public static void OnTrapEnter(PrPlayer player)
        {
            // invincible
            if (player.StateMachine.State == PrPlayer.State.SuperRocket)
            {
                return;
            }
            
            ShowGameOver();
        }

        public static void ShowGameOver()
        {
            UIManager.Instance.Push<GameOverPage>();
        }

        public static void OnTrapEnterWithActor(PrPlayer player, Transform trapTrans)
        {
            var cam = player.Level.m_LevelCamera;
            // addition check: both internal camera view
            var bounds = cam.GetViewBound();
            var isInSide = bounds.Contains(trapTrans.position);
            isInSide &= bounds.Contains(player.Position);
            if (isInSide)
            {
                OnTrapEnter(player);
            }
        }
    }
}