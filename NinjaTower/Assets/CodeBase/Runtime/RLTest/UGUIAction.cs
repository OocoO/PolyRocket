using UnityEngine;

namespace RLTest
{
    public abstract class UGUIAction : IAction
    {
    }

    public class UGUIClick : UGUIAction
    {
        public Vector2 Position;
    }
}