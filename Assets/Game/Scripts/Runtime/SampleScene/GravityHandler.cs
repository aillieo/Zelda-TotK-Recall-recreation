// -----------------------------------------------------------------------
// <copyright file="GravityHandler.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    public class GravityHandler : MotionDriverHandler
    {
        private bool useGravity;

        protected override void DisableMotionDrivers()
        {
            this.useGravity = this.rigidbody.useGravity;
            this.rigidbody.useGravity = false;
        }

        protected override void RestoreMotionDrivers()
        {
            this.rigidbody.useGravity = this.useGravity;
        }
    }
}
