namespace AillieoTech.Game
{
    using System;
    using UnityEngine;

    internal class FixedUpdateRunner : MonoBehaviour
    {
        internal event Action onFixedUpdate;

        private void FixedUpdate()
        {
            this.onFixedUpdate?.Invoke();
        }
    }
}
