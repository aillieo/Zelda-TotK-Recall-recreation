namespace AillieoTech.Game.Views
{
    using AillieoTech.Game.Rendering;
    using UnityEngine;

    public class RenderController : MonoBehaviour
    {
        [SerializeField]
        private Trace tracePrefab;

        private Trace traceInstance;

        private void OnEnable()
        {
            RecallManager.Instance.OnPreviewBegin += this.OnPreviewBegin;
            RecallManager.Instance.OnPreviewTargetUpdate += this.OnPreviewTargetUpdate;
            RecallManager.Instance.OnPreviewEnd += this.OnPreviewEnd;
        }

        private void OnDisable()
        {
            RecallManager.Instance.OnPreviewBegin -= this.OnPreviewBegin;
            RecallManager.Instance.OnPreviewTargetUpdate -= this.OnPreviewTargetUpdate;
            RecallManager.Instance.OnPreviewEnd -= this.OnPreviewEnd;
        }

        private void OnPreviewBegin()
        {
            RecallRendererSwitch.Instance.enableScanning = true;
            RecallRendererSwitch.Instance.enableHighlight = true;

            if (this.traceInstance == null)
            {
                this.traceInstance = Instantiate<Trace>(this.tracePrefab);
            }

            this.traceInstance.LoadData(null);
        }

        private void OnPreviewTargetUpdate(Recallable recallable)
        {
            this.traceInstance.LoadData(recallable);
        }

        private void OnPreviewEnd()
        {
            RecallRendererSwitch.Instance.enableScanning = false;
            RecallRendererSwitch.Instance.enableHighlight = false;

            if (this.traceInstance != null)
            {
                this.traceInstance.LoadData(null);

                GameObject.Destroy(this.traceInstance.gameObject);
                this.traceInstance = null;
            }
        }
    }
}
