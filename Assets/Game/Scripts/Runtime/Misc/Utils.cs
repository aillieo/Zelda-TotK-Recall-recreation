namespace AillieoTech.Game
{
    using UnityEngine;

    public static class Utils
    {
        public static void SetLayerRecursively(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            foreach (Transform child in gameObject.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }
}
