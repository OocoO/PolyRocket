namespace PolyRocket.Game.Actor
{
    public class PrWaterMelon: PrTrigger
    {
        public override void OnTriggerWithPlayer(PrPlayer player)
        {
            Level.MelonCount.Value++;
            
            Destroy(gameObject);
            player.StateMachine.Driver.OnTriggerWithWaterMelon.Invoke();
        }
    }
}