namespace AillieoTech.Game
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class Recallable : MonoBehaviour
    {
        public enum State
        {
            Forward = 0,
            Backward,
            Paused,
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

        private Rigidbody rigidbodyValue;

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
        private State stateValue;

        private void OnEnable()
        {
            RecallManager.Instance.Register(this);
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
                    this.rigidbody.detectCollisions = false;
                    break;
                case State.Paused:
                    this.rigidbody.isKinematic = true;
                    this.rigidbody.detectCollisions = false;
                    break;
            }
        }
    }
}
