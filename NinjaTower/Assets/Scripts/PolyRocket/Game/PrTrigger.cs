using UnityEngine;

namespace PolyRocket.Game
{
    // base class for all env triggers
    public abstract class PrTrigger: PrActor
    {
        public abstract void OnTriggerWithPlayer(PrPlayer player);
    }
}