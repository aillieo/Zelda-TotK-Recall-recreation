namespace AillieoTech.Game.Rendering
{
    public class RecallRendererSwitch
    {
        public static readonly RecallRendererSwitch Instance = new RecallRendererSwitch();

        public bool enableScanning = false;

        public bool enableFading = false;
        public float fadingPassTime = 0f;

        public bool enableOutline = false;

        public bool enableHighlight = false;
    }
}
