namespace PolyRocket.Game.Actor
{
    public class PrBerry : PrTrigger
    {
        public override void OnTriggerWithPlayer(PrPlayer player)
        {
            Level.BerryCount.Value++;
            
            // add some effect
            Destroy(gameObject);
        }
    }
}