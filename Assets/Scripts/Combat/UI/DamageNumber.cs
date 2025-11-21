using System.Collections;
using UnityEngine;
using TMPro;

namespace TurnBasedCombat.UI
{
    /// <summary>
    /// Displays floating damage/heal numbers
    /// </summary>
    public class DamageNumber : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float floatSpeed = 2f;
        [SerializeField] private float fadeSpeed = 1f;
        [SerializeField] private float lifetime = 1.5f;
        [SerializeField] private Vector3 randomOffset = new Vector3(0.5f, 0.5f, 0);

        [Header("Colors")]
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;
        [SerializeField] private Color criticalColor = new Color(1f, 0.5f, 0f);

        private TextMeshProUGUI textMesh;
        private CanvasGroup canvasGroup;
        private Vector3 floatDirection;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        /// <summary>
        /// Show damage number
        /// </summary>
        public void ShowDamage(int amount, bool isCritical = false)
        {
            if (textMesh != null)
            {
                textMesh.text = amount.ToString();
                textMesh.color = isCritical ? criticalColor : damageColor;

                if (isCritical)
                {
                    textMesh.fontSize *= 1.5f;
                }
            }

            StartFloating();
        }

        /// <summary>
        /// Show heal number
        /// </summary>
        public void ShowHeal(int amount)
        {
            if (textMesh != null)
            {
                textMesh.text = $"+{amount}";
                textMesh.color = healColor;
            }

            StartFloating();
        }

        /// <summary>
        /// Show custom text
        /// </summary>
        public void ShowText(string text, Color color)
        {
            if (textMesh != null)
            {
                textMesh.text = text;
                textMesh.color = color;
            }

            StartFloating();
        }

        /// <summary>
        /// Start floating animation
        /// </summary>
        private void StartFloating()
        {
            // Random offset
            Vector3 offset = new Vector3(
                Random.Range(-randomOffset.x, randomOffset.x),
                Random.Range(-randomOffset.y, randomOffset.y),
                0
            );

            transform.position += offset;
            floatDirection = Vector3.up;

            StartCoroutine(FloatAndFade());
        }

        /// <summary>
        /// Float up and fade out
        /// </summary>
        private IEnumerator FloatAndFade()
        {
            float elapsed = 0f;

            while (elapsed < lifetime)
            {
                // Float up
                transform.position += floatDirection * floatSpeed * Time.deltaTime;

                // Fade out
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f - (elapsed / lifetime);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
