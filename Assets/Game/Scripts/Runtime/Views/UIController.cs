namespace AillieoTech.Game.Views
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform))]
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject cursor;

        [SerializeField]
        private GameObject progressNode;

        [SerializeField]
        private Image progressImage;

        private void OnEnable()
        {
            RecallManager.Instance.OnAbilityBegin += this.OnAbilityBegin;
            RecallManager.Instance.OnAbilityUpdate += this.OnAbilityUpdate;
            RecallManager.Instance.OnAbilityEnd += this.OnAbilityEnd;
        }

        private void OnDisable()
        {
            RecallManager.Instance.OnAbilityBegin -= this.OnAbilityBegin;
            RecallManager.Instance.OnAbilityUpdate -= this.OnAbilityUpdate;
            RecallManager.Instance.OnAbilityEnd -= this.OnAbilityEnd;
        }

        private void OnAbilityBegin(RecallAbility ability)
        {
            this.progressNode.SetActive(true);
        }

        private void OnAbilityEnd(RecallAbility ability)
        {
            this.progressNode.SetActive(false);
        }

        private void OnAbilityUpdate(RecallAbility ability)
        {
            var max = RecallManager.maxFrameCount;
            var frame = ability.frameCount;
            var proportion = (float)frame / max;
            this.progressImage.fillAmount = proportion;
        }
    }
}
