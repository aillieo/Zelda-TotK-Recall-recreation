namespace AillieoTech.Game
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum AbilityStage
    {
        Invalid = 0,
        Previewing,
        Casting,
    }

    public class RecallManager
    {
        public const int maxFrameCount = 1200;

        private static RecallManager instance;

        private readonly Dictionary<Recallable, Queue<FrameData>> managedRecallables = new Dictionary<Recallable, Queue<FrameData>>();

        private RecallAbility currentAbility;
        private Recallable potentialTarget;

        private FixedUpdateRunner fixedUpdateRunner;

        private bool isPreviewing = false;

        private RecallManager()
        {
            this.fixedUpdateRunner = new GameObject($"[{nameof(FixedUpdateRunner)}]").AddComponent<FixedUpdateRunner>();
            this.fixedUpdateRunner.hideFlags |= HideFlags.HideInHierarchy;
            UnityEngine.Object.DontDestroyOnLoad(this.fixedUpdateRunner);
            this.fixedUpdateRunner.onFixedUpdate += this.FixedUpdate;
        }

        public event Action<IEnumerable<Recallable>> OnPreviewBegin;

        public event Action<Recallable> OnPreviewTargetUpdate;

        public event Action OnPreviewEnd;

        public event Action<RecallAbility> OnAbilityBegin;

        public event Action<RecallAbility> OnAbilityUpdate;

        public event Action<RecallAbility> OnAbilityEnd;

        public static RecallManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RecallManager();
                }

                return instance;
            }
        }

        public AbilityStage stage
        {
            get
            {
                if (this.isPreviewing)
                {
                    return AbilityStage.Previewing;
                }

                if (this.currentAbility != null)
                {
                    return AbilityStage.Casting;
                }

                return AbilityStage.Invalid;
            }
        }

        public bool BeginPreview()
        {
            if (this.stage != AbilityStage.Invalid)
            {
                return false;
            }

            this.InternalBeginPreview();

            return true;
        }

        public bool TryCast()
        {
            if (this.stage != AbilityStage.Previewing)
            {
                return false;
            }

            Recallable currentTarget = this.potentialTarget;

            if (currentTarget != null && this.managedRecallables.TryGetValue(currentTarget, out Queue<FrameData> frames))
            {
                this.InternalEndPreview();

                var ability = new RecallAbility(currentTarget);
                this.currentAbility = ability;

                try
                {
                    this.OnAbilityBegin?.Invoke(ability);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                frames.Clear();
                return true;
            }

            return false;
        }

        public void AbortCurrentAbility()
        {
            if (this.isPreviewing)
            {
                this.InternalEndPreview();
                return;
            }

            if (this.currentAbility == null)
            {
                return;
            }

            this.StopCurrentAbility();
        }

        internal void Register(Recallable recallable)
        {
            this.managedRecallables.Add(recallable, new Queue<FrameData>());
        }

        internal void Unregister(Recallable recallable)
        {
            this.managedRecallables.Remove(recallable);
        }

        internal bool TryGetFrames(Recallable recallable, List<FrameData> toFill)
        {
            if (this.managedRecallables.TryGetValue(recallable, out Queue<FrameData> frames))
            {
                toFill.AddRange(frames);
                return true;
            }

            return false;
        }

        private void InternalBeginPreview()
        {
            this.isPreviewing = true;
            Physics.autoSimulation = false;
            try
            {
                var recallables = new List<Recallable>();
                recallables.AddRange(this.managedRecallables.Keys);
                this.OnPreviewBegin?.Invoke(recallables);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            foreach (var pair in this.managedRecallables)
            {
                pair.Key.state = Recallable.State.Paused;
            }
        }

        private void InternalEndPreview()
        {
            this.isPreviewing = false;
            Physics.autoSimulation = true;

            foreach (var pair in this.managedRecallables)
            {
                pair.Key.state = Recallable.State.Forward;
            }

            try
            {
                this.OnPreviewEnd?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            this.potentialTarget = null;
        }

        private Recallable FindTarget()
        {
            Camera cam = Camera.main;
            Vector3 start = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane));
            Vector3 end = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cam.farClipPlane));
            var hits = new RaycastHit[16];
            var hitCount = Physics.RaycastNonAlloc(start, end - start, hits, 10000, LayerMask.GetMask("Recallable"), QueryTriggerInteraction.Ignore);

            if (hitCount > 0)
            {
                GameObject hit = hits[0].collider.gameObject;
                if (hit.TryGetComponent<Recallable>(out Recallable recallable))
                {
                    return recallable;
                }
            }

            return null;
        }

        private void StopCurrentAbility()
        {
            this.currentAbility.Stop();

            try
            {
                this.OnAbilityEnd?.Invoke(this.currentAbility);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            this.currentAbility = null;
        }

        private void FixedUpdate()
        {
            if (this.isPreviewing)
            {
                Recallable oldTarget = this.potentialTarget;
                this.potentialTarget = this.FindTarget();

                if (this.potentialTarget != oldTarget)
                {
                    try
                    {
                        this.OnPreviewTargetUpdate?.Invoke(this.potentialTarget);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
            else
            {
                foreach (var pair in this.managedRecallables)
                {
                    Recallable recallable = pair.Key;
                    Queue<FrameData> frames = pair.Value;

                    switch (recallable.state)
                    {
                        case Recallable.State.Forward:
                            var frameData = new FrameData(recallable.transform);
                            frames.Enqueue(frameData);
                            while (frames.Count > maxFrameCount)
                            {
                                frames.Dequeue();
                            }

                            break;
                    }
                }
            }

            if (this.currentAbility != null)
            {
                var f = this.currentAbility.Tick();
                try
                {
                    this.OnAbilityUpdate?.Invoke(this.currentAbility);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (f > maxFrameCount)
                {
                    this.StopCurrentAbility();
                }
            }
        }
    }
}
