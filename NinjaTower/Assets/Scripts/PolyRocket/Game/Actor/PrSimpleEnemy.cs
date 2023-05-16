namespace PolyRocket.Game.Actor
{
    public class PrSimpleEnemy : PrTrap
    {
        public override void OnTriggerWithPlayer(PrPlayer player)
        {
            OnTrapEnterWithActor(player, transform);
        }
    }
}