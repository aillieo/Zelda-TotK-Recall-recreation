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

        public static Vector3 WorldToScreenPointSafe(this Camera camera, Vector3 position)
        {
            Vector3 cameraNormal = camera.transform.forward;
            Vector3 nearClipCenter = camera.transform.position + (cameraNormal * camera.nearClipPlane);
            Vector3 positionToClipCenter = position - nearClipCenter;
            var distanceToClip = Vector3.Dot(cameraNormal, positionToClipCenter);

            if (distanceToClip < 0)
            {
                if (camera.orthographic)
                {
                    var projectOnClip = Vector3.ProjectOnPlane(positionToClipCenter, cameraNormal);
                    position += projectOnClip * 1000f;
                }
                else
                {
                    Vector3 positionToCamera = position - camera.transform.position;
                    var projectToCamPlane = Vector3.Dot(cameraNormal, positionToCamera);
                    position -= cameraNormal * projectToCamPlane * 1.001f;
                }
            }

            return camera.WorldToScreenPoint(position);
        }
    }
}
