using System.Collections;
using UnityEngine;

namespace TurnBasedCombat.Effects
{
    /// <summary>
    /// Controls combat visual effects (particles, sprites, etc.)
    /// </summary>
    public class CombatEffect : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        [Header("Settings")]
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private bool destroyOnComplete = true;
        [SerializeField] private bool autoPlay = true;

        private void Awake()
        {
            if (particleSystem == null)
                particleSystem = GetComponent<ParticleSystem>();

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (animator == null)
                animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (autoPlay)
            {
                Play();
            }
        }

        /// <summary>
        /// Play the effect
        /// </summary>
        public void Play()
        {
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            if (animator != null)
            {
                animator.SetTrigger("Play");
            }

            if (destroyOnComplete)
            {
                StartCoroutine(DestroyAfterDelay());
            }
        }

        /// <summary>
        /// Stop the effect
        /// </summary>
        public void Stop()
        {
            if (particleSystem != null)
            {
                particleSystem.Stop();
            }
        }

        /// <summary>
        /// Set the effect color
        /// </summary>
        public void SetColor(Color color)
        {
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = color;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }

        /// <summary>
        /// Set the effect scale
        /// </summary>
        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// Destroy after delay
        /// </summary>
        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(lifetime);

            // Wait for particles to finish
            if (particleSystem != null)
            {
                while (particleSystem.IsAlive())
                {
                    yield return null;
                }
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Create an effect at a position
        /// </summary>
        public static CombatEffect Create(GameObject prefab, Vector3 position, Quaternion rotation = default)
        {
            if (prefab == null)
                return null;

            if (rotation == default)
                rotation = Quaternion.identity;

            GameObject effectObj = Instantiate(prefab, position, rotation);
            return effectObj.GetComponent<CombatEffect>();
        }

        /// <summary>
        /// Create an effect at a position with color
        /// </summary>
        public static CombatEffect Create(GameObject prefab, Vector3 position, Color color, Quaternion rotation = default)
        {
            var effect = Create(prefab, position, rotation);
            if (effect != null)
            {
                effect.SetColor(color);
            }
            return effect;
        }
    }
}
