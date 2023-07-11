// -----------------------------------------------------------------------
// <copyright file="MotionRecord.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System.Collections.Generic;

    public class MotionRecord
    {
        private readonly FixedQueue<FrameData> queue;

        public MotionRecord()
        {
            this.queue = new FixedQueue<FrameData>(RecallManager.maxFrameCount);
            this.staticFrames = 0;
        }

        public int staticFrames { get; private set; }

        public IEnumerable<FrameData> frames => this.queue;

        public void Clear()
        {
            this.queue.Clear();
            this.staticFrames = 0;
        }

        public bool Append(in FrameData frame)
        {
            if (this.queue.Count == 0)
            {
                this.queue.Enqueue(frame);
                return true;
            }

            this.queue.TryGetItem(this.queue.Count - 1, out FrameData last);
            if (last != frame)
            {
                this.staticFrames = 0;
                this.queue.Enqueue(frame);
                return true;
            }

            if (this.staticFrames > 60)
            {
                return false;
            }

            this.staticFrames++;
            this.queue.Enqueue(frame);
            return true;
        }
    }
}
