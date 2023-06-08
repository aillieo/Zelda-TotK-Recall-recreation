namespace AillieoTech.Game.Rendering
{
    using UnityEngine.Experimental.Rendering.Universal;
    using UnityEngine.Rendering.Universal;

    internal class HighlightPass : RenderObjects
    {
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (RecallRendererSwitch.Instance.enableHighlight)
            {
                base.AddRenderPasses(renderer, ref renderingData);
            }
        }
    }
}
