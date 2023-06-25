namespace AillieoTech.Game.Rendering
{
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    public class RecallRendererFeature : ScriptableRendererFeature
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
        private OutlineSettings outlineSettings;
        private OutlinePass outlinePass;

        [SerializeField]
        private FadingSettings fadingSettings;
        private FadingPass fadingPass;

        public override void Create()
        {
            this.scanningPass = new ScanningPass(this.scanningSettings);
            this.highlightPass = new HighlightPass(this.highlightSettings);
            this.outlineMaskPass = new OutlineMaskPass(this.outlineMaskSettings);
            this.outlinePass = new OutlinePass(this.outlineSettings);
            this.fadingPass = new FadingPass(this.fadingSettings);
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

            if (RecallRendererSwitch.Instance.enableOutline)
            {
                this.outlineMaskPass.renderPassEvent = this.outlineMaskSettings.renderPassEvent;
                renderer.EnqueuePass(this.outlineMaskPass);

                this.outlinePass.renderPassEvent = this.outlineSettings.renderPassEvent;
                renderer.EnqueuePass(this.outlinePass);
            }

            if (RecallRendererSwitch.Instance.enableFading)
            {
                this.fadingPass.renderPassEvent = this.fadingSettings.renderPassEvent;
                this.fadingPass.fadingPassTime = RecallRendererSwitch.Instance.fadingPassTime;
                renderer.EnqueuePass(this.fadingPass);
            }
        }
    }
}
