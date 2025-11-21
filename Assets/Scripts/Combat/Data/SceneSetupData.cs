using UnityEngine;

namespace TurnBasedCombat.Data
{
    /// <summary>
    /// ScriptableObject for storing scene visual and environment settings
    /// Used in SimpleCombatSceneGenerator for consistent scene appearance
    /// </summary>
    [CreateAssetMenu(fileName = "NewSceneSetup", menuName = "Combat/Scene Setup Data")]
    public class SceneSetupData : ScriptableObject
    {
        [Header("Camera Settings")]
        [SerializeField] private Vector3 cameraPosition = new Vector3(0, 0, -10);
        [SerializeField] private float cameraSize = 5f;
        [SerializeField] private bool orthographic = true;

        [Header("Background")]
        [SerializeField] private Color backgroundColor = new Color(0.2f, 0.2f, 0.3f);

        [Header("UI Colors")]
        [SerializeField] private Color uiPrimaryColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        [SerializeField] private Color uiSecondaryColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        [SerializeField] private Color uiAccentColor = new Color(0.8f, 0.2f, 0.2f);

        [Header("Canvas Settings")]
        [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);

        // Properties
        public Vector3 CameraPosition => cameraPosition;
        public float CameraSize => cameraSize;
        public bool Orthographic => orthographic;
        public Color BackgroundColor => backgroundColor;
        public Color UIPrimaryColor => uiPrimaryColor;
        public Color UISecondaryColor => uiSecondaryColor;
        public Color UIAccentColor => uiAccentColor;
        public Vector2 ReferenceResolution => referenceResolution;

        /// <summary>
        /// Validate if the scene setup data is complete and valid
        /// </summary>
        public bool IsValid()
        {
            return cameraSize > 0 && 
                   referenceResolution.x > 0 && 
                   referenceResolution.y > 0;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Create a default scene setup preset
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/Combat/Presets/Default Scene Setup")]
        public static void CreateDefaultSceneSetup()
        {
            var setup = CreateInstance<SceneSetupData>();
            setup.cameraPosition = new Vector3(0, 0, -10);
            setup.cameraSize = 5f;
            setup.orthographic = true;
            setup.backgroundColor = new Color(0.2f, 0.2f, 0.3f);
            setup.uiPrimaryColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            setup.uiSecondaryColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            setup.uiAccentColor = new Color(0.8f, 0.2f, 0.2f);
            setup.referenceResolution = new Vector2(1920, 1080);

            UnityEditor.AssetDatabase.CreateAsset(setup, "Assets/DefaultScene.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = setup;
        }

        /// <summary>
        /// Create a dark cave scene setup preset
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/Combat/Presets/Dark Cave Scene")]
        public static void CreateDarkCaveSetup()
        {
            var setup = CreateInstance<SceneSetupData>();
            setup.cameraPosition = new Vector3(0, 0, -10);
            setup.cameraSize = 5f;
            setup.orthographic = true;
            setup.backgroundColor = new Color(0.05f, 0.05f, 0.1f);
            setup.uiPrimaryColor = new Color(0.05f, 0.05f, 0.08f, 0.8f);
            setup.uiSecondaryColor = new Color(0.1f, 0.1f, 0.15f, 0.9f);
            setup.uiAccentColor = new Color(0.6f, 0.3f, 0.8f);
            setup.referenceResolution = new Vector2(1920, 1080);

            UnityEditor.AssetDatabase.CreateAsset(setup, "Assets/DarkCaveScene.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = setup;
        }

        /// <summary>
        /// Create a bright forest scene setup preset
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/Combat/Presets/Bright Forest Scene")]
        public static void CreateBrightForestSetup()
        {
            var setup = CreateInstance<SceneSetupData>();
            setup.cameraPosition = new Vector3(0, 0, -10);
            setup.cameraSize = 5f;
            setup.orthographic = true;
            setup.backgroundColor = new Color(0.5f, 0.7f, 0.4f);
            setup.uiPrimaryColor = new Color(0.2f, 0.3f, 0.15f, 0.8f);
            setup.uiSecondaryColor = new Color(0.3f, 0.4f, 0.2f, 0.9f);
            setup.uiAccentColor = new Color(0.2f, 0.8f, 0.3f);
            setup.referenceResolution = new Vector2(1920, 1080);

            UnityEditor.AssetDatabase.CreateAsset(setup, "Assets/BrightForestScene.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = setup;
        }
#endif
    }
}
