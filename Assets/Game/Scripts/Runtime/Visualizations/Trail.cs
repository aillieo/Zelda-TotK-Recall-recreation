namespace AillieoTech.Game.Views
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Trail : MonoBehaviour
    {
        private readonly Stack<Trace> traces = new Stack<Trace>();

        [SerializeField]
        private GameObject arrowHead;

        [SerializeField]
        private LineRenderer arrowBody;

        [SerializeField]
        private GameObject meshRenderer;

        [SerializeField]
        private float traceInterval = 50;

        private Vector3[] rawPositions;
        private int rawPositionCount;

        public void LoadData(Recallable recallable)
        {
            this.ClearOldTraces();

            var active = recallable != null;
            this.arrowHead.SetActive(active);
            this.arrowBody.gameObject.SetActive(active);

            if (active)
            {
                var frames = new List<FrameData>();
                RecallManager.Instance.TryGetFrames(recallable, frames);

                this.arrowHead.transform.position = frames[0].position;

                // update trail
                this.rawPositions = new Vector3[frames.Count];
                this.rawPositionCount = frames.Count;
                for (var i = 0; i < frames.Count; i++)
                {
                    this.rawPositions[i] = frames[i].position;
                }

                this.arrowBody.positionCount = this.rawPositionCount;
                this.arrowBody.SetPositions(this.rawPositions);
                this.arrowBody.Simplify(0.1f);

                // update traces
                var index = 0;
                var distanceAccumulated = this.traceInterval;
                Vector3 lastPosition = frames[0].position;
                while (index < frames.Count)
                {
                    FrameData frame = frames[index];
                    Vector3 newPosition = frame.position;
                    var newDistance = Vector3.Distance(newPosition, lastPosition);
                    lastPosition = newPosition;
                    distanceAccumulated += newDistance;
                    if (distanceAccumulated < this.traceInterval)
                    {
                        index++;
                        continue;
                    }

                    distanceAccumulated = 0f;

                    if (this.traces.Count == 0)
                    {
                        GameObject template = recallable.gameObject;
                        GameObject s = this.CreateTrace(template, frame, true);
                        s.name = $"{recallable.name} {index}";
                        this.traces.Push(new Trace(index, s));
                    }
                    else
                    {
                        Trace first = this.traces.Peek();
                        if (first.index == index)
                        {
                            continue;
                        }

                        GameObject template = first.gameObject;
                        GameObject s = this.CreateTrace(template, frame, false);
                        s.name = $"{recallable.name} {index}";
                        this.traces.Push(new Trace(index, s));
                    }

                    index++;
                }
            }
        }

        public void Consume(int frameCount)
        {
            // update trail
            var positionCount = this.rawPositionCount - frameCount;
            if (positionCount < 2)
            {
                positionCount = 0;
            }

            this.arrowBody.positionCount = positionCount;
            this.arrowBody.SetPositions(this.rawPositions);
            this.arrowBody.Simplify(0.1f);

            // update traces
            if (this.traces.Count > 0)
            {
                Trace first = this.traces.Peek();
                if (first.index > this.rawPositionCount - frameCount)
                {
                    this.traces.Pop();
                    GameObject.Destroy(first.gameObject);
                }
            }
        }

        private void ClearOldTraces()
        {
            foreach (var s in this.traces)
            {
                GameObject.Destroy(s.gameObject);
            }

            this.traces.Clear();
        }

        private GameObject CreateTrace(GameObject template, FrameData frameData, bool createFromRecallable)
        {
            GameObject newGo;
            if (createFromRecallable)
            {
                newGo = GameObject.Instantiate<GameObject>(this.meshRenderer);
                Mesh mesh = this.CombineMeshes(template);
                newGo.GetComponent<MeshFilter>().mesh = mesh;
                newGo.transform.localScale = template.transform.localScale;
                Utils.SetLayerRecursively(newGo, LayerMask.NameToLayer("Trail"));
            }
            else
            {
                newGo = GameObject.Instantiate<GameObject>(template);
            }

            newGo.transform.SetPositionAndRotation(frameData.position, frameData.rotation);
            return newGo;
        }

        private Mesh CombineMeshes(GameObject root)
        {
            MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>();
            var combineInstances = new List<CombineInstance>();

            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (meshFilter.mesh != null)
                {
                    var combineInstance = new CombineInstance
                    {
                        mesh = meshFilter.mesh,
                        transform = root.transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix,
                    };
                    combineInstances.Add(combineInstance);
                }
            }

            var combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

            return combinedMesh;
        }

        public readonly struct Trace
        {
            public readonly int index;
            public readonly GameObject gameObject;

            public Trace(int index, GameObject gameObject)
            {
                this.index = index;
                this.gameObject = gameObject;
            }
        }
    }
}
