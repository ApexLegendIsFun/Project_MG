using UnityEngine;
using System.Collections.Generic;

namespace TurnBasedCombat.Data
{
    /// <summary>
    /// Preset configuration for a complete battle scenario
    /// </summary>
    [CreateAssetMenu(fileName = "NewBattlePreset", menuName = "Combat/Battle Preset")]
    public class BattlePreset : ScriptableObject
    {
        [Header("Battle Info")]
        [SerializeField] private string battleName = "Test Battle";
        [TextArea(2, 4)]
        [SerializeField] private string description = "Battle description";

        [Header("Player Characters")]
        [SerializeField] private List<CharacterEntry> playerCharacters = new List<CharacterEntry>();

        [Header("Enemy Characters")]
        [SerializeField] private List<CharacterEntry> enemyCharacters = new List<CharacterEntry>();

        [Header("Battle Settings")]
        [SerializeField] private float turnDelay = 0.5f;
        [SerializeField] private bool autoStartBattle = true;

        [Header("Positioning")]
        [SerializeField] private FormationType playerFormation = FormationType.Line;
        [SerializeField] private FormationType enemyFormation = FormationType.Line;
        [SerializeField] private Vector3 playerAreaCenter = new Vector3(-3, 0, 0);
        [SerializeField] private Vector3 enemyAreaCenter = new Vector3(3, 0, 0);
        [SerializeField] private float formationSpacing = 1.5f;

        [Header("UI Settings")]
        [SerializeField] private bool generateUI = true;
        [SerializeField] private bool enableDamageNumbers = true;
        [SerializeField] private bool enableCameraShake = true;

        [Header("Scene Setup")]
        [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        [SerializeField] private bool create2DLighting = true;

        // Properties
        public string BattleName => battleName;
        public string Description => description;
        public List<CharacterEntry> PlayerCharacters => playerCharacters;
        public List<CharacterEntry> EnemyCharacters => enemyCharacters;
        public float TurnDelay => turnDelay;
        public bool AutoStartBattle => autoStartBattle;
        public FormationType PlayerFormation => playerFormation;
        public FormationType EnemyFormation => enemyFormation;
        public Vector3 PlayerAreaCenter => playerAreaCenter;
        public Vector3 EnemyAreaCenter => enemyAreaCenter;
        public float FormationSpacing => formationSpacing;
        public bool GenerateUI => generateUI;
        public bool EnableDamageNumbers => enableDamageNumbers;
        public bool EnableCameraShake => enableCameraShake;
        public Color BackgroundColor => backgroundColor;
        public bool Create2DLighting => create2DLighting;

        /// <summary>
        /// Get position for character based on formation
        /// </summary>
        public Vector3 GetCharacterPosition(bool isPlayer, int index, int totalCount)
        {
            Vector3 basePosition = isPlayer ? playerAreaCenter : enemyAreaCenter;
            FormationType formation = isPlayer ? playerFormation : enemyFormation;

            return CalculateFormationPosition(basePosition, formation, index, totalCount, formationSpacing);
        }

        /// <summary>
        /// Calculate position based on formation type
        /// </summary>
        private Vector3 CalculateFormationPosition(Vector3 center, FormationType formation, int index, int total, float spacing)
        {
            switch (formation)
            {
                case FormationType.Line:
                    // Vertical line
                    float offset = (total - 1) * spacing * 0.5f;
                    return center + new Vector3(0, -offset + (index * spacing), 0);

                case FormationType.HorizontalLine:
                    // Horizontal line
                    float hOffset = (total - 1) * spacing * 0.5f;
                    return center + new Vector3(-hOffset + (index * spacing), 0, 0);

                case FormationType.Grid:
                    // Grid formation (2 columns)
                    int row = index / 2;
                    int col = index % 2;
                    return center + new Vector3(col * spacing - spacing * 0.5f, -row * spacing, 0);

                case FormationType.V:
                    // V formation
                    int side = index % 2 == 0 ? 1 : -1;
                    int depth = index / 2;
                    return center + new Vector3(side * depth * spacing * 0.5f, -depth * spacing, 0);

                case FormationType.Circle:
                    // Circular formation
                    float angle = (360f / total) * index * Mathf.Deg2Rad;
                    float radius = spacing * 1.5f;
                    return center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);

                case FormationType.Random:
                    // Random within area
                    return center + new Vector3(
                        Random.Range(-spacing, spacing),
                        Random.Range(-spacing, spacing),
                        0
                    );

                default:
                    return center;
            }
        }

        /// <summary>
        /// Get total character count
        /// </summary>
        public int GetTotalCharacters()
        {
            return playerCharacters.Count + enemyCharacters.Count;
        }

        /// <summary>
        /// Validate preset
        /// </summary>
        public bool IsValid()
        {
            return playerCharacters.Count > 0 && enemyCharacters.Count > 0;
        }
    }

    /// <summary>
    /// Entry for a character in a battle preset
    /// </summary>
    [System.Serializable]
    public class CharacterEntry
    {
        [SerializeField] private CharacterTemplate template;
        [SerializeField] private string customName = "";
        [SerializeField] private bool overrideStats = false;

        [Header("Stat Overrides")]
        [SerializeField] private int hpOverride = 100;
        [SerializeField] private int mpOverride = 50;
        [SerializeField] private int attackOverride = 15;
        [SerializeField] private int defenseOverride = 8;
        [SerializeField] private int speedOverride = 12;

        public CharacterTemplate Template => template;
        public string CustomName => string.IsNullOrEmpty(customName) ? template?.CharacterName : customName;
        public bool OverrideStats => overrideStats;

        public CharacterStatsData GetStats()
        {
            if (overrideStats)
            {
                return new CharacterStatsData
                {
                    maxHP = hpOverride,
                    maxMP = mpOverride,
                    attack = attackOverride,
                    defense = defenseOverride,
                    speed = speedOverride
                };
            }
            else
            {
                return template?.GetStatsData() ?? new CharacterStatsData();
            }
        }
    }

    /// <summary>
    /// Formation patterns for character positioning
    /// </summary>
    public enum FormationType
    {
        Line,           // Vertical line
        HorizontalLine, // Horizontal line
        Grid,           // Grid pattern (2 columns)
        V,              // V formation
        Circle,         // Circular formation
        Random          // Random positions
    }
}
