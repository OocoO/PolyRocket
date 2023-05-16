using System;
using PolyRocket.Game;
using UnityEngine;
using Random = System.Random;

namespace PolyRocket.SO
{
    public abstract class ElementGenerator : ScriptableObject
    {
        protected PrLevel Level;
        
        public virtual void Init(PrLevel level, Random random)
        {
            Level = level;
        }
        
        public virtual void Update(){}
    }
}