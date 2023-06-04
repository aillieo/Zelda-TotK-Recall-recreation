namespace AillieoTech.Game.Rendering
{
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    public class RecallRendererFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private ScanningSettings scanningSettings;
        private ScanningPass scanningPass;

        public override void Create()
        {
            this.scanningPass = new ScanningPass("scanning");
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (RecallRendererSwitch.Instance.enableScanning)
            {
                this.scanningPass.renderPassEvent = this.scanningSettings.renderPassEvent;
                this.scanningPass.settings = this.scanningSettings;
                renderer.EnqueuePass(this.scanningPass);
            }
        }
    }
}
