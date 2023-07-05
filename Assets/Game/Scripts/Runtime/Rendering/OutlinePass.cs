// -----------------------------------------------------------------------
// <copyright file="OutlinePass.cs" company="AillieoTech">
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
    internal class OutlineSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        public Material blitMaterial;
    }

    internal class OutlinePass : ScriptableRenderPass
    {
        private readonly OutlineSettings settings;

        private readonly ProfilingSampler sampler = new ProfilingSampler(nameof(OutlinePass));

        public OutlinePass(OutlineSettings settings)
        {
            this.settings = settings;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, this.sampler))
            {
                var renderer = renderingData.cameraData.renderer;
                var source = new RenderTargetIdentifier(Consts.outlineMaskRTId);
                var destination = renderer.cameraColorTarget;

                this.Blit(cmd, source, destination, this.settings.blitMaterial, 0);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(Consts.outlineMaskRTId);
        }
    }
}
