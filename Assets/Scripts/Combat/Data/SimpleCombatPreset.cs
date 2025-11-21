using UnityEngine;

namespace TurnBasedCombat.Data
{
    /// <summary>
    /// Complete preset for a simple 1v1 combat scenario
    /// Combines player setup, enemy setup, and scene setup for one-click scene generation
    /// </summary>
    [CreateAssetMenu(fileName = "NewCombatPreset", menuName = "Combat/Simple Combat Preset")]
    public class SimpleCombatPreset : ScriptableObject
    {
        [Header("Preset Info")]
        [SerializeField] private string presetName = "New Combat Preset";
        [TextArea(2, 4)]
        [SerializeField] private string description = "Preset description";

        [Header("Combat Setup")]
        [SerializeField] private CombatSetupData playerSetup;
        [SerializeField] private CombatSetupData enemySetup;

        [Header("Scene Setup")]
        [SerializeField] private SceneSetupData sceneSetup;

        // Properties
        public string PresetName => presetName;
        public string Description => description;
        public CombatSetupData PlayerSetup => playerSetup;
        public CombatSetupData EnemySetup => enemySetup;
        public SceneSetupData SceneSetup => sceneSetup;

        /// <summary>
        /// Validate if the preset has all required data
        /// </summary>
        public bool IsValid()
        {
            bool hasPlayerSetup = playerSetup != null && playerSetup.IsValid();
            bool hasEnemySetup = enemySetup != null && enemySetup.IsValid();
            bool hasSceneSetup = sceneSetup != null && sceneSetup.IsValid();

            return hasPlayerSetup && hasEnemySetup && hasSceneSetup;
        }

        /// <summary>
        /// Get validation status message for debugging
        /// </summary>
        public string GetValidationMessage()
        {
            if (IsValid())
                return "✓ Preset is valid and ready to use";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("✗ Preset validation failed:");

            if (playerSetup == null)
                sb.AppendLine("  - Player Setup is missing");
            else if (!playerSetup.IsValid())
                sb.AppendLine("  - Player Setup is invalid");

            if (enemySetup == null)
                sb.AppendLine("  - Enemy Setup is missing");
            else if (!enemySetup.IsValid())
                sb.AppendLine("  - Enemy Setup is invalid");

            if (sceneSetup == null)
                sb.AppendLine("  - Scene Setup is missing");
            else if (!sceneSetup.IsValid())
                sb.AppendLine("  - Scene Setup is invalid");

            return sb.ToString();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Show validation status in the Inspector
        /// </summary>
        private void OnValidate()
        {
            // This will trigger whenever the asset is modified in the Inspector
            if (!IsValid())
            {
                Debug.LogWarning($"[{name}] {GetValidationMessage()}", this);
            }
        }

        /// <summary>
        /// Create a default Hero vs Goblin preset
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/Combat/Presets/Hero vs Goblin Preset")]
        public static void CreateHeroVsGoblinPreset()
        {
            var preset = CreateInstance<SimpleCombatPreset>();
            preset.presetName = "Hero vs Goblin";
            preset.description = "Classic 1v1 battle between a brave hero and a cunning goblin";

            // Note: In practice, you would assign existing CombatSetupData and SceneSetupData assets here
            // For now, we'll leave them null - user needs to assign them in Inspector

            UnityEditor.AssetDatabase.CreateAsset(preset, "Assets/HeroVsGoblin.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = preset;

            Debug.Log("Created Hero vs Goblin preset. Please assign Player Setup, Enemy Setup, and Scene Setup in the Inspector.");
        }

        /// <summary>
        /// Create a test combat preset template
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/Combat/Presets/Test Combat Preset")]
        public static void CreateTestPreset()
        {
            var preset = CreateInstance<SimpleCombatPreset>();
            preset.presetName = "Test Battle";
            preset.description = "Quick test battle for debugging and balancing";

            UnityEditor.AssetDatabase.CreateAsset(preset, "Assets/TestBattle.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = preset;

            Debug.Log("Created Test Battle preset. Please assign Player Setup, Enemy Setup, and Scene Setup in the Inspector.");
        }
#endif
    }
}
