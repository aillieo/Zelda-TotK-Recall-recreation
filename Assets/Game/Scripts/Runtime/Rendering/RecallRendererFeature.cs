// -----------------------------------------------------------------------
// <copyright file="RecallRendererFeature.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game.Rendering
{
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    internal class RecallRendererFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private ScanningSettings scanningSettings;
        private ScanningPass scanningPass;

        [SerializeField]
        private HighlightSettings highlightSettings;
        private HighlightPass highlightPass;

        [SerializeField]
        private OutlineMaskSettings outlineMaskSettings;
        private OutlineMaskPass outlineMaskPass;

        [SerializeField]
        private FadingSettings fadingSettings;
        private FadingPass fadingPass;

        [SerializeField]
        private OutlineSettings outlineSettings;
        private OutlinePass outlinePass;

        public override void Create()
        {
            this.scanningPass = new ScanningPass(this.scanningSettings);
            this.highlightPass = new HighlightPass(this.highlightSettings);
            this.fadingPass = new FadingPass(this.fadingSettings);
            this.outlineMaskPass = new OutlineMaskPass(this.outlineMaskSettings);
            this.outlinePass = new OutlinePass(this.outlineSettings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (RecallRendererSwitch.Instance.enableScanning)
            {
                this.scanningPass.renderPassEvent = this.scanningSettings.renderPassEvent;
                renderer.EnqueuePass(this.scanningPass);
            }

            if (RecallRendererSwitch.Instance.enableHighlight)
            {
                this.highlightPass.renderPassEvent = this.highlightSettings.renderPassEvent;
                renderer.EnqueuePass(this.highlightPass);
            }

            if (RecallRendererSwitch.Instance.enableFading)
            {
                this.fadingPass.renderPassEvent = this.fadingSettings.renderPassEvent;
                this.fadingPass.fadingPassTime = RecallRendererSwitch.Instance.fadingPassTime;
                this.fadingPass.fadingCenter = RecallRendererSwitch.Instance.fadingCenter;
                renderer.EnqueuePass(this.fadingPass);
            }

            if (RecallRendererSwitch.Instance.enableOutline)
            {
                this.outlineMaskPass.renderPassEvent = this.outlineMaskSettings.renderPassEvent;
                renderer.EnqueuePass(this.outlineMaskPass);

                this.outlinePass.renderPassEvent = this.outlineSettings.renderPassEvent;
                renderer.EnqueuePass(this.outlinePass);
            }
        }
    }
}
