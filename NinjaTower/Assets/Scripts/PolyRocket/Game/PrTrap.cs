﻿using Carotaa.Code;
using PolyRocket.UI;
using UnityEngine;

namespace PolyRocket.Game
{
    // Hit the trap, game over
    public class PrTrap: PrTrigger
    {
        public override void OnTriggerWithPlayer(PrPlayer player)
        {
            ShowGameOver();
        }

        public static void ShowGameOver()
        {
            PrUIPop.Show("Game Over", "Restart", () =>
            {
                var info = PrLevelInfo.Find(1);
                info.JumpToLevel();
                UIManager.Instance.Pop<PrUIPop>();
            });
        }
    }
}