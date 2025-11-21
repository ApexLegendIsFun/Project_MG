using UnityEngine;
using System.Collections.Generic;

namespace TurnBasedCombat.Core
{
    /// <summary>
    /// Main manager that coordinates all combat systems
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private TurnManager turnManager;

        [Header("UI")]
        [SerializeField] private UI.ActionMenu actionMenu;
        [SerializeField] private UI.CombatLog combatLog;
        [SerializeField] private UI.DamageNumberSpawner damageNumberSpawner;
        [SerializeField] private List<UI.CombatHUD> playerHUDs = new List<UI.CombatHUD>();
        [SerializeField] private List<UI.CombatHUD> enemyHUDs = new List<UI.CombatHUD>();

        [Header("Combat Settings")]
        [SerializeField] private bool startOnAwake = true;

        private void Awake()
        {
            // Auto-find TurnManager if not assigned
            if (turnManager == null)
            {
                turnManager = FindObjectOfType<TurnManager>();
                if (turnManager == null)
                {
                    Debug.LogError("TurnManager not found in scene! Please add a TurnManager to the scene or assign it in the Inspector.");
                    return;
                }
                else
                {
                    Debug.Log("TurnManager found automatically.");
                }
            }
            
            SetupEventListeners();
        }

        private void Start()
        {
            if (startOnAwake)
            {
                InitializeCombat();
            }
        }

        /// <summary>
        /// Setup event listeners
        /// </summary>
        private void SetupEventListeners()
        {
            if (turnManager != null)
            {
                turnManager.OnStateChanged.AddListener(OnCombatStateChanged);
                turnManager.OnTurnStart.AddListener(OnTurnStart);
                turnManager.OnTurnEnd.AddListener(OnTurnEnd);
                turnManager.OnBattleStart.AddListener(OnBattleStart);
                turnManager.OnBattleEnd.AddListener(OnBattleEnd);
            }

            // Setup damage number spawning
            if (damageNumberSpawner != null)
            {
                SetupCharacterEvents(turnManager.PlayerCharacters);
                SetupCharacterEvents(turnManager.EnemyCharacters);
            }

            // Setup UI references
            if (actionMenu != null)
            {
                actionMenu.SetTurnManager(turnManager);
                actionMenu.SetCombatLog(combatLog);
            }
        }

        /// <summary>
        /// Setup events for characters
        /// </summary>
        private void SetupCharacterEvents(List<CombatCharacter> characters)
        {
            foreach (var character in characters)
            {
                character.OnDamageTaken.AddListener((damage) =>
                {
                    if (damageNumberSpawner != null)
                    {
                        damageNumberSpawner.SpawnDamage(character.Position, damage);
                    }
                    
                    if (combatLog != null)
                    {
                        combatLog.LogDamage(character.CharacterName, damage);
                    }
                });

                character.OnHealed.AddListener((heal) =>
                {
                    if (damageNumberSpawner != null)
                    {
                        damageNumberSpawner.SpawnHeal(character.Position, heal);
                    }
                    
                    if (combatLog != null)
                    {
                        combatLog.LogHeal(character.CharacterName, heal);
                    }
                });
            }
        }

        /// <summary>
        /// Initialize combat HUDs
        /// </summary>
        private void InitializeCombat()
        {
            // Setup player HUDs
            for (int i = 0; i < turnManager.PlayerCharacters.Count && i < playerHUDs.Count; i++)
            {
                playerHUDs[i].SetCharacter(turnManager.PlayerCharacters[i]);
            }

            // Setup enemy HUDs
            for (int i = 0; i < turnManager.EnemyCharacters.Count && i < enemyHUDs.Count; i++)
            {
                enemyHUDs[i].SetCharacter(turnManager.EnemyCharacters[i]);
            }

            // Start battle
            turnManager.StartBattle();
        }

        #region Event Handlers

        private void OnCombatStateChanged(CombatState newState)
        {
            Debug.Log($"Combat state changed to: {newState}");

            switch (newState)
            {
                case CombatState.Victory:
                    HandleVictory();
                    break;
                case CombatState.Defeat:
                    HandleDefeat();
                    break;
            }
        }

        private void OnTurnStart(CombatCharacter character)
        {
            Debug.Log($"{character.CharacterName}'s turn!");

            // Log turn start
            if (combatLog != null)
            {
                combatLog.LogTurnStart(character.CharacterName, character.IsPlayer);
            }

            // Update turn indicators
            UpdateTurnIndicators(character);

            // Show action menu if player's turn
            if (character.IsPlayer && actionMenu != null)
            {
                actionMenu.ShowMenu(character);
            }
        }

        private void OnTurnEnd(CombatCharacter character)
        {
            Debug.Log($"{character.CharacterName}'s turn ended");

            // Hide action menu
            if (actionMenu != null)
            {
                actionMenu.HideMenu();
            }
        }

        private void OnBattleStart()
        {
            Debug.Log("Battle started!");
            
            if (combatLog != null)
            {
                combatLog.Clear();
                combatLog.LogMessage("=== 전투 시작! ===");
            }
        }

        private void OnBattleEnd()
        {
            Debug.Log("Battle ended!");
        }

        #endregion

        #region Turn Indicators

        /// <summary>
        /// Update turn indicators on HUDs
        /// </summary>
        private void UpdateTurnIndicators(CombatCharacter activeCharacter)
        {
            // Update player HUDs
            foreach (var hud in playerHUDs)
            {
                // You'll need to add a method to get the character from HUD
                // hud.SetTurnActive(hud.Character == activeCharacter);
            }

            // Update enemy HUDs
            foreach (var hud in enemyHUDs)
            {
                // hud.SetTurnActive(hud.Character == activeCharacter);
            }
        }

        #endregion

        #region Victory/Defeat

        private void HandleVictory()
        {
            Debug.Log("Victory!");
            
            if (combatLog != null)
            {
                combatLog.LogBattleResult(true);
            }
            // TODO: Show victory screen, rewards, etc.
        }

        private void HandleDefeat()
        {
            Debug.Log("Defeat!");
            
            if (combatLog != null)
            {
                combatLog.LogBattleResult(false);
            }
            // TODO: Show defeat screen, game over options, etc.
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start combat manually
        /// </summary>
        public void StartCombat()
        {
            InitializeCombat();
        }

        /// <summary>
        /// Add a player character to combat
        /// </summary>
        public void AddPlayerCharacter(CombatCharacter character)
        {
            turnManager.AddCharacter(character, true);
        }

        /// <summary>
        /// Add an enemy character to combat
        /// </summary>
        public void AddEnemyCharacter(CombatCharacter character)
        {
            turnManager.AddCharacter(character, false);
        }

        #endregion

        private void OnDestroy()
        {
            if (turnManager != null)
            {
                turnManager.OnStateChanged.RemoveListener(OnCombatStateChanged);
                turnManager.OnTurnStart.RemoveListener(OnTurnStart);
                turnManager.OnTurnEnd.RemoveListener(OnTurnEnd);
                turnManager.OnBattleStart.RemoveListener(OnBattleStart);
                turnManager.OnBattleEnd.RemoveListener(OnBattleEnd);
            }
        }
    }
}
