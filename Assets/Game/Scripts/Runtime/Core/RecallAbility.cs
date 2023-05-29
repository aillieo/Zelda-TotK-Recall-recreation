namespace AillieoTech.Game
{
    using System.Collections.Generic;

    public class RecallAbility
    {
        private Stack<FrameData> frames = new Stack<FrameData>();
        public readonly Recallable recallable;
        public int frameCount { get; private set; }

        public RecallAbility(Recallable target, IEnumerable<FrameData> rawFrames)
        {
            foreach (var fd in rawFrames)
            {
                this.frames.Push(fd);
            }

            this.recallable = target;
            this.recallable.state = Recallable.State.Backward;
        }

        public int Tick()
        {
            if (this.frames.Count > 0)
            {
                FrameData top = this.frames.Pop();
                top.ApplyTo(this.recallable.transform);
            }

            return this.frameCount++;
        }

        public void Stop()
        {
            this.recallable.state = Recallable.State.Forward;
        }
    }
}
