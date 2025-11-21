using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TurnBasedCombat.Core
{
    /// <summary>
    /// Manages turn order and combat flow
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        [Header("Combat Participants")]
        [SerializeField] private List<CombatCharacter> playerCharacters = new List<CombatCharacter>();
        [SerializeField] private List<CombatCharacter> enemyCharacters = new List<CombatCharacter>();

        [Header("Settings")]
        [SerializeField] private float turnDelay = 0.5f;

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
            ChangeState(CombatState.BattleStart);

            // Calculate turn order based on speed
            CalculateTurnOrder();

            OnBattleStart?.Invoke();

            StartCoroutine(BattleLoop());
        }

        /// <summary>
        /// Main battle loop
        /// </summary>
        private IEnumerator BattleLoop()
        {
            yield return new WaitForSeconds(turnDelay);

            while (currentState != CombatState.Victory &&
                   currentState != CombatState.Defeat &&
                   currentState != CombatState.Flee)
            {
                // Get next character in turn order
                currentCharacter = GetNextCharacter();

                if (currentCharacter == null || !currentCharacter.IsAlive)
                {
                    NextTurn();
                    continue;
                }

                // Start turn
                OnTurnStart?.Invoke(currentCharacter);

                // Determine if player or enemy turn
                if (playerCharacters.Contains(currentCharacter))
                {
                    ChangeState(CombatState.PlayerTurn);
                    yield return StartCoroutine(PlayerTurnRoutine());
                }
                else
                {
                    ChangeState(CombatState.EnemyTurn);
                    yield return StartCoroutine(EnemyTurnRoutine());
                }

                OnTurnEnd?.Invoke(currentCharacter);

                // Process effects and check win conditions
                ChangeState(CombatState.Processing);
                yield return StartCoroutine(ProcessEffects());

                // Check win/lose conditions
                if (CheckVictory())
                {
                    ChangeState(CombatState.Victory);
                    OnBattleEnd?.Invoke();
                    break;
                }
                else if (CheckDefeat())
                {
                    ChangeState(CombatState.Defeat);
                    OnBattleEnd?.Invoke();
                    break;
                }

                yield return new WaitForSeconds(turnDelay);
            }
        }

        /// <summary>
        /// Player's turn routine - waits for player input
        /// </summary>
        private IEnumerator PlayerTurnRoutine()
        {
            // Wait for player to select an action
            // This will be triggered by UI buttons
            while (currentState == CombatState.PlayerTurn)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Enemy's turn routine - AI decision making
        /// </summary>
        private IEnumerator EnemyTurnRoutine()
        {
            yield return new WaitForSeconds(1f); // Think time

            // Simple AI: attack random player character
            var aliveEnemies = enemyCharacters.Where(e => e.IsAlive).ToList();
            var alivePlayers = playerCharacters.Where(p => p.IsAlive).ToList();

            if (alivePlayers.Count > 0)
            {
                var target = alivePlayers[Random.Range(0, alivePlayers.Count)];

                ChangeState(CombatState.EnemyAction);
                yield return StartCoroutine(currentCharacter.Attack(target));
            }
        }

        /// <summary>
        /// Processes status effects, regeneration, etc.
        /// </summary>
        private IEnumerator ProcessEffects()
        {
            // Process all active status effects
            foreach (var character in turnOrder)
            {
                if (character.IsAlive)
                {
                    character.ProcessStatusEffects();
                }
            }

            yield return new WaitForSeconds(0.3f);
        }

        /// <summary>
        /// Executes a player action
        /// </summary>
        public void ExecutePlayerAction(CombatCharacter target, System.Action<CombatCharacter> action)
        {
            if (currentState != CombatState.PlayerTurn)
                return;

            ChangeState(CombatState.PlayerAction);
            StartCoroutine(ExecuteActionRoutine(target, action));
        }

        private IEnumerator ExecuteActionRoutine(CombatCharacter target, System.Action<CombatCharacter> action)
        {
            action?.Invoke(target);
            yield return new WaitForSeconds(0.5f);

            // Move to next turn after action completes
            NextTurn();
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

            currentTurnIndex = 0;
        }

        /// <summary>
        /// Gets the next character in turn order
        /// </summary>
        private CombatCharacter GetNextCharacter()
        {
            if (turnOrder.Count == 0)
                return null;

            var character = turnOrder[currentTurnIndex];
            return character;
        }

        /// <summary>
        /// Advances to next turn
        /// </summary>
        private void NextTurn()
        {
            currentTurnIndex++;

            // Loop back to start if we've gone through all characters
            if (currentTurnIndex >= turnOrder.Count)
            {
                currentTurnIndex = 0;
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
            Debug.Log($"Combat State: {currentState}");
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
