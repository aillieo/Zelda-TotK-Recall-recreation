// -----------------------------------------------------------------------
// <copyright file="Recallable.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
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

        private new Rigidbody rigidbody
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
            switch (this.state)
            {
                case State.Forward:
                    this.rigidbody.isKinematic = false;
                    this.rigidbody.detectCollisions = true;
                    break;
                case State.Backward:
                    this.rigidbody.isKinematic = true;
                    this.rigidbody.detectCollisions = true;
                    break;
                case State.Paused:
                    break;
            }
        }
    }
}
