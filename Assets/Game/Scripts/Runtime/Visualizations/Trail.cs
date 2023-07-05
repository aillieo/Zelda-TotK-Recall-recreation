// -----------------------------------------------------------------------
// <copyright file="Trail.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game.Views
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class Trail : MonoBehaviour
    {
        private static readonly float headLength = 3f;
        private static readonly float headWidth = 3f;
        private static readonly float bodyWidth = 0.5f;
        private static readonly float widthCurveFix = 0.01f;

        private readonly Stack<Trace> traces = new Stack<Trace>();

        private LineRenderer lineRendererValue;

        [SerializeField]
        private GameObject meshRenderer;

        [SerializeField]
        private float traceInterval = 50;

        private Vector3[] rawPositions = Array.Empty<Vector3>();
        private int rawPositionCount;

        private LineRenderer lineRenderer
        {
            get
            {
                if (this.lineRendererValue == null)
                {
                    this.lineRendererValue = this.GetComponent<LineRenderer>();
                }

                return this.lineRendererValue;
            }
        }

        public void LoadData(Recallable recallable, bool showFullTrail)
        {
            this.ClearOldTraces();

            var active = recallable != null;
            this.lineRenderer.enabled = active;

            if (active)
            {
                var frames = new List<FrameData>();
                RecallManager.Instance.TryGetFrames(recallable, frames);

                if (frames.Count == 0)
                {
                    return;
                }

                var startIndex = 0;
                if (!showFullTrail)
                {
                    startIndex = Mathf.FloorToInt(frames.Count * 0.75f);
                }

                // update trail
                this.rawPositionCount = frames.Count - startIndex;
                if (this.rawPositions.Length < this.rawPositionCount)
                {
                    Array.Resize(ref this.rawPositions, this.rawPositionCount);
                }

                for (var i = startIndex; i < frames.Count; i++)
                {
                    this.rawPositions[i - startIndex] = frames[i].position;
                }

                this.lineRenderer.positionCount = this.rawPositionCount;
                this.lineRenderer.SetPositions(this.rawPositions);
                this.lineRenderer.Simplify(0.1f);
                this.CreateArrowHead();

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

            this.lineRenderer.positionCount = positionCount;
            this.lineRenderer.SetPositions(this.rawPositions);
            this.lineRenderer.Simplify(0.1f);
            this.CreateArrowHead();

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

        private void CreateArrowHead()
        {
            if (this.lineRenderer.positionCount < 2)
            {
                return;
            }

            var length = Utils.GetTotalLength(this.lineRenderer);

            if (length < headLength)
            {
                return;
            }

            var headPercent = headLength / length;
            var fixPercent = widthCurveFix / length;
            headPercent = Mathf.Clamp(headPercent, fixPercent, 1 - fixPercent);

            if (this.lineRenderer.widthCurve == null || this.lineRenderer.widthCurve.length < 4)
            {
                this.lineRenderer.widthCurve = new AnimationCurve(
                    new Keyframe(0, 0f),
                    new Keyframe(headPercent - fixPercent, headWidth),
                    new Keyframe(headPercent + fixPercent, bodyWidth),
                    new Keyframe(1, bodyWidth));
            }
            else
            {
                this.lineRenderer.widthCurve.RemoveKey(2);
                this.lineRenderer.widthCurve.RemoveKey(1);
                this.lineRenderer.widthCurve.AddKey(new Keyframe(headPercent - fixPercent, headWidth));
                this.lineRenderer.widthCurve.AddKey(new Keyframe(headPercent + fixPercent, bodyWidth));
            }

            var fixHeadPoint = 0;
            Vector3 lastPosition = this.lineRenderer.GetPosition(0);
            float distanceAccumulated = 0;
            for (var i = 1; i < this.lineRenderer.positionCount; i++)
            {
                Vector3 p = this.lineRenderer.GetPosition(i);
                var thisDist = Vector3.Distance(p, lastPosition);

                if (fixHeadPoint == 0)
                {
                    if (distanceAccumulated < headLength - widthCurveFix && distanceAccumulated + thisDist > headLength - widthCurveFix)
                    {
                        var t = (headLength - widthCurveFix - distanceAccumulated) / thisDist;
                        var newPosition = Vector3.Lerp(lastPosition, p, t);
                        Utils.InsertPosition(this.lineRenderer, i, newPosition);
                        i++;
                        lastPosition = newPosition;
                        thisDist = headLength - widthCurveFix - distanceAccumulated;
                        distanceAccumulated = headLength - widthCurveFix;
                        fixHeadPoint++;
                    }
                }

                if (fixHeadPoint == 1)
                {
                    if (distanceAccumulated < headLength + widthCurveFix && distanceAccumulated + thisDist > headLength + widthCurveFix)
                    {
                        var t = (headLength + widthCurveFix - distanceAccumulated) / thisDist;
                        var newPosition = Vector3.Lerp(lastPosition, p, t);
                        Utils.InsertPosition(this.lineRenderer, i, newPosition);
                        i++;
                        lastPosition = newPosition;
                        thisDist = headLength + widthCurveFix - distanceAccumulated;
                        distanceAccumulated = headLength + widthCurveFix;
                        fixHeadPoint++;
                    }
                }

                lastPosition = p;
                distanceAccumulated += thisDist;

                if (fixHeadPoint >= 2)
                {
                    break;
                }

                if (distanceAccumulated > headLength + widthCurveFix)
                {
                    break;
                }
            }
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
