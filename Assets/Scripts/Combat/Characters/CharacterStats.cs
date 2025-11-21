using System;
using UnityEngine;

namespace TurnBasedCombat.Core
{
    /// <summary>
    /// Container for character statistics
    /// </summary>
    [System.Serializable]
    public class CharacterStats
    {
        [Header("Base Stats")]
        [SerializeField] private int maxHP = 100;
        [SerializeField] private int maxMP = 50;
        [SerializeField] private int attack = 10;
        [SerializeField] private int defense = 5;
        [SerializeField] private int speed = 10;

        [Header("Current Values")]
        [SerializeField] private int currentHP;
        [SerializeField] private int currentMP;

        /// <summary>
        /// Constructor for creating stats with custom values
        /// </summary>
        public CharacterStats(int hp, int mp, int atk, int def, int spd)
        {
            maxHP = hp;
            maxMP = mp;
            attack = atk;
            defense = def;
            speed = spd;
            currentHP = hp;
            currentMP = mp;
        }

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public CharacterStats()
        {
        }

        // Events
        public event Action<int, int> OnHPChanged;  // current, max
        public event Action<int, int> OnMPChanged;  // current, max
        public event Action OnDeath;

        // Properties
        public int MaxHP => maxHP;
        public int MaxMP => maxMP;
        public int Attack => attack;
        public int Defense => defense;
        public int Speed => speed;
        public int CurrentHP => currentHP;
        public int CurrentMP => currentMP;
        public bool IsAlive => currentHP > 0;

        /// <summary>
        /// Initialize stats to max values
        /// </summary>
        public void Initialize()
        {
            currentHP = maxHP;
            currentMP = maxMP;
            OnHPChanged?.Invoke(currentHP, maxHP);
            OnMPChanged?.Invoke(currentMP, maxMP);
        }

        /// <summary>
        /// Initialize with specific stat values (for runtime creation)
        /// </summary>
        public void InitializeWithValues(int hp, int mp, int atk, int def, int spd)
        {
            maxHP = hp;
            maxMP = mp;
            attack = atk;
            defense = def;
            speed = spd;
            currentHP = hp;
            currentMP = mp;
            OnHPChanged?.Invoke(currentHP, maxHP);
            OnMPChanged?.Invoke(currentMP, maxMP);
        }

        /// <summary>
        /// Take damage
        /// </summary>
        public int TakeDamage(int damage)
        {
            int actualDamage = Mathf.Max(0, damage - defense);
            currentHP = Mathf.Max(0, currentHP - actualDamage);

            OnHPChanged?.Invoke(currentHP, maxHP);

            if (currentHP <= 0)
            {
                OnDeath?.Invoke();
            }

            return actualDamage;
        }

        /// <summary>
        /// Heal HP
        /// </summary>
        public int Heal(int amount)
        {
            int previousHP = currentHP;
            currentHP = Mathf.Min(maxHP, currentHP + amount);

            int actualHeal = currentHP - previousHP;
            OnHPChanged?.Invoke(currentHP, maxHP);

            return actualHeal;
        }

        /// <summary>
        /// Restore MP
        /// </summary>
        public int RestoreMP(int amount)
        {
            int previousMP = currentMP;
            currentMP = Mathf.Min(maxMP, currentMP + amount);

            int actualRestore = currentMP - previousMP;
            OnMPChanged?.Invoke(currentMP, maxMP);

            return actualRestore;
        }

        /// <summary>
        /// Consume MP
        /// </summary>
        public bool ConsumeMP(int amount)
        {
            if (currentMP < amount)
                return false;

            currentMP -= amount;
            OnMPChanged?.Invoke(currentMP, maxMP);
            return true;
        }

        /// <summary>
        /// Modify stats temporarily or permanently
        /// </summary>
        public void ModifyStat(StatType statType, int amount, bool permanent = false)
        {
            switch (statType)
            {
                case StatType.MaxHP:
                    if (permanent) maxHP += amount;
                    break;
                case StatType.MaxMP:
                    if (permanent) maxMP += amount;
                    break;
                case StatType.Attack:
                    attack += amount;
                    break;
                case StatType.Defense:
                    defense += amount;
                    break;
                case StatType.Speed:
                    speed += amount;
                    break;
            }
        }

        /// <summary>
        /// Create a copy of these stats
        /// </summary>
        public CharacterStats Clone()
        {
            CharacterStats clone = new CharacterStats();
            clone.maxHP = this.maxHP;
            clone.maxMP = this.maxMP;
            clone.attack = this.attack;
            clone.defense = this.defense;
            clone.speed = this.speed;
            clone.currentHP = this.currentHP;
            clone.currentMP = this.currentMP;
            return clone;
        }
    }

    /// <summary>
    /// Types of stats that can be modified
    /// </summary>
    public enum StatType
    {
        MaxHP,
        MaxMP,
        Attack,
        Defense,
        Speed
    }
}
