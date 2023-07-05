// -----------------------------------------------------------------------
// <copyright file="OutlineMaskPass.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game.Rendering
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [Serializable]
    internal class OutlineMaskSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        public LayerMask layerMask;

        [RenderingLayerMask]
        public uint renderingLayerMask = 1;

        public Material overrideMaterial = null;

        public int overrideMaterialPassIndex;
    }

    internal class OutlineMaskPass : ScriptableRenderPass
    {
        private static readonly List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>(Consts.defaultShaderTagIdList);

        private readonly ProfilingSampler sampler = new ProfilingSampler(nameof(OutlineMaskPass));

        private readonly OutlineMaskSettings settings;

        private FilteringSettings filteringSettings;

        public OutlineMaskPass(OutlineMaskSettings settings)
        {
            this.settings = settings;
            this.filteringSettings = new FilteringSettings(RenderQueueRange.all);
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

            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, this.sampler))
            {
                var renderer = renderingData.cameraData.renderer;
                var source = renderer.cameraColorTarget;

                var destination = new RenderTargetIdentifier(Consts.outlineMaskRTId);
                var depthTexture = new RenderTargetIdentifier(Consts.depthTexId);
                cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, depthTexture, RenderBufferLoadAction.Load, RenderBufferStoreAction.DontCare);
                cmd.ClearRenderTarget(false, true, Color.clear);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref this.filteringSettings);

                context.ExecuteCommandBuffer(cmd);

                cmd.Clear();
                cmd.SetRenderTarget(source, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }
        }
    }
}
