namespace AillieoTech.Game.Rendering
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [Serializable]
    public class OutlineSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        public Material blitMaterial;
    }

    internal class OutlinePass : ScriptableRenderPass
    {
        private readonly OutlineSettings settings;

        private readonly string tag;

        private readonly int outlineMaskRTId = Shader.PropertyToID("_OutlineMaskRT");

        public OutlinePass(OutlineSettings settings)
        {
            this.settings = settings;
            this.tag = nameof(OutlinePass);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            // blitTargetDescriptor.depthBufferBits = 0;
            // blitTargetDescriptor.colorFormat = RenderTextureFormat.R16;

            // cmd.GetTemporaryRT(this.outlineMaskRTId, blitTargetDescriptor, FilterMode.Bilinear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(this.tag);

            var renderer = renderingData.cameraData.renderer;
            var source = new RenderTargetIdentifier(this.outlineMaskRTId);
            var destination = renderer.cameraColorTarget;

            this.Blit(cmd, source, destination, this.settings.blitMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(this.outlineMaskRTId);
        }
    }
}
