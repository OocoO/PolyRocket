using Carotaa.Code;
using PolyRocket.UI;

namespace PolyRocket.Game.Actor
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
            UIManager.Instance.Push<GameOverPage>();
        }
    }
}