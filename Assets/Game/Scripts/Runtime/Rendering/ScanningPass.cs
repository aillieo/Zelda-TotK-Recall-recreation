// -----------------------------------------------------------------------
// <copyright file="ScanningPass.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game.Rendering
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [Serializable]
    internal class ScanningSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        public Material blitMaterial = null;
    }

    internal class ScanningPass : ScriptableRenderPass
    {
        private readonly ScanningSettings settings;

        private readonly ProfilingSampler sampler = new ProfilingSampler(nameof(ScanningPass));

        public ScanningPass(ScanningSettings settings)
        {
            this.settings = settings;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.depthBufferBits = 0;

            cmd.GetTemporaryRT(Consts.temporaryRTId, blitTargetDescriptor, FilterMode.Bilinear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, this.sampler))
            {
                var renderer = renderingData.cameraData.renderer;
                var source = renderer.cameraColorTarget;
                var destination = new RenderTargetIdentifier(Consts.temporaryRTId);

                this.Blit(cmd, source, destination, this.settings.blitMaterial);
                this.Blit(cmd, destination, source);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(Consts.temporaryRTId);
        }
    }
}
