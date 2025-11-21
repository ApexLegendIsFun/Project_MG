using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TurnBasedCombat.UI
{
    /// <summary>
    /// Displays character health, MP, and status in combat
    /// </summary>
    public class CombatHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TurnBasedCombat.Core.CombatCharacter character;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Slider mpSlider;
        [SerializeField] private TextMeshProUGUI mpText;
        [SerializeField] private Image characterPortrait;
        [SerializeField] private GameObject turnIndicator;

        [Header("Colors")]
        [SerializeField] private Color healthyColor = Color.green;
        [SerializeField] private Color damagedColor = Color.yellow;
        [SerializeField] private Color criticalColor = Color.red;

        private Image hpFillImage;
        private Image mpFillImage;

        private void Awake()
        {
            if (hpSlider != null)
                hpFillImage = hpSlider.fillRect.GetComponent<Image>();

            if (mpSlider != null)
                mpFillImage = mpSlider.fillRect.GetComponent<Image>();

            if (turnIndicator != null)
                turnIndicator.SetActive(false);
        }

        /// <summary>
        /// Set the character for this HUD
        /// </summary>
        public void SetCharacter(TurnBasedCombat.Core.CombatCharacter newCharacter)
        {
            // Unsubscribe from old character
            if (character != null)
            {
                character.Stats.OnHPChanged -= UpdateHP;
                character.Stats.OnMPChanged -= UpdateMP;
            }

            character = newCharacter;

            // Subscribe to new character
            if (character != null)
            {
                character.Stats.OnHPChanged += UpdateHP;
                character.Stats.OnMPChanged += UpdateMP;

                // Initial update
                if (characterNameText != null)
                    characterNameText.text = character.CharacterName;

                UpdateHP(character.Stats.CurrentHP, character.Stats.MaxHP);
                UpdateMP(character.Stats.CurrentMP, character.Stats.MaxMP);
            }
        }

        /// <summary>
        /// Update HP display
        /// </summary>
        private void UpdateHP(int current, int max)
        {
            if (hpSlider != null)
            {
                hpSlider.maxValue = max;
                hpSlider.value = current;

                // Change color based on health percentage
                float healthPercent = (float)current / max;
                if (hpFillImage != null)
                {
                    if (healthPercent > 0.5f)
                        hpFillImage.color = healthyColor;
                    else if (healthPercent > 0.25f)
                        hpFillImage.color = damagedColor;
                    else
                        hpFillImage.color = criticalColor;
                }
            }

            if (hpText != null)
            {
                hpText.text = $"{current}/{max}";
            }
        }

        /// <summary>
        /// Update MP display
        /// </summary>
        private void UpdateMP(int current, int max)
        {
            if (mpSlider != null)
            {
                mpSlider.maxValue = max;
                mpSlider.value = current;
            }

            if (mpText != null)
            {
                mpText.text = $"{current}/{max}";
            }
        }

        /// <summary>
        /// Show/hide turn indicator
        /// </summary>
        public void SetTurnActive(bool active)
        {
            if (turnIndicator != null)
            {
                turnIndicator.SetActive(active);
            }
        }

        /// <summary>
        /// Set character portrait
        /// </summary>
        public void SetPortrait(Sprite sprite)
        {
            if (characterPortrait != null)
            {
                characterPortrait.sprite = sprite;
            }
        }

        private void OnDestroy()
        {
            if (character != null)
            {
                character.Stats.OnHPChanged -= UpdateHP;
                character.Stats.OnMPChanged -= UpdateMP;
            }
        }
    }
}
