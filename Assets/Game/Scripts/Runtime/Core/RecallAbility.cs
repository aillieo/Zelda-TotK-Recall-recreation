namespace AillieoTech.Game
{
    using System.Collections.Generic;

    public class RecallAbility
    {
        public readonly Recallable recallable;
        private readonly Stack<FrameData> frames = new Stack<FrameData>();

        public RecallAbility(Recallable target)
        {
            var buffer = new List<FrameData>();
            RecallManager.Instance.TryGetFrames(target, buffer);

            foreach (var fd in buffer)
            {
                this.frames.Push(fd);
            }

            this.recallable = target;
            this.recallable.state = Recallable.State.Backward;
        }

        public int frameCount { get; private set; }

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
