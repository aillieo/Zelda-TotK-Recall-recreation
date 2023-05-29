namespace AillieoTech.Game
{
    using UnityEngine;

    public readonly struct FrameData
    {
        public readonly Vector3 position;
        public readonly Quaternion rotation;

        public FrameData(Transform transform)
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
        }

        public void ApplyTo(Transform transform)
        {
            transform.position = this.position;
            transform.rotation = this.rotation;
        }
    }
}
