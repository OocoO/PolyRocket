using System;
using PolyRocket.Game;
using UnityEngine;

namespace PolyRocket.SO
{
    public abstract class ElementGenerator : ScriptableObject
    {
        protected PrLevel Level;
        
        public virtual void Init(PrLevel level)
        {
            Level = level;
        }
        
        public virtual void Update(){}
    }
}