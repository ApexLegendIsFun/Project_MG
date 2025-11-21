using UnityEngine;

namespace TurnBasedCombat.Core
{
    /// <summary>
    /// Types of status effects
    /// </summary>
    public enum StatusEffectType
    {
        Poison,      // Damage over time
        Burn,        // Damage over time
        Regen,       // Heal over time
        AttackUp,    // Increases attack
        AttackDown,  // Decreases attack
        DefenseUp,   // Increases defense
        DefenseDown, // Decreases defense
        Stun,        // Cannot act
        Slow,        // Reduces speed
        Haste        // Increases speed
    }

    /// <summary>
    /// Represents a status effect that can be applied to characters
    /// </summary>
    [System.Serializable]
    public class StatusEffect
    {
        [SerializeField] private string effectName;
        [SerializeField] private StatusEffectType effectType;
        [SerializeField] private int duration; // Turns remaining
        [SerializeField] private int power;    // Strength of the effect

        private int turnsRemaining;
        private bool isActive = false;

        public string EffectName => effectName;
        public StatusEffectType EffectType => effectType;
        public int TurnsRemaining => turnsRemaining;
        public bool IsExpired => turnsRemaining <= 0;
        public bool IsActive => isActive;

        /// <summary>
        /// Constructor
        /// </summary>
        public StatusEffect(string name, StatusEffectType type, int duration, int power)
        {
            this.effectName = name;
            this.effectType = type;
            this.duration = duration;
            this.power = power;
            this.turnsRemaining = duration;
        }

        /// <summary>
        /// Apply the status effect to a character
        /// </summary>
        public void Apply(CombatCharacter character)
        {
            isActive = true;

            switch (effectType)
            {
                case StatusEffectType.AttackUp:
                    character.Stats.ModifyStat(StatType.Attack, power);
                    break;
                case StatusEffectType.AttackDown:
                    character.Stats.ModifyStat(StatType.Attack, -power);
                    break;
                case StatusEffectType.DefenseUp:
                    character.Stats.ModifyStat(StatType.Defense, power);
                    break;
                case StatusEffectType.DefenseDown:
                    character.Stats.ModifyStat(StatType.Defense, -power);
                    break;
                case StatusEffectType.Haste:
                    character.Stats.ModifyStat(StatType.Speed, power);
                    break;
                case StatusEffectType.Slow:
                    character.Stats.ModifyStat(StatType.Speed, -power);
                    break;
            }
        }

        /// <summary>
        /// Process the effect each turn
        /// </summary>
        public void Process(CombatCharacter character)
        {
            if (!isActive || IsExpired)
                return;

            switch (effectType)
            {
                case StatusEffectType.Poison:
                case StatusEffectType.Burn:
                    character.TakeDamage(power);
                    Debug.Log($"{character.CharacterName} takes {power} {effectType} damage!");
                    break;

                case StatusEffectType.Regen:
                    character.Heal(power);
                    Debug.Log($"{character.CharacterName} regenerates {power} HP!");
                    break;

                case StatusEffectType.Stun:
                    Debug.Log($"{character.CharacterName} is stunned!");
                    break;
            }

            turnsRemaining--;
        }

        /// <summary>
        /// Remove the status effect from a character
        /// </summary>
        public void Remove(CombatCharacter character)
        {
            if (!isActive)
                return;

            isActive = false;

            // Reverse stat modifications
            switch (effectType)
            {
                case StatusEffectType.AttackUp:
                    character.Stats.ModifyStat(StatType.Attack, -power);
                    break;
                case StatusEffectType.AttackDown:
                    character.Stats.ModifyStat(StatType.Attack, power);
                    break;
                case StatusEffectType.DefenseUp:
                    character.Stats.ModifyStat(StatType.Defense, -power);
                    break;
                case StatusEffectType.DefenseDown:
                    character.Stats.ModifyStat(StatType.Defense, power);
                    break;
                case StatusEffectType.Haste:
                    character.Stats.ModifyStat(StatType.Speed, -power);
                    break;
                case StatusEffectType.Slow:
                    character.Stats.ModifyStat(StatType.Speed, power);
                    break;
            }

            Debug.Log($"{character.CharacterName}'s {effectName} has worn off!");
        }

        /// <summary>
        /// Get a description of this effect
        /// </summary>
        public string GetDescription()
        {
            return $"{effectName}: {GetEffectDescription()} ({turnsRemaining} turns)";
        }

        private string GetEffectDescription()
        {
            switch (effectType)
            {
                case StatusEffectType.Poison:
                    return $"Takes {power} poison damage per turn";
                case StatusEffectType.Burn:
                    return $"Takes {power} burn damage per turn";
                case StatusEffectType.Regen:
                    return $"Heals {power} HP per turn";
                case StatusEffectType.AttackUp:
                    return $"Attack increased by {power}";
                case StatusEffectType.AttackDown:
                    return $"Attack decreased by {power}";
                case StatusEffectType.DefenseUp:
                    return $"Defense increased by {power}";
                case StatusEffectType.DefenseDown:
                    return $"Defense decreased by {power}";
                case StatusEffectType.Stun:
                    return "Cannot act";
                case StatusEffectType.Slow:
                    return $"Speed decreased by {power}";
                case StatusEffectType.Haste:
                    return $"Speed increased by {power}";
                default:
                    return "Unknown effect";
            }
        }
    }
}
