// -----------------------------------------------------------------------
// <copyright file="MotionDriverHandler.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody), typeof(Recallable))]
    public abstract class MotionDriverHandler : MonoBehaviour
    {
        private Recallable recallableValue;

        private Rigidbody rigidbodyValue;

        private bool effectActive = true;

        protected new Rigidbody rigidbody
        {
            get
            {
                if (this.rigidbodyValue == null)
                {
                    this.rigidbodyValue = this.gameObject.GetComponent<Rigidbody>();
                }

                return this.rigidbodyValue;
            }
        }

        protected Recallable recallable
        {
            get
            {
                if (this.recallableValue == null)
                {
                    this.recallableValue = this.gameObject.GetComponent<Recallable>();
                }

                return this.recallableValue;
            }
        }

        internal void InvokeDisableMotionDrivers()
        {
            if (!this.effectActive)
            {
                return;
            }

            this.effectActive = false;

            this.DisableMotionDrivers();
        }

        internal void InvokeRestoreMotionDrivers()
        {
            if (this.effectActive)
            {
                return;
            }

            this.effectActive = true;
            this.RestoreMotionDrivers();
        }

        protected abstract void DisableMotionDrivers();

        protected abstract void RestoreMotionDrivers();
    }
}
