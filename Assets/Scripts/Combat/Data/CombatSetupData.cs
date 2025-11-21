using UnityEngine;

namespace TurnBasedCombat.Data
{
    /// <summary>
    /// ScriptableObject for storing combat setup data for a single character
    /// Used in SimpleCombatSceneGenerator for quick testing scenarios
    /// </summary>
    [CreateAssetMenu(fileName = "NewCombatSetup", menuName = "Combat/Combat Setup Data")]
    public class CombatSetupData : ScriptableObject
    {
        [Header("Character Info")]
        [SerializeField] private string characterName = "Character";
        [SerializeField] private bool isPlayerSetup = true;

        [Header("Stats")]
        [SerializeField] private int maxHP = 100;
        [SerializeField] private int maxMP = 50;
        [SerializeField] private int attack = 15;
        [SerializeField] private int defense = 8;
        [SerializeField] private int speed = 12;

        [Header("Position & Visuals")]
        [SerializeField] private Vector3 position = Vector3.zero;
        [SerializeField] private Color spriteColor = Color.white;
        [SerializeField] private CharacterShape shape = CharacterShape.Square;

        // Properties
        public string CharacterName => characterName;
        public bool IsPlayerSetup => isPlayerSetup;
        public int MaxHP => maxHP;
        public int MaxMP => maxMP;
        public int Attack => attack;
        public int Defense => defense;
        public int Speed => speed;
        public Vector3 Position => position;
        public Color SpriteColor => spriteColor;
        public CharacterShape Shape => shape;

        /// <summary>
        /// Validate if the setup data is complete and valid
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(characterName) &&
                   maxHP > 0 &&
                   maxMP >= 0 &&
                   attack >= 0 &&
                   defense >= 0 &&
                   speed > 0;
        }

        /// <summary>
        /// Get stats as a struct for easy passing
        /// </summary>
        public CharacterStatsData GetStatsData()
        {
            return new CharacterStatsData
            {
                maxHP = this.maxHP,
                maxMP = this.maxMP,
                attack = this.attack,
                defense = this.defense,
                speed = this.speed
            };
        }

#if UNITY_EDITOR
        /// <summary>
        /// Create a default player setup preset
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/Combat/Presets/Default Player Setup")]
        public static void CreateDefaultPlayerSetup()
        {
            var setup = CreateInstance<CombatSetupData>();
            setup.characterName = "Hero";
            setup.isPlayerSetup = true;
            setup.maxHP = 100;
            setup.maxMP = 50;
            setup.attack = 15;
            setup.defense = 8;
            setup.speed = 12;
            setup.position = new Vector3(-4, -1, 0);
            setup.spriteColor = Color.cyan;
            setup.shape = CharacterShape.Square;

            UnityEditor.AssetDatabase.CreateAsset(setup, "Assets/Hero_Default.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = setup;
        }

        /// <summary>
        /// Create a default enemy setup preset
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/Combat/Presets/Default Enemy Setup")]
        public static void CreateDefaultEnemySetup()
        {
            var setup = CreateInstance<CombatSetupData>();
            setup.characterName = "Goblin";
            setup.isPlayerSetup = false;
            setup.maxHP = 80;
            setup.maxMP = 30;
            setup.attack = 12;
            setup.defense = 5;
            setup.speed = 10;
            setup.position = new Vector3(4, -1, 0);
            setup.spriteColor = Color.red;
            setup.shape = CharacterShape.Circle;

            UnityEditor.AssetDatabase.CreateAsset(setup, "Assets/Goblin_Default.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = setup;
        }
#endif
    }
}
