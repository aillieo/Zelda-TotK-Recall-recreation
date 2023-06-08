namespace AillieoTech.Game.Views
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Trace : MonoBehaviour
    {
        [SerializeField]
        private GameObject arrowHead;

        [SerializeField]
        private LineRenderer arrowBody;

        [SerializeField]
        private GameObject meshRenderer;

        [SerializeField]
        private int snapshotInterval = 240;

        private readonly List<GameObject> snapshots = new List<GameObject>();

        public void LoadData(Recallable recallable)
        {
            this.ClearOldSnapshots();

            var active = recallable != null;
            this.arrowHead.SetActive(active);
            this.arrowBody.gameObject.SetActive(active);

            if (active)
            {
                var frames = new List<FrameData>();
                RecallManager.Instance.TryGetFrames(recallable, frames);

                this.arrowHead.transform.position = frames[0].position;
                this.arrowBody.positionCount = frames.Count;
                for (var i = 0; i < frames.Count; i++)
                {
                    this.arrowBody.SetPosition(i, frames[i].position);
                }

                this.arrowBody.Simplify(0.8f);
                var count = Mathf.CeilToInt((float)frames.Count / this.snapshotInterval);
                var interval = (float)frames.Count / count;
                float f = 0;
                while (true)
                {
                    var index = Mathf.RoundToInt(f);
                    if (index < 0 || index >= frames.Count)
                    {
                        break;
                    }

                    FrameData frame = frames[index];
                    if (this.snapshots.Count == 0)
                    {
                        GameObject template = recallable.gameObject;
                        GameObject s = this.CreateSnapshot(template, frame, true);
                        s.name = $"{recallable.name} {this.snapshots.Count}";
                        this.snapshots.Add(s);
                    }
                    else
                    {
                        GameObject template = this.snapshots[0];
                        GameObject s = this.CreateSnapshot(template, frame, false);
                        s.name = $"{recallable.name} {this.snapshots.Count}";
                        this.snapshots.Add(s);
                    }

                    f += interval;
                }
            }
        }

        private void ClearOldSnapshots()
        {
            foreach (var s in this.snapshots)
            {
                GameObject.Destroy(s);
            }

            this.snapshots.Clear();
        }

        private GameObject CreateSnapshot(GameObject template, FrameData frameData, bool createFromRecallable)
        {
            GameObject newGo;
            if (createFromRecallable)
            {
                newGo = GameObject.Instantiate<GameObject>(this.meshRenderer);
                Mesh mesh = this.CombineMeshes(template);
                newGo.GetComponent<MeshFilter>().mesh = mesh;
                newGo.transform.localScale = template.transform.localScale;
                Utils.SetLayerRecursively(newGo, LayerMask.NameToLayer("Trace"));
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
    }
}
