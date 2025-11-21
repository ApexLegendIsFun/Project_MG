using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TurnBasedCombat.Core
{
    /// <summary>
    /// Manages turn order and combat flow using a state machine
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        [Header("Combat Participants")]
        [SerializeField] private List<CombatCharacter> playerCharacters = new List<CombatCharacter>();
        [SerializeField] private List<CombatCharacter> enemyCharacters = new List<CombatCharacter>();

        [Header("Settings")]
        [SerializeField] private float turnDelay = 0.5f;
        [SerializeField] private float enemyThinkTime = 1f;

        // Events
        public UnityEvent<CombatState> OnStateChanged;
        public UnityEvent<CombatCharacter> OnTurnStart;
        public UnityEvent<CombatCharacter> OnTurnEnd;
        public UnityEvent OnBattleStart;
        public UnityEvent OnBattleEnd;

        // State
        private CombatState currentState = CombatState.Idle;
        private List<CombatCharacter> turnOrder = new List<CombatCharacter>();
        private int currentTurnIndex = 0;
        private CombatCharacter currentCharacter;
        
        // Timers
        private float stateTimer = 0f;
        private bool waitingForAction = false;
        private Coroutine currentActionCoroutine = null;

        public CombatState CurrentState => currentState;
        public CombatCharacter CurrentCharacter => currentCharacter;
        public List<CombatCharacter> PlayerCharacters => playerCharacters;
        public List<CombatCharacter> EnemyCharacters => enemyCharacters;

        private void Awake()
        {
            if (OnStateChanged == null) OnStateChanged = new UnityEvent<CombatState>();
            if (OnTurnStart == null) OnTurnStart = new UnityEvent<CombatCharacter>();
            if (OnTurnEnd == null) OnTurnEnd = new UnityEvent<CombatCharacter>();
            if (OnBattleStart == null) OnBattleStart = new UnityEvent();
            if (OnBattleEnd == null) OnBattleEnd = new UnityEvent();
        }

        /// <summary>
        /// Starts the combat
        /// </summary>
        public void StartBattle()
        {
            Debug.Log("[TurnManager] Starting battle");
            
            // Calculate turn order based on speed
            CalculateTurnOrder();

            ChangeState(CombatState.BattleStart);
            OnBattleStart?.Invoke();
            
            stateTimer = turnDelay;
        }

        /// <summary>
        /// State machine update - processes current state each frame
        /// </summary>
        private void Update()
        {
            // Don't process if not in an active battle state
            if (currentState == CombatState.Idle)
                return;

            // Update timer
            if (stateTimer > 0)
            {
                stateTimer -= Time.deltaTime;
                return;
            }

            // Process current state
            switch (currentState)
            {
                case CombatState.BattleStart:
                    ProcessBattleStart();
                    break;

                case CombatState.PlayerTurn:
                    ProcessPlayerTurn();
                    break;

                case CombatState.PlayerAction:
                    ProcessPlayerAction();
                    break;

                case CombatState.EnemyTurn:
                    ProcessEnemyTurn();
                    break;

                case CombatState.EnemyAction:
                    ProcessEnemyAction();
                    break;

                case CombatState.Processing:
                    ProcessEffectsState();
                    break;

                case CombatState.Victory:
                case CombatState.Defeat:
                    // Battle ended, do nothing
                    break;
            }
        }

        /// <summary>
        /// Process BattleStart state - move to first turn
        /// </summary>
        private void ProcessBattleStart()
        {
            Debug.Log("[TurnManager] Processing BattleStart");
            StartNextTurn();
        }

        /// <summary>
        /// Process PlayerTurn state - wait for player input
        /// </summary>
        private void ProcessPlayerTurn()
        {
            // Just wait - player input will call ExecutePlayerAction
            // which will change the state
        }

        /// <summary>
        /// Process PlayerAction state - wait for action to complete
        /// </summary>
        private void ProcessPlayerAction()
        {
            // Wait for action coroutine to complete
            if (!waitingForAction)
            {
                EndCurrentTurn();
            }
        }

        /// <summary>
        /// Process EnemyTurn state - AI decides action
        /// </summary>
        private void ProcessEnemyTurn()
        {
            Debug.Log($"[TurnManager] Enemy {currentCharacter.name} deciding action");
            
            var alivePlayers = playerCharacters.Where(p => p.IsAlive).ToList();

            if (alivePlayers.Count > 0)
            {
                var target = alivePlayers[Random.Range(0, alivePlayers.Count)];
                Debug.Log($"[TurnManager] Enemy targeting {target.name}");

                ChangeState(CombatState.EnemyAction);
                waitingForAction = true;
                currentActionCoroutine = StartCoroutine(ExecuteActionCoroutine(currentCharacter.Attack(target)));
            }
            else
            {
                // No valid targets, end turn
                EndCurrentTurn();
            }
        }

        /// <summary>
        /// Process EnemyAction state - wait for action to complete
        /// </summary>
        private void ProcessEnemyAction()
        {
            // Wait for action coroutine to complete
            if (!waitingForAction)
            {
                EndCurrentTurn();
            }
        }

        /// <summary>
        /// Process effects and check win conditions
        /// </summary>
        private void ProcessEffectsState()
        {
            Debug.Log("[TurnManager] Processing effects");
            
            // Process status effects
            foreach (var character in turnOrder)
            {
                if (character.IsAlive)
                {
                    character.ProcessStatusEffects();
                }
            }

            // Check win/lose conditions
            if (CheckVictory())
            {
                Debug.Log("[TurnManager] Victory!");
                ChangeState(CombatState.Victory);
                OnBattleEnd?.Invoke();
                return;
            }
            else if (CheckDefeat())
            {
                Debug.Log("[TurnManager] Defeat!");
                ChangeState(CombatState.Defeat);
                OnBattleEnd?.Invoke();
                return;
            }

            // Move to next turn
            stateTimer = turnDelay;
            StartNextTurn();
        }

        /// <summary>
        /// Start the next character's turn
        /// </summary>
        private void StartNextTurn()
        {
            // Get next character
            currentCharacter = GetNextCharacter();
            Debug.Log($"[TurnManager] Starting turn for {(currentCharacter != null ? currentCharacter.name : "NULL")}");

            // Skip dead characters
            while (currentCharacter != null && !currentCharacter.IsAlive)
            {
                Debug.Log($"[TurnManager] Skipping dead character {currentCharacter.name}");
                AdvanceTurnIndex();
                currentCharacter = GetNextCharacter();
            }

            if (currentCharacter == null)
            {
                Debug.LogError("[TurnManager] No valid character found!");
                return;
            }

            // Invoke turn start event
            OnTurnStart?.Invoke(currentCharacter);

            // Determine if player or enemy turn
            if (playerCharacters.Contains(currentCharacter))
            {
                Debug.Log($"[TurnManager] Player turn: {currentCharacter.name}");
                ChangeState(CombatState.PlayerTurn);
            }
            else
            {
                Debug.Log($"[TurnManager] Enemy turn: {currentCharacter.name}");
                ChangeState(CombatState.EnemyTurn);
                stateTimer = enemyThinkTime;
            }
        }

        /// <summary>
        /// End the current character's turn
        /// </summary>
        private void EndCurrentTurn()
        {
            Debug.Log($"[TurnManager] Ending turn for {currentCharacter.name}");
            OnTurnEnd?.Invoke(currentCharacter);

            ChangeState(CombatState.Processing);
            AdvanceTurnIndex();
        }

        /// <summary>
        /// Executes a player action
        /// </summary>
        public void ExecutePlayerAction(CombatCharacter target, System.Action<CombatCharacter> action)
        {
            if (currentState != CombatState.PlayerTurn)
            {
                Debug.LogWarning("[TurnManager] ExecutePlayerAction called but not in PlayerTurn state!");
                return;
            }

            Debug.Log($"[TurnManager] Executing player action on {target.name}");
            ChangeState(CombatState.PlayerAction);
            
            waitingForAction = true;
            
            // Execute action immediately (it's a callback, not a coroutine)
            action?.Invoke(target);
            
            // For now, assume action completes immediately
            // If actions become coroutines, we'd need to track them
            waitingForAction = false;
        }

        /// <summary>
        /// Wrapper coroutine to track action completion
        /// </summary>
        private IEnumerator ExecuteActionCoroutine(IEnumerator actionCoroutine)
        {
            yield return StartCoroutine(actionCoroutine);
            waitingForAction = false;
            Debug.Log("[TurnManager] Action completed");
        }

        /// <summary>
        /// Calculates turn order based on character speed
        /// </summary>
        private void CalculateTurnOrder()
        {
            turnOrder.Clear();
            turnOrder.AddRange(playerCharacters);
            turnOrder.AddRange(enemyCharacters);

            // Sort by speed (highest first)
            turnOrder = turnOrder.OrderByDescending(c => c.Speed).ToList();

            Debug.Log($"[TurnManager] Turn order calculated: {string.Join(", ", turnOrder.Select(c => c.name))}");

            currentTurnIndex = 0;
        }

        /// <summary>
        /// Gets the current character in turn order
        /// </summary>
        private CombatCharacter GetNextCharacter()
        {
            if (turnOrder.Count == 0)
                return null;

            return turnOrder[currentTurnIndex];
        }

        /// <summary>
        /// Advances the turn index
        /// </summary>
        private void AdvanceTurnIndex()
        {
            currentTurnIndex++;

            // Loop back to start if we've gone through all characters
            if (currentTurnIndex >= turnOrder.Count)
            {
                currentTurnIndex = 0;
                Debug.Log("[TurnManager] Turn order looping back to start");
            }
        }

        /// <summary>
        /// Checks if all enemies are defeated
        /// </summary>
        private bool CheckVictory()
        {
            return enemyCharacters.All(e => !e.IsAlive);
        }

        /// <summary>
        /// Checks if all players are defeated
        /// </summary>
        private bool CheckDefeat()
        {
            return playerCharacters.All(p => !p.IsAlive);
        }

        /// <summary>
        /// Changes the combat state
        /// </summary>
        private void ChangeState(CombatState newState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
            Debug.Log($"[TurnManager] Combat State: {currentState}");
        }

        /// <summary>
        /// Adds a character to the battle
        /// </summary>
        public void AddCharacter(CombatCharacter character, bool isPlayer)
        {
            if (isPlayer)
                playerCharacters.Add(character);
            else
                enemyCharacters.Add(character);

            if (currentState != CombatState.Idle)
                CalculateTurnOrder();
        }

        /// <summary>
        /// Removes a character from the battle
        /// </summary>
        public void RemoveCharacter(CombatCharacter character)
        {
            playerCharacters.Remove(character);
            enemyCharacters.Remove(character);
            turnOrder.Remove(character);
        }
    }
}
