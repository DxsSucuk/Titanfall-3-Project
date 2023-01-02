using UnityEngine;

namespace Utilities
{
    public class LayerUtility
    {
        public static void ReplaceLayerRecursively(Transform parent, int searchId, int replaceId)
        {
            if (parent.gameObject.layer == searchId)
            {
                parent.gameObject.layer = replaceId;
            }
            
            foreach (Transform child in parent)
            {
                if (child.gameObject.layer == searchId)
                {
                    child.gameObject.layer = replaceId;
                }

                if (child.childCount > 0)
                {
                    ReplaceLayerRecursively(child, searchId, replaceId);
                }
            }
        }
    }
}