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
        private FadingSettings fadingSettings;
        private FadingPass fadingPass;

        public override void Create()
        {
            this.scanningPass = new ScanningPass("scanning");
            this.fadingPass = new FadingPass("fading");
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (RecallRendererSwitch.Instance.enableScanning)
            {
                this.scanningPass.renderPassEvent = this.scanningSettings.renderPassEvent;
                this.scanningPass.settings = this.scanningSettings;
                renderer.EnqueuePass(this.scanningPass);
            }

            if (RecallRendererSwitch.Instance.enableFading)
            {
                this.fadingPass.renderPassEvent = this.fadingSettings.renderPassEvent;
                this.fadingPass.settings = this.fadingSettings;
                renderer.EnqueuePass(this.fadingPass);
            }
        }
    }
}
