// -----------------------------------------------------------------------
// <copyright file="RecallRendererSwitch.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game.Rendering
{
    using UnityEngine;

    internal class RecallRendererSwitch
    {
        public static readonly RecallRendererSwitch Instance = new RecallRendererSwitch();

        public bool enableScanning = false;

        public bool enableFading = false;
        public float fadingPassTime = 0f;
        public Vector3 fadingCenter;

        public bool enableOutline = false;

        public bool enableHighlight = false;
    }
}
