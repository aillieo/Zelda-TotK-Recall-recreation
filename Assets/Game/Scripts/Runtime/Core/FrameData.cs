// -----------------------------------------------------------------------
// <copyright file="FrameData.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

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

        internal void ApplyTo(Transform transform)
        {
            transform.SetPositionAndRotation(this.position, this.rotation);
        }
    }
}
