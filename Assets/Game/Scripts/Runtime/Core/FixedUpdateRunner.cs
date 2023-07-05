// -----------------------------------------------------------------------
// <copyright file="FixedUpdateRunner.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

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
