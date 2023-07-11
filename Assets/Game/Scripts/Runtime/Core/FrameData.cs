// -----------------------------------------------------------------------
// <copyright file="FrameData.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using UnityEngine;

    public readonly struct FrameData : IEquatable<FrameData>
    {
        public readonly Vector3 position;
        public readonly Quaternion rotation;

        public FrameData(Rigidbody rigidbody)
        {
            this.position = rigidbody.position;
            this.rotation = rigidbody.rotation;
        }

        public static bool operator ==(FrameData lhs, FrameData rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(FrameData lhs, FrameData rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(FrameData other)
        {
            return this.position == other.position && this.rotation == other.rotation;
        }

        public override bool Equals(object obj)
        {
            if (obj is FrameData frameData)
            {
                return this.Equals(frameData);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.position.GetHashCode() ^ this.rotation.GetHashCode() << 2;
        }

        internal void ApplyTo(Rigidbody rigidbody)
        {
            var deltaRotation = Quaternion.FromToRotation(rigidbody.rotation * Vector3.forward, this.rotation * Vector3.forward);
            deltaRotation.ToAngleAxis(out var deltaAngle, out Vector3 axis);
            deltaAngle = Mathf.Deg2Rad * deltaAngle;
            Vector3 deltaAngularVelocity = ((deltaAngle * axis) / Time.fixedDeltaTime) - rigidbody.angularVelocity;
            rigidbody.AddTorque(deltaAngularVelocity, ForceMode.VelocityChange);

            Vector3 deltaPosition = this.position - rigidbody.position;
            Vector3 deltaVelocity = (deltaPosition / Time.fixedDeltaTime) - rigidbody.velocity;
            rigidbody.AddForce(deltaVelocity, ForceMode.VelocityChange);
        }
    }
}
