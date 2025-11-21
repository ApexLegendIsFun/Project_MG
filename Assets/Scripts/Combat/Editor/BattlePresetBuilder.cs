using UnityEngine;
using UnityEditor;
using TurnBasedCombat.Data;

namespace TurnBasedCombat.Editor
{
    /// <summary>
    /// Editor window for creating and managing Battle Presets
    /// </summary>
    public class BattlePresetBuilder : EditorWindow
    {
        private BattlePreset preset;
        private Vector2 scrollPosition;
        private UnityEditor.Editor presetEditor;

        [MenuItem("Tools/Combat/Battle Preset Builder")]
        public static void ShowWindow()
        {
            var window = GetWindow<BattlePresetBuilder>("Battle Preset Builder");
            window.minSize = new Vector2(400, 500);
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("Battle Preset Builder", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Preset selection
            EditorGUILayout.BeginHorizontal();
            preset = (BattlePreset)EditorGUILayout.ObjectField("Current Preset", preset, typeof(BattlePreset), false);
            
            if (GUILayout.Button("Create New", GUILayout.Width(100)))
            {
                CreateNewPreset();
            }
            EditorGUILayout.EndHorizontal();

            if (preset == null)
            {
                EditorGUILayout.HelpBox("Select or create a Battle Preset to begin", MessageType.Info);
                EditorGUILayout.EndScrollView();
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preset Configuration", EditorStyles.boldLabel);

            // Draw preset inspector
            if (presetEditor == null || presetEditor.target != preset)
            {
                presetEditor = UnityEditor.Editor.CreateEditor(preset);
            }
            presetEditor.OnInspectorGUI();

            EditorGUILayout.Space();

            // Quick info
            EditorGUILayout.LabelField("Quick Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Players: {preset.PlayerCharacters.Count}");
            EditorGUILayout.LabelField($"Enemies: {preset.EnemyCharacters.Count}");
            EditorGUILayout.LabelField($"Valid: {(preset.IsValid() ? "Yes" : "No")}");

            EditorGUILayout.Space();

            if (GUILayout.Button("Save Changes", GUILayout.Height(30)))
            {
                SavePreset();
            }

            EditorGUILayout.EndScrollView();
        }

        private void CreateNewPreset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Battle Preset",
                "NewBattlePreset",
                "asset",
                "Create a new battle preset");

            if (!string.IsNullOrEmpty(path))
            {
                var newPreset = CreateInstance<BattlePreset>();
                
                // Use SerializedObject to modify private fields
                SerializedObject so = new SerializedObject(newPreset);
                so.FindProperty("battleName").stringValue = "New Battle";
                so.FindProperty("description").stringValue = "Battle description";
                so.ApplyModifiedProperties();
                
                AssetDatabase.CreateAsset(newPreset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                preset = newPreset;
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = newPreset;

                Debug.Log($"Created new Battle Preset at {path}");
            }
        }

        private void SavePreset()
        {
            if (preset == null) return;

            EditorUtility.SetDirty(preset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Saved preset: {preset.BattleName}");
        }

        private void OnDestroy()
        {
            if (presetEditor != null)
            {
                DestroyImmediate(presetEditor);
            }
        }
    }
}
