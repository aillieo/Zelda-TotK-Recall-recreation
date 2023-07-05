// -----------------------------------------------------------------------
// <copyright file="Utils.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class Utils
    {
        private static readonly List<Renderer> rendererCompBuffer = new List<Renderer>();
        private static Vector3[] positionBuffer = Array.Empty<Vector3>();

        public static void SetLayerRecursively(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            foreach (Transform child in gameObject.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        public static void AddRenderingLayerMask(GameObject go, int renderingLayerMask)
        {
            go.GetComponentsInChildren(rendererCompBuffer);
            foreach (var r in rendererCompBuffer)
            {
                r.renderingLayerMask |= (uint)(1 << renderingLayerMask);
            }

            rendererCompBuffer.Clear();
        }

        public static void RemoveRenderingLayerMask(GameObject go, int renderingLayerMask)
        {
            go.GetComponentsInChildren(rendererCompBuffer);
            foreach (var r in rendererCompBuffer)
            {
                r.renderingLayerMask &= (uint)(~(1 << renderingLayerMask));
            }

            rendererCompBuffer.Clear();
        }

        public static Vector3 WorldToScreenPointSafe(this Camera camera, Vector3 position)
        {
            Vector3 cameraNormal = camera.transform.forward;
            Vector3 nearClipCenter = camera.transform.position + (cameraNormal * camera.nearClipPlane);
            Vector3 positionToClipCenter = position - nearClipCenter;
            var distanceToClip = Vector3.Dot(cameraNormal, positionToClipCenter);

            if (distanceToClip < 0)
            {
                if (camera.orthographic)
                {
                    var projectOnClip = Vector3.ProjectOnPlane(positionToClipCenter, cameraNormal);
                    position += projectOnClip * 1000f;
                }
                else
                {
                    Vector3 positionToCamera = position - camera.transform.position;
                    var projectToCamPlane = Vector3.Dot(cameraNormal, positionToCamera);
                    position -= cameraNormal * projectToCamPlane * 1.001f;
                }
            }

            return camera.WorldToScreenPoint(position);
        }

        public static float GetTotalLength(LineRenderer lineRenderer)
        {
            if (lineRenderer.positionCount <= 1)
            {
                return 0;
            }

            var totalLength = 0f;
            Vector3 lastPoint = lineRenderer.GetPosition(0);
            for (var i = 1; i < lineRenderer.positionCount; i++)
            {
                Vector3 p = lineRenderer.GetPosition(i);
                totalLength += Vector3.Distance(p, lastPoint);
                lastPoint = p;
            }

            return totalLength;
        }

        public static void InsertPosition(LineRenderer lineRenderer, int index, Vector3 position)
        {
            if (positionBuffer.Length < lineRenderer.positionCount + 1)
            {
                Array.Resize(ref positionBuffer, lineRenderer.positionCount + 1);
            }

            lineRenderer.GetPositions(positionBuffer);
            Array.Copy(positionBuffer, index, positionBuffer, index + 1, lineRenderer.positionCount - index);
            positionBuffer[index] = position;
            lineRenderer.positionCount++;
            lineRenderer.SetPositions(positionBuffer);
        }
    }
}
