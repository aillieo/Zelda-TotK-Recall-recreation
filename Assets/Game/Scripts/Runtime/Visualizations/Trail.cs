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

        public void LoadData(Recallable recallable, bool showFullTrail)
        {
            this.ClearOldTraces();

            var active = recallable != null;
            this.arrowHead.SetActive(active);
            this.arrowBody.gameObject.SetActive(active);

            if (active)
            {
                var frames = new List<FrameData>();
                RecallManager.Instance.TryGetFrames(recallable, frames);

                var startIndex = 0;
                if (!showFullTrail)
                {
                    startIndex = Mathf.FloorToInt(frames.Count * 0.75f);
                }

                this.arrowHead.transform.position = frames[startIndex].position;

                // update trail
                this.rawPositionCount = frames.Count - startIndex;
                this.rawPositions = new Vector3[this.rawPositionCount];
                for (var i = startIndex; i < frames.Count; i++)
                {
                    this.rawPositions[i - startIndex] = frames[i].position;
                }

                this.arrowBody.positionCount = this.rawPositionCount;
                this.arrowBody.SetPositions(this.rawPositions);
                this.arrowBody.Simplify(0.1f);

                // update traces
                var distanceAccumulated = 0f;
                Vector3 lastPosition = frames[startIndex].position;
                while (startIndex < frames.Count)
                {
                    FrameData frame = frames[startIndex];
                    Vector3 newPosition = frame.position;
                    var newDistance = Vector3.Distance(newPosition, lastPosition);
                    lastPosition = newPosition;
                    distanceAccumulated += newDistance;
                    if (distanceAccumulated < this.traceInterval)
                    {
                        startIndex++;
                        continue;
                    }

                    distanceAccumulated = 0f;

                    if (this.traces.Count == 0)
                    {
                        GameObject template = recallable.gameObject;
                        GameObject s = this.CreateTrace(template, frame, true);
                        s.name = $"{recallable.name} {startIndex}";
                        this.traces.Push(new Trace(startIndex, s));
                    }
                    else
                    {
                        Trace first = this.traces.Peek();
                        if (first.index == startIndex)
                        {
                            continue;
                        }

                        GameObject template = first.gameObject;
                        GameObject s = this.CreateTrace(template, frame, false);
                        s.name = $"{recallable.name} {startIndex}";
                        this.traces.Push(new Trace(startIndex, s));
                    }

                    startIndex++;
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
