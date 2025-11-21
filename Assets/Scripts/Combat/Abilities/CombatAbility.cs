using System.Collections;
using UnityEngine;

namespace TurnBasedCombat.Core
{
    /// <summary>
    /// Defines a combat ability/skill
    /// </summary>
    [CreateAssetMenu(fileName = "NewAbility", menuName = "Combat/Ability")]
    public class CombatAbility : ScriptableObject
    {
        [Header("Ability Info")]
        [SerializeField] private string abilityName = "New Ability";
        [TextArea(2, 4)]
        [SerializeField] private string description = "Ability description";
        [SerializeField] private Sprite icon;

        [Header("Costs")]
        [SerializeField] private int mpCost = 10;
        [SerializeField] private int cooldown = 0;

        [Header("Targeting")]
        [SerializeField] private TargetType targetType = TargetType.SingleEnemy;

        [Header("Effects")]
        [SerializeField] private AbilityEffectType effectType = AbilityEffectType.Damage;
        [SerializeField] private int basePower = 20;
        [SerializeField] private float powerMultiplier = 1.5f;

        [Header("Status Effects")]
        [SerializeField] private bool appliesStatusEffect = false;
        [SerializeField] private StatusEffectType statusEffectType;
        [SerializeField] private int statusEffectDuration = 3;
        [SerializeField] private int statusEffectPower = 5;
        [SerializeField] [Range(0f, 1f)] private float statusEffectChance = 0.5f;

        [Header("Visuals")]
        [SerializeField] private GameObject effectPrefab;
        [SerializeField] private Color effectColor = Color.white;

        // Properties
        public string AbilityName => abilityName;
        public string Description => description;
        public Sprite Icon => icon;
        public int MPCost => mpCost;
        public int Cooldown => cooldown;
        public TargetType TargetingType => targetType;

        /// <summary>
        /// Execute the ability
        /// </summary>
        public IEnumerator Execute(CombatCharacter user, CombatCharacter target)
        {
            if (!CanUse(user))
            {
                Debug.Log($"Cannot use {abilityName}!");
                yield break;
            }

            // Consume MP
            user.Stats.ConsumeMP(mpCost);

            // Visual effect
            if (effectPrefab != null)
            {
                var effect = Instantiate(effectPrefab, target.Position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            yield return new WaitForSeconds(0.3f);

            // Apply effect
            ApplyEffect(user, target);

            yield return new WaitForSeconds(0.3f);
        }

        /// <summary>
        /// Check if the ability can be used
        /// </summary>
        public bool CanUse(CombatCharacter user)
        {
            if (user.Stats.CurrentMP < mpCost)
            {
                Debug.Log("Not enough MP!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply the ability's effect
        /// </summary>
        private void ApplyEffect(CombatCharacter user, CombatCharacter target)
        {
            int power = CalculatePower(user);

            switch (effectType)
            {
                case AbilityEffectType.Damage:
                    target.TakeDamage(power);
                    Debug.Log($"{user.CharacterName} used {abilityName} on {target.CharacterName} for {power} damage!");
                    break;

                case AbilityEffectType.Heal:
                    target.Heal(power);
                    Debug.Log($"{user.CharacterName} used {abilityName} on {target.CharacterName} for {power} healing!");
                    break;

                case AbilityEffectType.Buff:
                    ApplyStatusEffect(target);
                    Debug.Log($"{user.CharacterName} used {abilityName} on {target.CharacterName}!");
                    break;

                case AbilityEffectType.Debuff:
                    ApplyStatusEffect(target);
                    Debug.Log($"{user.CharacterName} used {abilityName} on {target.CharacterName}!");
                    break;
            }

            // Chance to apply additional status effect
            if (appliesStatusEffect && Random.value <= statusEffectChance)
            {
                ApplyStatusEffect(target);
            }
        }

        /// <summary>
        /// Calculate ability power
        /// </summary>
        private int CalculatePower(CombatCharacter user)
        {
            int power = basePower;

            // Scale with user's attack stat
            if (effectType == AbilityEffectType.Damage)
            {
                power = Mathf.RoundToInt(user.Stats.Attack * powerMultiplier);
            }

            return power;
        }

        /// <summary>
        /// Apply status effect to target
        /// </summary>
        private void ApplyStatusEffect(CombatCharacter target)
        {
            if (!appliesStatusEffect)
                return;

            var effect = new StatusEffect(
                abilityName,
                statusEffectType,
                statusEffectDuration,
                statusEffectPower
            );

            target.AddStatusEffect(effect);
        }

        /// <summary>
        /// Get full description with stats
        /// </summary>
        public string GetFullDescription()
        {
            string desc = $"{abilityName}\n{description}\n\n";
            desc += $"MP Cost: {mpCost}\n";
            desc += $"Power: {basePower}";

            if (appliesStatusEffect)
            {
                desc += $"\n{statusEffectChance * 100}% chance to apply {statusEffectType}";
            }

            return desc;
        }
    }

    /// <summary>
    /// Types of ability effects
    /// </summary>
    public enum AbilityEffectType
    {
        Damage,
        Heal,
        Buff,
        Debuff
    }

    /// <summary>
    /// Ability targeting types
    /// </summary>
    public enum TargetType
    {
        Self,
        SingleAlly,
        SingleEnemy,
        AllAllies,
        AllEnemies,
        All
    }
}
