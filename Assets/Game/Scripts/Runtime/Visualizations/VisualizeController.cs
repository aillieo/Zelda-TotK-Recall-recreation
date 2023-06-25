namespace AillieoTech.Game.Views
{
    using System.Collections.Generic;
    using AillieoTech.Game.Rendering;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class VisualizeController : MonoBehaviour
    {
        private readonly Dictionary<Recallable, Trail> trailInstances = new Dictionary<Recallable, Trail>();

        [SerializeField]
        private Trail trailPrefab;

        private RecallAbility abilityInstance;

        private Recallable lastTargtPreview;

        private void OnEnable()
        {
            RecallRendererSwitch.Instance.enableScanning = false;
            RecallRendererSwitch.Instance.enableOutline = false;
            RecallRendererSwitch.Instance.enableFading = false;

            RecallManager.Instance.OnPreviewBegin += this.OnPreviewBegin;
            RecallManager.Instance.OnPreviewTargetUpdate += this.OnPreviewTargetUpdate;
            RecallManager.Instance.OnPreviewEnd += this.OnPreviewEnd;
            RecallManager.Instance.OnAbilityBegin += this.OnAbilityBegin;
            RecallManager.Instance.OnAbilityEnd += this.OnAbilityEnd;
        }

        private void OnDisable()
        {
            RecallRendererSwitch.Instance.enableScanning = false;
            RecallRendererSwitch.Instance.enableOutline = false;
            RecallRendererSwitch.Instance.enableFading = false;

            RecallManager.Instance.OnPreviewBegin -= this.OnPreviewBegin;
            RecallManager.Instance.OnPreviewTargetUpdate -= this.OnPreviewTargetUpdate;
            RecallManager.Instance.OnPreviewEnd -= this.OnPreviewEnd;
            RecallManager.Instance.OnAbilityBegin -= this.OnAbilityBegin;
            RecallManager.Instance.OnAbilityEnd -= this.OnAbilityEnd;
        }

        private void OnPreviewBegin(IEnumerable<Recallable> recallables)
        {
            RecallRendererSwitch.Instance.enableScanning = true;
            RecallRendererSwitch.Instance.enableOutline = true;

            foreach (var r in recallables)
            {
                Trail trail = Instantiate<Trail>(this.trailPrefab);
                trail.LoadData(r, false);
                this.trailInstances[r] = trail;

                this.AddRenderingLayerMask(r.gameObject, Consts.outlineMask);
                this.AddRenderingLayerMask(r.gameObject, Consts.highlightMask);
            }
        }

        private void OnPreviewTargetUpdate(Recallable recallable)
        {
            this.OnEndBeingPotentialTarget(this.lastTargtPreview);
            this.OnBecomePotentialTarget(recallable);
            this.lastTargtPreview = recallable;
        }

        private void OnPreviewEnd()
        {
            this.OnEndBeingPotentialTarget(this.lastTargtPreview);
            this.lastTargtPreview = null;

            RecallRendererSwitch.Instance.enableScanning = false;
            RecallRendererSwitch.Instance.enableOutline = false;

            foreach (var pair in this.trailInstances)
            {
                Trail trail = pair.Value;
                trail.LoadData(null, false);
                GameObject.Destroy(trail.gameObject);

                this.RemoveRenderingLayerMask(pair.Key.gameObject, Consts.outlineMask);
                this.RemoveRenderingLayerMask(pair.Key.gameObject, Consts.highlightMask);
            }

            this.trailInstances.Clear();
        }

        private void OnAbilityBegin(RecallAbility ability)
        {
            Assert.IsNull(this.abilityInstance);
            this.abilityInstance = ability;

            RecallRendererSwitch.Instance.enableOutline = true;
            RecallRendererSwitch.Instance.enableFading = true;

            if (!this.trailInstances.TryGetValue(ability.recallable, out Trail trail))
            {
                trail = Instantiate<Trail>(this.trailPrefab);
                trail.LoadData(ability.recallable, true);
                this.trailInstances[ability.recallable] = trail;
            }

            this.AddRenderingLayerMask(ability.recallable.gameObject, Consts.outlineMask);
        }

        private void OnAbilityEnd(RecallAbility ability)
        {
            Assert.AreEqual(this.abilityInstance, ability);
            this.abilityInstance = null;

            RecallRendererSwitch.Instance.enableOutline = false;
            RecallRendererSwitch.Instance.enableFading = false;

            if (this.trailInstances.TryGetValue(ability.recallable, out Trail trail))
            {
                trail.LoadData(null, false);
                GameObject.Destroy(trail.gameObject);
                this.trailInstances.Remove(ability.recallable);
            }

            this.RemoveRenderingLayerMask(ability.recallable.gameObject, Consts.outlineMask);
        }

        private void Update()
        {
            if (RecallManager.Instance.stage == AbilityStage.Casting)
            {
                if (this.trailInstances.TryGetValue(this.abilityInstance.recallable, out Trail trail))
                {
                    trail.Consume(this.abilityInstance.frameCount);
                }
            }
        }

        private void OnBecomePotentialTarget(Recallable recallable)
        {
            if (recallable != null)
            {
                if (this.trailInstances.TryGetValue(recallable, out Trail trail))
                {
                    trail.LoadData(recallable, true);
                }

                this.RemoveRenderingLayerMask(recallable.gameObject, Consts.highlightMask);
            }
        }

        private void OnEndBeingPotentialTarget(Recallable recallable)
        {
            if (recallable != null)
            {
                if (this.trailInstances.TryGetValue(recallable, out Trail trail))
                {
                    trail.LoadData(recallable, false);
                }

                this.RemoveRenderingLayerMask(recallable.gameObject, Consts.highlightMask);
            }
        }

        private void AddRenderingLayerMask(GameObject go, int renderingLayerMask)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                r.renderingLayerMask |= (uint)(1 << renderingLayerMask);
            }
        }

        private void RemoveRenderingLayerMask(GameObject go, int renderingLayerMask)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                r.renderingLayerMask &= (uint)(~(1 << renderingLayerMask));
            }
        }
    }
}
