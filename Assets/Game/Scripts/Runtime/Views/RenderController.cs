namespace AillieoTech.Game.Views
{
    using AillieoTech.Game.Rendering;
    using UnityEngine;

    public class RenderController : MonoBehaviour
    {
        [SerializeField]
        private GameObject tracePrefab;

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
        }

        private void OnPreviewTargetUpdate(Recallable recallable)
        {
            Debug.Log(recallable);
        }

        private void OnPreviewEnd()
        {
            RecallRendererSwitch.Instance.enableScanning = false;
        }
    }
}
