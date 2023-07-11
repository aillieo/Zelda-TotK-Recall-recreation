// -----------------------------------------------------------------------
// <copyright file="HingeJointHandler.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    [RequireComponent(typeof(HingeJoint))]
    public class HingeJointHandler : MotionDriverHandler
    {
        private new HingeJoint hingeJoint;

        private JointMotor motor;
        private bool useMotor;

        protected override void DisableMotionDrivers()
        {
            if (this.hingeJoint == null)
            {
                this.hingeJoint = this.GetComponent<HingeJoint>();
            }

            this.motor = this.hingeJoint.motor;
            this.useMotor = this.hingeJoint.useMotor;
        }

        protected override void RestoreMotionDrivers()
        {
            if (this.hingeJoint == null)
            {
                return;
            }

            this.hingeJoint.useMotor = this.useMotor;
            if (this.useMotor)
            {
                this.hingeJoint.motor = this.motor;
            }
        }
    }
}
