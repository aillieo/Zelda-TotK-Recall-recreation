// -----------------------------------------------------------------------
// <copyright file="RenderingLayerMaskAttributeDrawer.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game.Rendering.Editor
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;

    [CustomPropertyDrawer(typeof(RenderingLayerMaskAttribute))]
    internal class RenderingLayerMaskAttributeDrawer : PropertyDrawer
    {
        private static string[] defaultRenderingLayerNamesValue;

        private static string[] defaultRenderingLayerNames
        {
            get
            {
                if (defaultRenderingLayerNamesValue == null)
                {
                    defaultRenderingLayerNamesValue = Enumerable.Range(1, 32).Select(i => $"Layer{i}").ToArray();
                }

                return defaultRenderingLayerNamesValue;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var renderPipelineAsset = GraphicsSettings.renderPipelineAsset;
            if (renderPipelineAsset == null)
            {
                EditorGUI.LabelField(position, $"No active {nameof(RenderPipelineAsset)} found.");
            }
            else
            {
                var displayedOptions = renderPipelineAsset.renderingLayerMaskNames;
                if (displayedOptions == null)
                {
                    displayedOptions = defaultRenderingLayerNames;
                }

                property.intValue = EditorGUI.MaskField(position, label, property.intValue, displayedOptions);
            }
        }
    }
}
