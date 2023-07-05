// -----------------------------------------------------------------------
// <copyright file="InputHandler.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game.Input
{
    using UnityEngine;

    internal class InputHandler : MonoBehaviour
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
