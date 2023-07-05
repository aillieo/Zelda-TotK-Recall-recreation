// -----------------------------------------------------------------------
// <copyright file="FadingPass.cs" company="AillieoTech">
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
    internal class FadingSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        public Material blitMaterial;
    }

    internal class FadingPass : ScriptableRenderPass
    {
        public float fadingPassTime = 0;
        public Vector3 fadingCenter;

        private readonly FadingSettings settings;

        private readonly ProfilingSampler sampler = new ProfilingSampler(nameof(FadingPass));

        public FadingPass(FadingSettings settings)
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

                Material material = this.settings.blitMaterial;
                material.SetFloat("_FadeValue", this.fadingPassTime);
                material.SetVector("_Center", this.fadingCenter);

                this.Blit(cmd, source, destination);
                this.Blit(cmd, destination, source, material);

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
