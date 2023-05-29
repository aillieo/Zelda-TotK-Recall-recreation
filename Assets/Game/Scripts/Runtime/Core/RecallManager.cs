namespace AillieoTech.Game
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class RecallManager : MonoBehaviour
    {
        public const int maxFrameCount = 6000;

        private static RecallManager instance;

        private readonly Dictionary<Recallable, Queue<FrameData>> managedRecallables = new Dictionary<Recallable, Queue<FrameData>>();

        private RecallAbility currentAbility;

        public event Action<Recallable> OnPreview;

        public event Action<RecallAbility> OnAbilityBegin;

        public event Action<RecallAbility> OnAbilityUpdate;

        public event Action<RecallAbility> OnAbilityEnd;

        public static RecallManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject($"[{nameof(RecallManager)}]").AddComponent<RecallManager>();
                    DontDestroyOnLoad(instance);
                }

                return instance;
            }
        }

        public void Register(Recallable recallable)
        {
            this.managedRecallables.Add(recallable, new Queue<FrameData>());
        }

        public void Unregister(Recallable recallable)
        {
            this.managedRecallables.Remove(recallable);
        }

        public void TryCast()
        {
            if (this.currentAbility != null)
            {
                return;
            }

            UnityEngine.Debug.Log("Cast!");
            Camera cam = Camera.main;
            Vector3 start = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane));
            Vector3 end = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cam.farClipPlane));
            var hits = new RaycastHit[16];
            var hitCount = Physics.RaycastNonAlloc(start, end - start, hits, 10000, LayerMask.GetMask("Recallable"), QueryTriggerInteraction.Ignore);
            if (hitCount > 0)
            {
                GameObject hit = hits[0].collider.gameObject;
                UnityEngine.Debug.Log(hit.name);
                if (hit.TryGetComponent<Recallable>(out Recallable recallable) && this.managedRecallables.TryGetValue(recallable, out Queue<FrameData> frames))
                {
                    var ability = new RecallAbility(recallable, frames);
                    this.currentAbility = ability;
                    frames.Clear();

                    try
                    {
                        this.OnAbilityBegin?.Invoke(ability);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        public void AbortCurrentAbility()
        {
            if (this.currentAbility == null)
            {
                return;
            }

            this.StopCurrentAbility();
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

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
        }

        private void FixedUpdate()
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
