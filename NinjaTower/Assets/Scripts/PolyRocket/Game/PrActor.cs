using System;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrActor : MonoBehaviour
    {
        private PrLevel _level;

        public PrLevel Level
        {
            get
            {
                // do not use mono-related properties
                if (object.ReferenceEquals(_level, null)) 
                    _level = GetComponentInParent<PrLevel>();

                return _level;
            }
        }
    }
}