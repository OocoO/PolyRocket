using UnityEngine;

namespace PolyRocket.Game
{
    public abstract class RocketModule
    {
        public enum Name
        {
            None,
            Main,
            Left,
            Right,
            Supper
        }
        
        protected PrPlayer Player;
        protected Rigidbody2D Rb;
        protected PrLevel Level;
        
        
        public Name ModuleName { get; private set; }

        protected RocketModule(Name name, PrPlayer player)
        {
            Player = player;
            Rb = player.m_rb;
            Level = player.Level;
            ModuleName = name;
        }

        public virtual void OnFixedUpdate()
        {
        }
        
        public virtual void OnUpdate(){}
    }

    public class RocketSideModule : RocketModule
    {
        private Vector2 _direct;
        
        public bool IsActive { get; private set; }
        
        public RocketSideModule(Name name, PrPlayer player, float direct) : base(name, player)
        {
            _direct = Quaternion.Euler(Vector3.forward * direct) * Vector2.right;
        }

        public void SetActive(bool active)
        {
            IsActive = active;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            
            if (IsActive)
            {
                Rb.AddForce(_direct * Level.Config.m_SideForce);
            }
        }
    }

    public class RocketMainModule : RocketModule
    {
        public bool IsActive;
        public RocketMainModule(Name name, PrPlayer player) : base(name, player)
        {
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (IsActive)
            {
                Rb.AddForce(Vector2.up * Level.Config.m_MainForce);
            }
        }
        
        public void SetActive(bool active)
        {
            IsActive = active;
        }
    }
}