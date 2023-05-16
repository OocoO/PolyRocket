namespace PolyRocket.Game.Actor
{
    public class PrDeadTrap : PrTrap
    {
        public override void OnTriggerWithPlayer(PrPlayer player)
        {
            ShowGameOver();
        }
    }
}