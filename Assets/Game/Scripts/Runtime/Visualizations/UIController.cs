// -----------------------------------------------------------------------
// <copyright file="UIController.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game.Views
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform))]
    internal class UIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject cursor;

        [SerializeField]
        private RectTransform progressNode;

        [SerializeField]
        private Image progressImage;

        private RecallAbility abilityInstance;

        private Canvas canvasValue;

        private Canvas canvas
        {
            get
            {
                if (this.canvasValue == null)
                {
                    this.canvasValue = this.GetComponent<Canvas>();
                }

                return this.canvasValue;
            }
        }

        private void OnEnable()
        {
            RecallManager.Instance.OnPreviewBegin += this.OnPreviewBegin;
            RecallManager.Instance.OnPreviewTargetUpdate += this.OnPreviewTargetUpdate;
            RecallManager.Instance.OnPreviewEnd += this.OnPreviewEnd;
            RecallManager.Instance.OnAbilityBegin += this.OnAbilityBegin;
            RecallManager.Instance.OnAbilityEnd += this.OnAbilityEnd;

            this.progressNode.gameObject.SetActive(false);
            this.cursor.SetActive(false);
        }

        private void OnDisable()
        {
            RecallManager.Instance.OnPreviewBegin -= this.OnPreviewBegin;
            RecallManager.Instance.OnPreviewTargetUpdate -= this.OnPreviewTargetUpdate;
            RecallManager.Instance.OnPreviewEnd -= this.OnPreviewEnd;
            RecallManager.Instance.OnAbilityBegin -= this.OnAbilityBegin;
            RecallManager.Instance.OnAbilityEnd -= this.OnAbilityEnd;
        }

        private void OnPreviewBegin(IEnumerable<Recallable> recallables)
        {
            this.cursor.SetActive(true);
        }

        private void OnPreviewTargetUpdate(Recallable recallable)
        {
            Animator animator = this.cursor.GetComponent<Animator>();
            animator.SetBool("HasTarget", recallable != null);
        }

        private void OnPreviewEnd()
        {
            this.cursor.SetActive(false);
        }

        private void OnAbilityBegin(RecallAbility ability)
        {
            Assert.IsNull(this.abilityInstance);
            this.abilityInstance = ability;

            this.progressNode.gameObject.SetActive(true);
            this.progressImage.fillAmount = 0;
        }

        private void OnAbilityEnd(RecallAbility ability)
        {
            Assert.AreEqual(this.abilityInstance, ability);
            this.abilityInstance = null;

            this.progressNode.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (RecallManager.Instance.stage == AbilityStage.Casting)
            {
                var max = RecallManager.maxFrameCount;
                var frame = this.abilityInstance.frameCount;
                var proportion = 1f - ((float)frame / max);
                proportion = Mathf.Round(proportion * 60) / 60f;
                this.progressImage.fillAmount = proportion;

                Vector3 screenPoint = Utils.WorldToScreenPointSafe(Camera.main, this.abilityInstance.recallable.transform.position);
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    this.progressNode,
                    screenPoint,
                    this.canvas.worldCamera,
                    out Vector3 uiPos);
                this.progressNode.transform.position = uiPos;
            }
        }
    }
}
