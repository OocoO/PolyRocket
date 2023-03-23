using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Carotaa.Code
{
    public class Empty4Raycaster : MaskableGraphic
    {
        protected Empty4Raycaster()
        {
            useLegacyMeshGeneration = false;
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
