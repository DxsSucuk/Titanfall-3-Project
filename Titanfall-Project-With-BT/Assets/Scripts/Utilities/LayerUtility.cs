using UnityEngine;

namespace Utilities
{
    public class LayerUtility
    {
        public static void SetLayerRecrusivly(Transform parent)
        {
            foreach (Transform child in parent)
            {
                if (child.gameObject.layer == 9)
                {
                    child.gameObject.layer = 6;
                }

                if (child.childCount > 0)
                {
                    SetLayerRecrusivly(child);
                }
            }
        }
    }
}