namespace AillieoTech.Game.Rendering
{
    using UnityEngine;

    public static class Consts
    {
        public static readonly int temporaryRTId = Shader.PropertyToID("_TempRT");

        public static readonly int outlineMaskRTId = Shader.PropertyToID("_OutlineMaskRT");

        public static readonly int depthTexId = Shader.PropertyToID("_CameraDepthTexture");

        public static readonly int outlineMask = 1;

        public static readonly int highlightMask = 2;
    }
}
