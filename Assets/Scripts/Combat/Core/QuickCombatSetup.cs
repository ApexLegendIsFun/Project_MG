using UnityEngine;

namespace TurnBasedCombat.Core
{
    /// <summary>
    /// Quick setup script for testing combat without manual scene setup
    /// Attach this to an empty GameObject to auto-create a basic combat test
    /// </summary>
    public class QuickCombatSetup : MonoBehaviour
    {
        [Header("Auto Setup Settings")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private Vector3 playerPosition = new Vector3(-3, 0, 0);
        [SerializeField] private Vector3 enemyPosition = new Vector3(3, 0, 0);

        [Header("Character Stats")]
        [SerializeField] private string playerName = "Hero";
        [SerializeField] private int playerHP = 100;
        [SerializeField] private int playerMP = 50;
        [SerializeField] private int playerAttack = 15;
        [SerializeField] private int playerDefense = 8;
        [SerializeField] private int playerSpeed = 12;

        [SerializeField] private string enemyName = "Goblin";
        [SerializeField] private int enemyHP = 80;
        [SerializeField] private int enemyMP = 30;
        [SerializeField] private int enemyAttack = 12;
        [SerializeField] private int enemyDefense = 5;
        [SerializeField] private int enemySpeed = 10;

        private TurnManager turnManager;
        private CombatCharacter player;
        private CombatCharacter enemy;

        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupQuickBattle();
            }
        }

        /// <summary>
        /// Automatically create a basic combat setup for testing
        /// </summary>
        [ContextMenu("Setup Quick Battle")]
        public void SetupQuickBattle()
        {
            Debug.Log("Setting up quick battle...");

            // Create TurnManager if it doesn't exist
            turnManager = FindObjectOfType<TurnManager>();
            if (turnManager == null)
            {
                var managerObj = new GameObject("TurnManager");
                turnManager = managerObj.AddComponent<TurnManager>();
            }

            // Create Player
            player = CreateCharacter(playerName, true, playerPosition,
                playerHP, playerMP, playerAttack, playerDefense, playerSpeed, Color.blue);

            // Create Enemy
            enemy = CreateCharacter(enemyName, false, enemyPosition,
                enemyHP, enemyMP, enemyAttack, enemyDefense, enemySpeed, Color.red);

            // Setup combat
            turnManager.AddCharacter(player, true);
            turnManager.AddCharacter(enemy, false);

            Debug.Log("Quick battle setup complete! Press Play to start combat.");
        }

        /// <summary>
        /// Create a character GameObject with stats
        /// </summary>
        private CombatCharacter CreateCharacter(string charName, bool isPlayer, Vector3 position,
            int hp, int mp, int attack, int defense, int speed, Color color)
        {
            // Create main GameObject
            var charObj = new GameObject(charName);
            charObj.transform.position = position;

            // Add visual representation
            var spriteObj = new GameObject("Sprite");
            spriteObj.transform.parent = charObj.transform;
            spriteObj.transform.localPosition = Vector3.zero;

            var spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSimpleSprite();
            spriteRenderer.color = color;

            // Add combat position
            var posObj = new GameObject("CombatPosition");
            posObj.transform.parent = charObj.transform;
            posObj.transform.localPosition = Vector3.zero;

            // Add CombatCharacter component
            var character = charObj.AddComponent<CombatCharacter>();

            // Initialize character with stats using new method
            character.InitializeCharacter(charName, isPlayer, hp, mp, attack, defense, speed);
            character.SetSpriteRenderer(spriteRenderer);
            character.SetCombatPosition(posObj.transform);

            Debug.Log($"Created character: {charName} at {position} with HP:{hp}, MP:{mp}, ATK:{attack}, DEF:{defense}, SPD:{speed}");

            return character;
        }

        /// <summary>
        /// Create a simple sprite for testing (white square)
        /// </summary>
        private Sprite CreateSimpleSprite()
        {
            // Create a simple texture
            Texture2D texture = new Texture2D(100, 100);
            Color[] pixels = new Color[100 * 100];

            // Fill with white
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            texture.SetPixels(pixels);
            texture.Apply();

            // Create sprite from texture
            return Sprite.Create(texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Start the battle
        /// </summary>
        [ContextMenu("Start Battle")]
        public void StartBattle()
        {
            if (turnManager != null)
            {
                turnManager.StartBattle();
            }
            else
            {
                Debug.LogWarning("TurnManager not found! Run SetupQuickBattle first.");
            }
        }

        /// <summary>
        /// Quick attack command for testing
        /// </summary>
        private void Update()
        {
            // Press Space during player turn to quick attack
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (turnManager != null &&
                    turnManager.CurrentState == CombatState.PlayerTurn &&
                    turnManager.CurrentCharacter == player &&
                    enemy != null && enemy.IsAlive)
                {
                    Debug.Log("Quick attack!");
                    turnManager.ExecutePlayerAction(enemy, (target) =>
                    {
                        StartCoroutine(player.Attack(target));
                    });
                }
            }

            // Press D during player turn to defend
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (turnManager != null &&
                    turnManager.CurrentState == CombatState.PlayerTurn &&
                    turnManager.CurrentCharacter == player)
                {
                    Debug.Log("Defend!");
                    turnManager.ExecutePlayerAction(player, (target) =>
                    {
                        StartCoroutine(player.Defend());
                    });
                }
            }
        }

        /// <summary>
        /// Display help text
        /// </summary>
        private void OnGUI()
        {
            if (turnManager != null && turnManager.CurrentState == CombatState.PlayerTurn)
            {
                GUI.Box(new Rect(10, 10, 300, 80), "");
                GUI.Label(new Rect(20, 20, 280, 20), "Player's Turn!");
                GUI.Label(new Rect(20, 40, 280, 20), "Press SPACE to Attack");
                GUI.Label(new Rect(20, 60, 280, 20), "Press D to Defend");
            }

            // Display combat state
            if (turnManager != null)
            {
                GUI.Box(new Rect(10, 100, 300, 100), "");
                GUI.Label(new Rect(20, 110, 280, 20), $"State: {turnManager.CurrentState}");

                if (turnManager.CurrentCharacter != null)
                {
                    GUI.Label(new Rect(20, 130, 280, 20),
                        $"Turn: {turnManager.CurrentCharacter.CharacterName}");
                }

                if (player != null)
                {
                    GUI.Label(new Rect(20, 150, 280, 20),
                        $"Player HP: {player.Stats.CurrentHP}/{player.Stats.MaxHP}");
                }

                if (enemy != null)
                {
                    GUI.Label(new Rect(20, 170, 280, 20),
                        $"Enemy HP: {enemy.Stats.CurrentHP}/{enemy.Stats.MaxHP}");
                }
            }
        }
    }
}
