// -----------------------------------------------------------------------
// <copyright file="RecallAbility.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System.Collections.Generic;

    public class RecallAbility
    {
        public readonly Recallable recallable;
        private readonly Stack<FrameData> frames = new Stack<FrameData>();
        private FrameData currentFrame;

        internal RecallAbility(Recallable target)
        {
            var buffer = new List<FrameData>();
            RecallManager.Instance.TryGetFrames(target, buffer);

            foreach (var fd in buffer)
            {
                this.frames.Push(fd);
            }

            this.recallable = target;
            this.recallable.state = Recallable.State.Backward;
            this.currentFrame = new FrameData(target.rigidbody);
        }

        public int frameCount { get; private set; }

        internal int Tick()
        {
            if (this.frames.Count > 0)
            {
                FrameData top = this.frames.Pop();
                this.currentFrame = top;
            }

            this.currentFrame.ApplyTo(this.recallable.rigidbody);

            return this.frameCount++;
        }

        internal void Stop()
        {
            this.recallable.state = Recallable.State.Forward;
        }
    }
}
