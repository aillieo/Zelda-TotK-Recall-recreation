namespace AillieoTech.Game.Rendering
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [Serializable]
    public class FadingSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        public Material blitMaterial;
    }

    internal class FadingPass : ScriptableRenderPass
    {
        private readonly FadingSettings settings;

        private readonly string tag;

        public FadingPass(FadingSettings settings)
        {
            this.settings = settings;
            this.tag = nameof(FadingPass);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.depthBufferBits = 0;

            cmd.GetTemporaryRT(Consts.temporaryRTId, blitTargetDescriptor, FilterMode.Bilinear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(this.tag);

            var renderer = renderingData.cameraData.renderer;
            var source = renderer.cameraColorTarget;
            var destination = new RenderTargetIdentifier(Consts.temporaryRTId);

            this.Blit(cmd, source, destination, this.settings.blitMaterial);
            this.Blit(cmd, destination, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(Consts.temporaryRTId);
        }
    }
}
