using System;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrActor : MonoBehaviour
    {
        public PrLevel Level => transform.GetComponentInParent<PrLevel>();
    }
}