using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PolyRocket.Game
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