using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TurnBasedCombat.Core
{
    /// <summary>
    /// Base class for all combat characters (players and enemies)
    /// </summary>
    public class CombatCharacter : MonoBehaviour
    {
        [Header("Character Info")]
        [SerializeField] private string characterName = "Character";
        [SerializeField] private bool isPlayer = true;

        [Header("Stats")]
        [SerializeField] private CharacterStats stats = new CharacterStats();

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        [Header("Combat Position")]
        [SerializeField] private Transform combatPosition;

        // Events
        public UnityEvent<int> OnDamageDealt;
        public UnityEvent<int> OnDamageTaken;
        public UnityEvent<int> OnHealed;
        public UnityEvent OnDeath;
        public UnityEvent OnActionComplete;

        // Status Effects
        private List<StatusEffect> activeEffects = new List<StatusEffect>();

        // Properties
        public string CharacterName => characterName;
        public bool IsPlayer => isPlayer;
        public CharacterStats Stats => stats;
        public bool IsAlive => stats.IsAlive;
        public int Speed => stats.Speed;
        public Vector3 Position => combatPosition != null ? combatPosition.position : transform.position;

        private void Awake()
        {
            if (OnDamageDealt == null) OnDamageDealt = new UnityEvent<int>();
            if (OnDamageTaken == null) OnDamageTaken = new UnityEvent<int>();
            if (OnHealed == null) OnHealed = new UnityEvent<int>();
            if (OnDeath == null) OnDeath = new UnityEvent();
            if (OnActionComplete == null) OnActionComplete = new UnityEvent();

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (animator == null)
                animator = GetComponent<Animator>();

            stats.Initialize();
            stats.OnDeath += HandleDeath;
        }

        private void OnDestroy()
        {
            stats.OnDeath -= HandleDeath;
        }

        /// <summary>
        /// Initialize character with custom values (for runtime creation)
        /// </summary>
        public void InitializeCharacter(string name, bool player, int hp, int mp, int atk, int def, int spd)
        {
            characterName = name;
            isPlayer = player;
            stats.InitializeWithValues(hp, mp, atk, def, spd);
        }

        /// <summary>
        /// Set sprite renderer reference
        /// </summary>
        public void SetSpriteRenderer(SpriteRenderer renderer)
        {
            spriteRenderer = renderer;
        }

        /// <summary>
        /// Set combat position transform
        /// </summary>
        public void SetCombatPosition(Transform position)
        {
            combatPosition = position;
        }

        #region Combat Actions

        /// <summary>
        /// Perform basic attack on target
        /// </summary>
        public IEnumerator Attack(CombatCharacter target)
        {
            if (!IsAlive || target == null || !target.IsAlive)
                yield break;

            // Play attack animation
            PlayAnimation("Attack");
            yield return new WaitForSeconds(0.3f);

            // Calculate damage
            int damage = CalculateDamage(stats.Attack);

            // Apply damage to target
            int actualDamage = target.TakeDamage(damage);

            OnDamageDealt?.Invoke(actualDamage);

            yield return new WaitForSeconds(0.3f);

            // Return to idle
            PlayAnimation("Idle");

            OnActionComplete?.Invoke();
        }

        /// <summary>
        /// Take damage from an attack
        /// </summary>
        public int TakeDamage(int damage)
        {
            if (!IsAlive)
                return 0;

            // Apply damage to stats
            int actualDamage = stats.TakeDamage(damage);

            // Play hit animation
            PlayAnimation("Hit");
            StartCoroutine(FlashSprite());

            OnDamageTaken?.Invoke(actualDamage);

            Debug.Log($"{characterName} took {actualDamage} damage! HP: {stats.CurrentHP}/{stats.MaxHP}");

            return actualDamage;
        }

        /// <summary>
        /// Heal this character
        /// </summary>
        public int Heal(int amount)
        {
            if (!IsAlive)
                return 0;

            int actualHeal = stats.Heal(amount);
            OnHealed?.Invoke(actualHeal);

            Debug.Log($"{characterName} healed {actualHeal} HP! HP: {stats.CurrentHP}/{stats.MaxHP}");

            return actualHeal;
        }

        /// <summary>
        /// Defend action - reduces incoming damage next turn
        /// </summary>
        public IEnumerator Defend()
        {
            PlayAnimation("Defend");

            // Add defense buff
            var defendEffect = new StatusEffect(
                "Defend",
                StatusEffectType.DefenseUp,
                1,
                stats.Defense / 2
            );
            AddStatusEffect(defendEffect);

            yield return new WaitForSeconds(0.5f);

            OnActionComplete?.Invoke();
        }

        #endregion

        #region Status Effects

        /// <summary>
        /// Add a status effect to this character
        /// </summary>
        public void AddStatusEffect(StatusEffect effect)
        {
            activeEffects.Add(effect);
            effect.Apply(this);

            Debug.Log($"{characterName} gained status: {effect.EffectName}");
        }

        /// <summary>
        /// Remove a status effect
        /// </summary>
        public void RemoveStatusEffect(StatusEffect effect)
        {
            if (activeEffects.Contains(effect))
            {
                effect.Remove(this);
                activeEffects.Remove(effect);
            }
        }

        /// <summary>
        /// Process all active status effects
        /// </summary>
        public void ProcessStatusEffects()
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                var effect = activeEffects[i];
                effect.Process(this);

                if (effect.IsExpired)
                {
                    RemoveStatusEffect(effect);
                }
            }
        }

        /// <summary>
        /// Get all active status effects
        /// </summary>
        public List<StatusEffect> GetActiveEffects()
        {
            return new List<StatusEffect>(activeEffects);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Calculate damage with variance
        /// </summary>
        private int CalculateDamage(int baseAttack)
        {
            float variance = Random.Range(0.9f, 1.1f);
            return Mathf.RoundToInt(baseAttack * variance);
        }

        /// <summary>
        /// Handle death
        /// </summary>
        private void HandleDeath()
        {
            PlayAnimation("Death");
            OnDeath?.Invoke();

            Debug.Log($"{characterName} has been defeated!");

            // Fade out sprite
            StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Play animation if animator exists
        /// </summary>
        private void PlayAnimation(string animationName)
        {
            if (animator != null)
            {
                animator.SetTrigger(animationName);
            }
        }

        /// <summary>
        /// Flash sprite when taking damage
        /// </summary>
        private IEnumerator FlashSprite()
        {
            if (spriteRenderer == null)
                yield break;

            Color originalColor = spriteRenderer.color;

            for (int i = 0; i < 3; i++)
            {
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.05f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.05f);
            }
        }

        /// <summary>
        /// Fade out sprite on death
        /// </summary>
        private IEnumerator FadeOut()
        {
            if (spriteRenderer == null)
                yield break;

            Color color = spriteRenderer.color;
            float fadeSpeed = 1f;

            while (color.a > 0)
            {
                color.a -= Time.deltaTime * fadeSpeed;
                spriteRenderer.color = color;
                yield return null;
            }
        }

        #endregion

        #region Editor Helpers

        private void OnDrawGizmosSelected()
        {
            if (combatPosition != null)
            {
                Gizmos.color = isPlayer ? Color.blue : Color.red;
                Gizmos.DrawWireSphere(combatPosition.position, 0.5f);
            }
        }

        #endregion
    }
}
