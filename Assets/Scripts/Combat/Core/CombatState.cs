using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedCombat.Core
{
    /// <summary>
    /// Defines all possible states in the combat system
    /// </summary>
    public enum CombatState
    {
        Idle,           // Combat hasn't started
        BattleStart,    // Initializing combat
        PlayerTurn,     // Player is choosing action
        PlayerAction,   // Player action is executing
        EnemyTurn,      // Enemy is deciding action
        EnemyAction,    // Enemy action is executing
        Processing,     // Processing effects, checking win conditions
        Victory,        // Player won
        Defeat,         // Player lost
        Flee            // Combat ended by fleeing
    }
}
