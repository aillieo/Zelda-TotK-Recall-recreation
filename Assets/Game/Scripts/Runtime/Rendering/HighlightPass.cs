// -----------------------------------------------------------------------
// <copyright file="HighlightPass.cs" company="AillieoTech">
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
    internal class HighlightSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        public LayerMask layerMask;

        [RenderingLayerMask]
        public uint renderingLayerMask = 1;

        public Material overrideMaterial = null;

        public int overrideMaterialPassIndex;
    }

    internal class HighlightPass : ScriptableRenderPass
    {
        private static readonly List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>(Consts.defaultShaderTagIdList);

        private readonly ProfilingSampler sampler = new ProfilingSampler(nameof(HighlightPass));

        private readonly HighlightSettings settings;

        private FilteringSettings filteringSettings;

        public HighlightPass(HighlightSettings settings)
        {
            this.settings = settings;
            this.filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
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
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref this.filteringSettings);
                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }
        }
    }
}
