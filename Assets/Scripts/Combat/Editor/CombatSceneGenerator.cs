using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System.Collections.Generic;
using TurnBasedCombat.Core;
using TurnBasedCombat.Data;

namespace TurnBasedCombat.Editor
{
    /// <summary>
    /// Editor window for generating combat scenes from BattlePresets or manual setup
    /// </summary>
    public class CombatSceneGenerator : EditorWindow
    {
        [Header("BattlePreset Setup")]
        private BattlePreset battlePreset;

        [Header("Manual Override")]
        private bool useManualSetup = true;

        [Header("Player Setup")]
        private string playerName = "Hero";
        private int playerHP = 100;
        private int playerMP = 50;
        private int playerAtk = 15;
        private int playerDef = 8;
        private int playerSpd = 12;
        private Vector3 playerPosition = new Vector3(-4, -1, 0);
        private Color playerColor = Color.cyan;

        [Header("Enemy Setup")]
        private string enemyName = "Goblin";
        private int enemyHP = 80;
        private int enemyMP = 30;
        private int enemyAtk = 12;
        private int enemyDef = 5;
        private int enemySpd = 10;
        private Vector3 enemyPosition = new Vector3(4, -1, 0);
        private Color enemyColor = Color.red;

        private Color backgroundColor = new Color(0.2f, 0.2f, 0.3f);

        [MenuItem("Tools/Combat/Generate Combat Scene")]
        public static void ShowWindow()
        {
            var window = GetWindow<CombatSceneGenerator>("Combat Scene Generator");
            window.minSize = new Vector2(400, 600);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Combat Scene Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Generate combat scenes from BattlePresets or manual setup", MessageType.Info);
            EditorGUILayout.Space(10);

            // ===== BattlePreset Loading Section =====
            EditorGUILayout.LabelField("Load from BattlePreset", EditorStyles.boldLabel);
            
            BattlePreset newPreset = (BattlePreset)EditorGUILayout.ObjectField(
                "Battle Preset", battlePreset, typeof(BattlePreset), false);
            
            if (newPreset != battlePreset)
            {
                battlePreset = newPreset;
                if (battlePreset != null)
                {
                    LoadFromPreset();
                    useManualSetup = false;
                }
            }

            // Show preset validation status
            if (battlePreset != null)
            {
                if (battlePreset.IsValid())
                {
                    EditorGUILayout.HelpBox($"✓ Preset loaded: {battlePreset.BattleName}\nPlayers: {battlePreset.PlayerCharacters.Count}, Enemies: {battlePreset.EnemyCharacters.Count}", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("⚠ Preset is invalid - needs at least 1 player and 1 enemy", MessageType.Warning);
                }
            }

            EditorGUILayout.Space(10);

            // ===== Manual Override Toggle =====
            bool newUseManual = EditorGUILayout.Toggle("Manual Override (1v1 only)", useManualSetup);
            if (newUseManual != useManualSetup)
            {
                useManualSetup = newUseManual;
                if (!useManualSetup && battlePreset != null)
                {
                    LoadFromPreset();
                }
            }

            if (useManualSetup)
            {
                EditorGUILayout.HelpBox("Manual mode: Quick 1v1 setup - Edit values directly below", MessageType.Info);
                EditorGUILayout.Space(5);

                // ===== Player Setup =====
                EditorGUILayout.LabelField("Player Setup", EditorStyles.boldLabel);
                playerName = EditorGUILayout.TextField("Name", playerName);
                playerHP = EditorGUILayout.IntField("HP", playerHP);
                playerMP = EditorGUILayout.IntField("MP", playerMP);
                playerAtk = EditorGUILayout.IntField("Attack", playerAtk);
                playerDef = EditorGUILayout.IntField("Defense", playerDef);
                playerSpd = EditorGUILayout.IntField("Speed", playerSpd);
                playerPosition = EditorGUILayout.Vector3Field("Position", playerPosition);
                playerColor = EditorGUILayout.ColorField("Color", playerColor);

                EditorGUILayout.Space(10);

                // ===== Enemy Setup =====
                EditorGUILayout.LabelField("Enemy Setup", EditorStyles.boldLabel);
                enemyName = EditorGUILayout.TextField("Name", enemyName);
                enemyHP = EditorGUILayout.IntField("HP", enemyHP);
                enemyMP = EditorGUILayout.IntField("MP", enemyMP);
                enemyAtk = EditorGUILayout.IntField("Attack", enemyAtk);
                enemyDef = EditorGUILayout.IntField("Defense", enemyDef);
                enemySpd = EditorGUILayout.IntField("Speed", enemySpd);
                enemyPosition = EditorGUILayout.Vector3Field("Position", enemyPosition);
                enemyColor = EditorGUILayout.ColorField("Color", enemyColor);

                EditorGUILayout.Space(10);

                // ===== Scene Settings =====
                EditorGUILayout.LabelField("Scene Settings", EditorStyles.boldLabel);
                backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
            }
            else
            {
                EditorGUILayout.HelpBox("Using BattlePreset data - toggle Manual Override to edit values", MessageType.Info);
            }

            EditorGUILayout.Space(20);

            // ===== Generate Button =====
            if (GUILayout.Button("Generate Combat Scene", GUILayout.Height(40)))
            {
                GenerateScene();
            }
        }

        #region BattlePreset Loading Methods

        /// <summary>
        /// Load data from BattlePreset for 1v1 preview (first player vs first enemy)
        /// </summary>
        private void LoadFromPreset()
        {
            if (battlePreset == null || !battlePreset.IsValid())
            {
                Debug.LogWarning("Cannot load from preset: Preset is null or invalid");
                return;
            }

            // Load first player
            if (battlePreset.PlayerCharacters.Count > 0)
            {
                var playerTemplate = battlePreset.PlayerCharacters[0];
                if (playerTemplate != null)
                {
                    playerName = playerTemplate.CharacterName;
                    playerHP = playerTemplate.MaxHP;
                    playerMP = playerTemplate.MaxMP;
                    playerAtk = playerTemplate.Attack;
                    playerDef = playerTemplate.Defense;
                    playerSpd = playerTemplate.Speed;
                    playerColor = playerTemplate.SpriteColor;
                    
                    // Use formation position
                    playerPosition = battlePreset.GetCharacterPosition(true, 0, battlePreset.PlayerCharacters.Count);
                }
            }

            // Load first enemy
            if (battlePreset.EnemyCharacters.Count > 0)
            {
                var enemyTemplate = battlePreset.EnemyCharacters[0];
                if (enemyTemplate != null)
                {
                    enemyName = enemyTemplate.CharacterName;
                    enemyHP = enemyTemplate.MaxHP;
                    enemyMP = enemyTemplate.MaxMP;
                    enemyAtk = enemyTemplate.Attack;
                    enemyDef = enemyTemplate.Defense;
                    enemySpd = enemyTemplate.Speed;
                    enemyColor = enemyTemplate.SpriteColor;
                    
                    // Use formation position
                    enemyPosition = battlePreset.GetCharacterPosition(false, 0, battlePreset.EnemyCharacters.Count);
                }
            }

            // Load scene settings
            backgroundColor = battlePreset.BackgroundColor;

            Debug.Log($"Loaded preset: {battlePreset.BattleName} (showing first player vs first enemy)");
        }

        #endregion

        private void GenerateScene()
        {
            Debug.Log("=== Generating Combat Scene ===");

            CreateEventSystem();
            SetupCamera();
            SetupCanvas();

            var (turnManager, combatManager) = CreateManagers();
            var player = CreateCharacter(playerName, true, playerPosition, playerHP, playerMP, playerAtk, playerDef, playerSpd, playerColor);
            var enemy = CreateCharacter(enemyName, false, enemyPosition, enemyHP, enemyMP, enemyAtk, enemyDef, enemySpd, enemyColor);

            turnManager.AddCharacter(player, true);
            turnManager.AddCharacter(enemy, false);

            var canvas = FindObjectOfType<Canvas>();
            var playerHUD = CreateHUD(canvas.transform, player, true);
            var enemyHUD = CreateHUD(canvas.transform, enemy, false);
            var combatLog = CreateCombatLog(canvas.transform);
            var actionMenu = CreateActionMenu(canvas.transform);

            WireUpCombatManager(combatManager, actionMenu, combatLog,
                new List<UI.CombatHUD> { playerHUD },
                new List<UI.CombatHUD> { enemyHUD });

            var actionMenuComp = actionMenu.GetComponent<UI.ActionMenu>();
            actionMenuComp.SetTurnManager(turnManager);
            actionMenuComp.SetCombatLog(combatLog.GetComponent<UI.CombatLog>());

            Debug.Log("=== Combat Scene Generated! ===");
            EditorUtility.DisplayDialog("Success", "Combat scene generated successfully!", "OK");
        }

        #region Scene Setup

        private void CreateEventSystem()
        {
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;

            var obj = new GameObject("EventSystem");
            obj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            obj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        private void SetupCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var camObj = new GameObject("Main Camera");
                camera = camObj.AddComponent<Camera>();
                camera.tag = "MainCamera";
            }

            camera.transform.position = new Vector3(0, 0, -10);
            camera.orthographic = true;
            camera.orthographicSize = 5;
            camera.backgroundColor = backgroundColor;
        }

        private void SetupCanvas()
        {
            if (FindObjectOfType<Canvas>() != null) return;

            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();
        }

        private (TurnManager, CombatManager) CreateManagers()
        {
            var obj = new GameObject("Combat Managers");
            return (obj.AddComponent<TurnManager>(), obj.AddComponent<CombatManager>());
        }

        #endregion

        #region Character Creation

        private CombatCharacter CreateCharacter(string name, bool isPlayer, Vector3 pos,
            int hp, int mp, int atk, int def, int spd, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.position = pos;

            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateSprite();
            sr.color = color;

            var character = obj.AddComponent<CombatCharacter>();
            character.InitializeCharacter(name, isPlayer, hp, mp, atk, def, spd);

            return character;
        }

        private Sprite CreateSprite()
        {
            var tex = new Texture2D(64, 64);
            var pixels = new Color[64 * 64];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        }

        #endregion

        #region UI Creation

        private UI.CombatHUD CreateHUD(Transform parent, CombatCharacter character, bool isPlayer)
        {
            var obj = new GameObject($"{character.CharacterName}_HUD");
            obj.transform.SetParent(parent, false);

            var rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 120);

            if (isPlayer)
            {
                rect.anchorMin = rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0, 0);
                rect.anchoredPosition = new Vector2(10, 10);
            }
            else
            {
                rect.anchorMin = rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                rect.anchoredPosition = new Vector2(-10, -10);
            }

            obj.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

            CreateText("Name", obj.transform, character.CharacterName, 20, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -10), new Vector2(-20, 30));
            CreateSlider("HP", obj.transform, Color.green, new Vector2(0, -45), $"{character.Stats.CurrentHP}/{character.Stats.MaxHP}");
            CreateSlider("MP", obj.transform, Color.blue, new Vector2(0, -75), $"{character.Stats.CurrentMP}/{character.Stats.MaxMP}");

            var hud = obj.AddComponent<UI.CombatHUD>();
            hud.SetCharacter(character);
            return hud;
        }

        private GameObject CreateCombatLog(Transform parent)
        {
            var obj = new GameObject("CombatLog");
            obj.transform.SetParent(parent, false);

            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 0.5f);
            rect.anchoredPosition = new Vector2(-20, 0);
            rect.sizeDelta = new Vector2(400, -140);

            obj.AddComponent<Image>().color = new Color(0, 0, 0, 0.8f);

            CreateText("Title", obj.transform, "전투 로그", 18, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -5), new Vector2(-10, 30));

            var scrollObj = new GameObject("ScrollView");
            scrollObj.transform.SetParent(obj.transform, false);
            var scrollRect = scrollObj.AddComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0);
            scrollRect.anchorMax = new Vector2(1, 1);
            scrollRect.anchoredPosition = new Vector2(0, -20);
            scrollRect.sizeDelta = new Vector2(-10, -50);

            var scroll = scrollObj.AddComponent<ScrollRect>();
            scroll.vertical = true;
            scroll.horizontal = false;

            var content = new GameObject("Content");
            content.transform.SetParent(scrollObj.transform, false);
            var contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = new Vector2(0, 1000);
            scroll.content = contentRect;

            var textObj = new GameObject("LogText");
            textObj.transform.SetParent(content.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 1);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0, 1);
            textRect.sizeDelta = new Vector2(-10, 1000);

            var text = textObj.AddComponent<TextMeshProUGUI>();
            text.fontSize = 14;
            text.alignment = TextAlignmentOptions.TopLeft;
            text.enableWordWrapping = true;
            text.color = Color.white;

            obj.AddComponent<UI.CombatLog>();
            return obj;
        }

        private GameObject CreateActionMenu(Transform parent)
        {
            var obj = new GameObject("ActionMenu");
            obj.transform.SetParent(parent, false);

            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(0, 20);
            rect.sizeDelta = new Vector2(600, 100);

            obj.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

            var panel = new GameObject("Main Menu Panel");
            var panelRect = panel.AddComponent<RectTransform>();
            panelRect.SetParent(rect, false);
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;

            var menu = obj.AddComponent<UI.ActionMenu>();

            var attackBtn = CreateButton(panelRect, "공격", new Vector2(-220, 0), new Color(0.8f, 0.2f, 0.2f));
            var skillBtn = CreateButton(panelRect, "스킬", new Vector2(-80, 0), new Color(0.2f, 0.4f, 0.8f));
            var itemBtn = CreateButton(panelRect, "아이템", new Vector2(60, 0), new Color(0.2f, 0.7f, 0.2f));
            var defendBtn = CreateButton(panelRect, "방어", new Vector2(200, 0), new Color(0.8f, 0.7f, 0.2f));

            var type = typeof(UI.ActionMenu);
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            type.GetField("attackButton", flags)?.SetValue(menu, attackBtn);
            type.GetField("skillsButton", flags)?.SetValue(menu, skillBtn);
            type.GetField("itemsButton", flags)?.SetValue(menu, itemBtn);
            type.GetField("defendButton", flags)?.SetValue(menu, defendBtn);
            type.GetField("mainMenuPanel", flags)?.SetValue(menu, panel);

            panel.SetActive(false);
            return obj;
        }

        #endregion

        #region Helpers

        private void CreateText(string name, Transform parent, string text, int fontSize,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        private void CreateSlider(string name, Transform parent, Color fillColor, Vector2 pos, string label)
        {
            var obj = new GameObject(name + "Slider");
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(-20, 20);

            var slider = obj.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = 100;

            var bg = new GameObject("Background");
            bg.transform.SetParent(obj.transform, false);
            var bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bg.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(obj.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = fillColor;

            slider.fillRect = fillRect;
            slider.targetGraphic = fillImg;

            CreateText("Text", obj.transform, label, 14, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        }

        private Button CreateButton(RectTransform parent, string text, Vector2 pos, Color color)
        {
            var obj = new GameObject(text + " Button");
            var rect = obj.AddComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(120, 60);

            var img = obj.AddComponent<Image>();
            img.color = color;

            var btn = obj.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = color;
            colors.highlightedColor = color * 1.2f;
            colors.pressedColor = color * 0.8f;
            btn.colors = colors;

            CreateText("Text", rect, text, 20, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return btn;
        }

        private void WireUpCombatManager(CombatManager manager, GameObject actionMenu, GameObject combatLog,
            List<UI.CombatHUD> playerHUDs, List<UI.CombatHUD> enemyHUDs)
        {
            var type = typeof(CombatManager);
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

            type.GetField("actionMenu", flags)?.SetValue(manager, actionMenu.GetComponent<UI.ActionMenu>());
            type.GetField("combatLog", flags)?.SetValue(manager, combatLog.GetComponent<UI.CombatLog>());
            type.GetField("playerHUDs", flags)?.SetValue(manager, playerHUDs);
            type.GetField("enemyHUDs", flags)?.SetValue(manager, enemyHUDs);
            type.GetField("startOnAwake", flags)?.SetValue(manager, true);
        }

        #endregion
    }
}
