// -----------------------------------------------------------------------
// <copyright file="Recallable.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class Recallable : MonoBehaviour
    {
        private Rigidbody rigidbodyValue;
        private State stateValue;

        public enum State
        {
            Forward = 0,
            Backward,
            Paused,
        }

        public State state
        {
            get => this.stateValue;
            set
            {
                if (this.stateValue != value)
                {
                    this.stateValue = value;
                    this.OnStateChanged();
                }
            }
        }

        internal new Rigidbody rigidbody
        {
            get
            {
                if (this.rigidbodyValue == null)
                {
                    this.rigidbodyValue = this.GetComponent<Rigidbody>();
                }

                return this.rigidbodyValue;
            }
        }

        private void OnEnable()
        {
            RecallManager.Instance.Register(this);
            this.OnStateChanged();
        }

        private void OnDisable()
        {
            RecallManager.Instance.Unregister(this);
        }

        private void OnStateChanged()
        {
            var list = new List<MotionDriverHandler>();

            switch (this.state)
            {
                case State.Forward:
                    this.gameObject.GetComponents(list);
                    foreach (var c in list)
                    {
                        c.InvokeRestoreMotionDrivers();
                    }

                    this.rigidbody.velocity = default;
                    this.rigidbody.angularVelocity = default;

                    break;
                case State.Backward:
                    this.gameObject.GetComponents(list);
                    foreach (var c in list)
                    {
                        c.InvokeDisableMotionDrivers();
                    }

                    this.rigidbody.velocity = default;
                    this.rigidbody.angularVelocity = default;

                    break;
                case State.Paused:
                    break;
            }

            list.Clear();
        }
    }
}
