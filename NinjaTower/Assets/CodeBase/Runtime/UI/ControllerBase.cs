namespace Carotaa.Code
{
    // UIController - UIPendingRecord - UIRecord is connected by reflection
    // SO, You can use attribute to extend some custom behaviour
    public abstract class ControllerBase
    {
        public abstract PageLayer Layer { get; }

        public abstract string GetPanelAddress();

        // event functions
        public virtual void OnPreload(object[] param)
        {
        }

        public virtual void OnActive()
        {
        }

        public virtual void OnDeActive()
        {
        }

        public virtual void OnDestroy()
        {
        }

        public int SortingLayerID()
        {
            var layer = Layer;
            switch (layer)
            {
                case PageLayer.Slide:
                    return CameraUtility.GetLayerID(CameraUtility.SLIDE_LAYER);
                case PageLayer.Pop:
                    return CameraUtility.GetLayerID(CameraUtility.POP_LAYER);
                case PageLayer.Toast:
                    return CameraUtility.GetLayerID(CameraUtility.TOAST_LAYER);
                default:
                    return CameraUtility.GetLayerID(CameraUtility.DEFAULT_LAYER);
            }
        }
    }

    public enum PageLayer
    {
        Slide = 1,
        Pop = 2,
        Toast = 3
    }
}