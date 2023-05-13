using UnityEngine;

namespace PolyRocket.Game.Actor
{
    public class PrActor : MonoBehaviour
    {
        private PrLevel _level;
        
        public virtual Vector2 Position => transform.position;

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

        public virtual void Start()
        {
            Level.Actors.Add(this);
        }

        public virtual void OnDestroy()
        {
            Level.Actors.Remove(this);
        }
    }
}