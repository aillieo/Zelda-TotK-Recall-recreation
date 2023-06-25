namespace AillieoTech.Game.Rendering
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;

    public static class Consts
    {
        public static readonly int temporaryRTId = Shader.PropertyToID("_TempRT");

        public static readonly int outlineMaskRTId = Shader.PropertyToID("_OutlineMaskRT");

        public static readonly int outlineMask = 1;

        public static readonly int highlightMask = 2;

        public static readonly IEnumerable<ShaderTagId> defaultShaderTagIdList = new List<ShaderTagId>()
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("LightweightForward"),
        }
        .AsReadOnly();
    }
}
