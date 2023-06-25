namespace AillieoTech.Game.Rendering
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable]
    public class OutlineMaskSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        public LayerMask layerMask;

        [RenderingLayerMask]
        public uint renderingLayerMask = 1;

        public Material overrideMaterial = null;

        public int overrideMaterialPassIndex;
    }

    public class OutlineMaskPass : ScriptableRenderPass
    {
        private static readonly List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>()
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("LightweightForward"),
        };

        private readonly string tag;

        private readonly OutlineMaskSettings settings;

        private FilteringSettings filteringSettings;

        public OutlineMaskPass(OutlineMaskSettings settings)
        {
            this.settings = settings;
            this.tag = nameof(OutlineMaskPass);
            this.filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.depthBufferBits = 0;
            blitTargetDescriptor.colorFormat = RenderTextureFormat.R16;

            cmd.GetTemporaryRT(Consts.outlineMaskRTId, blitTargetDescriptor, FilterMode.Point);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;

            DrawingSettings drawingSettings = this.CreateDrawingSettings(shaderTagIdList, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = this.settings.overrideMaterial;
            drawingSettings.overrideMaterialPassIndex = this.settings.overrideMaterialPassIndex;

            this.filteringSettings.layerMask = this.settings.layerMask;
            this.filteringSettings.renderingLayerMask = this.settings.renderingLayerMask;

            CommandBuffer cmd = CommandBufferPool.Get(this.tag);

            var destination = new RenderTargetIdentifier(Consts.outlineMaskRTId);
            var depthTexture = new RenderTargetIdentifier(Consts.depthTexId);
            cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, depthTexture, RenderBufferLoadAction.Load, RenderBufferStoreAction.DontCare);
            cmd.ClearRenderTarget(false, true, Color.clear);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref this.filteringSettings);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
