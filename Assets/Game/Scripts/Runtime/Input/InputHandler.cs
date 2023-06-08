namespace AillieoTech.Game.Input
{
    using UnityEngine;

    public class InputHandler : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.C))
            {
                RecallManager.Instance.BeginPreview();
            }

            if (Input.GetKey(KeyCode.Q))
            {
                RecallManager.Instance.AbortCurrentAbility();
            }

            if (Input.GetMouseButtonDown(0))
            {
                RecallManager.Instance.TryCast();
            }
        }
    }
}
