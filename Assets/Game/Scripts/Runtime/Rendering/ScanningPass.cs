namespace AillieoTech.Game.Rendering
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [Serializable]
    public class ScanningSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        public Material blitMaterial = null;
    }

    internal class ScanningPass : ScriptableRenderPass
    {
        public ScanningSettings settings;

        private readonly string profilerTag;

        private readonly int temporaryRTId = Shader.PropertyToID("_TempRT");

        public ScanningPass(string tag)
        {
            this.profilerTag = tag;
        }

        public FilterMode filterMode { get; set; }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.depthBufferBits = 0;

            cmd.GetTemporaryRT(this.temporaryRTId, blitTargetDescriptor, this.filterMode);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(this.profilerTag);

            var renderer = renderingData.cameraData.renderer;
            var source = renderer.cameraColorTarget;
            var destination = new RenderTargetIdentifier(this.temporaryRTId);

            this.Blit(cmd, source, destination, this.settings.blitMaterial);
            this.Blit(cmd, destination, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(this.temporaryRTId);
        }
    }
}
