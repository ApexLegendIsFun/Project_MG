# 2D Turn-Based Combat System

A complete turn-based combat system for Unity 2D games.

## Overview

This combat system provides a flexible, event-driven framework for creating turn-based battles with characters, abilities, status effects, and a complete UI.

## Features

- ✅ Turn-based combat with speed-based initiative
- ✅ Character stats system (HP, MP, Attack, Defense, Speed)
- ✅ Combat actions (Attack, Defend, Skills)
- ✅ Status effects (Poison, Burn, Regen, Buffs, Debuffs, etc.)
- ✅ Ability system using ScriptableObjects
- ✅ Combat UI (HUD, action menu, health bars)
- ✅ Visual feedback (damage numbers, camera shake, particle effects)
- ✅ Event system for easy integration
- ✅ Simple enemy AI

## Project Structure

```
Assets/
├── Scripts/
│   └── Combat/
│       ├── Core/              # Core combat logic
│       │   ├── CombatState.cs
│       │   ├── TurnManager.cs
│       │   └── CombatManager.cs
│       ├── Characters/        # Character classes
│       │   ├── CombatCharacter.cs
│       │   └── CharacterStats.cs
│       ├── Abilities/         # Skill system
│       │   └── CombatAbility.cs
│       ├── Effects/           # Visual and status effects
│       │   ├── StatusEffect.cs
│       │   ├── CameraShake.cs
│       │   └── CombatEffect.cs
│       └── UI/                # User interface
│           ├── CombatHUD.cs
│           ├── ActionMenu.cs
│           ├── DamageNumber.cs
│           └── DamageNumberSpawner.cs
├── Data/
│   └── Abilities/             # Ability ScriptableObjects
│       ├── Fireball.asset
│       ├── Heal.asset
│       └── PowerStrike.asset
├── Prefabs/
│   └── Combat/
└── Animations/
    └── Combat/
```

## Quick Start

### 1. Setup Combat Scene

1. Create a new scene or use an existing one
2. Add a `Canvas` for UI elements
3. Add a `Main Camera`
4. Create an empty GameObject called "CombatManager"

### 2. Setup Combat Manager

1. Add the `CombatManager` component to the CombatManager GameObject
2. Add the `TurnManager` component to the same object
3. Configure references in the Inspector

### 3. Create Characters

1. Create a GameObject for your character (e.g., "Player" or "Enemy")
2. Add a `SpriteRenderer` for the character's visual
3. Add the `CombatCharacter` component
4. Configure stats in the Inspector:
   - Set character name
   - Set if it's a player or enemy
   - Configure HP, MP, Attack, Defense, Speed

### 4. Setup UI

1. Create UI elements on the Canvas:
   - Player HUD (health bars, name)
   - Enemy HUD
   - Action Menu (buttons for Attack, Skills, Defend, Flee)
   - Damage Number prefab

2. Add components:
   - `CombatHUD` for each character display
   - `ActionMenu` for player actions
   - `DamageNumberSpawner` for floating numbers

3. Wire up references in CombatManager

### 5. Create Abilities

1. Right-click in Project window
2. Create → Combat → Ability
3. Configure the ability:
   - Name and description
   - MP cost
   - Target type (single enemy, all enemies, self, etc.)
   - Effect type (damage, heal, buff, debuff)
   - Power and multipliers
   - Optional status effects

## Core Systems

### Combat Flow

```
Battle Start
    ↓
Calculate Turn Order (based on Speed)
    ↓
For Each Turn:
    ↓
Player Turn → Select Action → Execute → Process Effects
    ↓
Enemy Turn → AI Decision → Execute → Process Effects
    ↓
Check Win/Loss Conditions
    ↓
Next Turn (or End Battle)
```

### Combat States

- **Idle**: Combat hasn't started
- **BattleStart**: Initializing combat
- **PlayerTurn**: Player is choosing action
- **PlayerAction**: Player action is executing
- **EnemyTurn**: Enemy is deciding action
- **EnemyAction**: Enemy action is executing
- **Processing**: Processing effects, checking win conditions
- **Victory**: Player won
- **Defeat**: Player lost
- **Flee**: Combat ended by fleeing

### Character Stats

```csharp
MaxHP      // Maximum health points
MaxMP      // Maximum magic/skill points
Attack     // Physical attack power
Defense    // Physical defense (damage reduction)
Speed      // Turn order priority (higher = earlier)
CurrentHP  // Current health
CurrentMP  // Current magic points
```

### Status Effects

#### Damage Over Time
- **Poison**: Deals damage each turn
- **Burn**: Deals damage each turn

#### Healing
- **Regen**: Heals HP each turn

#### Stat Modifiers
- **AttackUp/Down**: Modifies attack stat
- **DefenseUp/Down**: Modifies defense stat
- **Haste/Slow**: Modifies speed stat

#### Debuffs
- **Stun**: Cannot act (future implementation)

### Ability System

Abilities are ScriptableObjects with configurable properties:

```csharp
// Basic Info
string abilityName
string description
Sprite icon

// Costs
int mpCost
int cooldown

// Targeting
TargetType targetType  // Single/All, Enemy/Ally

// Effects
AbilityEffectType effectType  // Damage, Heal, Buff, Debuff
int basePower
float powerMultiplier

// Status Effects (optional)
bool appliesStatusEffect
StatusEffectType statusEffectType
int statusEffectDuration
int statusEffectPower
float statusEffectChance  // 0.0 to 1.0

// Visuals
GameObject effectPrefab
Color effectColor
```

## Events System

The combat system uses Unity Events for decoupling and extensibility:

### TurnManager Events

```csharp
OnStateChanged(CombatState)    // When combat state changes
OnTurnStart(CombatCharacter)   // When a turn begins
OnTurnEnd(CombatCharacter)     // When a turn ends
OnBattleStart()                // When battle starts
OnBattleEnd()                  // When battle ends
```

### CombatCharacter Events

```csharp
OnDamageDealt(int)    // When character deals damage
OnDamageTaken(int)    // When character takes damage
OnHealed(int)         // When character is healed
OnDeath()             // When character dies
OnActionComplete()    // When an action finishes
```

### CharacterStats Events

```csharp
OnHPChanged(current, max)  // When HP changes
OnMPChanged(current, max)  // When MP changes
OnDeath()                  // When HP reaches 0
```

## Customization

### Creating Custom Abilities

1. Create a new Ability ScriptableObject
2. Set the ability parameters
3. Optionally create a custom effect prefab
4. Add to character's ability list

### Creating Custom Status Effects

```csharp
var customEffect = new StatusEffect(
    "Effect Name",
    StatusEffectType.Poison,  // Or create custom type
    duration: 3,              // Turns
    power: 5                  // Effect strength
);

character.AddStatusEffect(customEffect);
```

### Extending Character Behavior

Inherit from `CombatCharacter` and override methods:

```csharp
public class CustomCharacter : CombatCharacter
{
    public override IEnumerator Attack(CombatCharacter target)
    {
        // Custom attack logic
        yield return base.Attack(target);
    }
}
```

### Custom Enemy AI

Modify the `EnemyTurnRoutine` in `TurnManager.cs`:

```csharp
private IEnumerator EnemyTurnRoutine()
{
    // Your custom AI logic here
    // Choose targets, abilities, etc.
}
```

## UI Customization

### Damage Numbers

Configure in `DamageNumber.cs`:
- Float speed and direction
- Fade speed
- Lifetime
- Colors for damage/heal/critical

### Camera Shake

Use `CameraShake` component:

```csharp
cameraShake.ShakeLight();   // Small shake
cameraShake.ShakeMedium();  // Medium shake
cameraShake.ShakeHeavy();   // Large shake
```

### Custom HUD

Extend or replace `CombatHUD.cs` to display:
- Character portraits
- Status effect icons
- Turn indicators
- Custom animations

## Best Practices

1. **Use Events**: Subscribe to events for UI updates and game logic
2. **ScriptableObjects**: Create abilities as assets for easy balancing
3. **Coroutines**: Use for action sequences and animations
4. **State Management**: Check combat state before executing actions
5. **Null Checks**: Always validate references before use
6. **Testing**: Test with various stat combinations and abilities

## Performance Tips

1. Object pooling for damage numbers and effects
2. Limit particle system complexity
3. Cache component references
4. Use appropriate texture sizes for sprites
5. Minimize UI rebuilds

## Common Issues

### Characters don't take turns
- Check that characters are added to TurnManager's lists
- Verify Speed stat is set (higher = earlier turns)
- Ensure characters are marked as `IsAlive`

### UI not updating
- Verify event subscriptions are set up
- Check that HUD has correct character reference
- Ensure Canvas is properly configured

### Abilities not working
- Check MP cost vs available MP
- Verify ability ScriptableObject is configured
- Ensure target type matches selected target

### Damage not showing
- Check DamageNumberSpawner is assigned
- Verify damage number prefab exists
- Ensure Canvas has proper render mode

## Future Enhancements

- [ ] Items and inventory system
- [ ] Equipment system
- [ ] Character leveling and progression
- [ ] Save/load combat state
- [ ] Multiple party members
- [ ] Formation system
- [ ] Combo attacks
- [ ] Elemental affinities
- [ ] Difficulty settings
- [ ] Battle rewards and loot
- [ ] Animated sprites
- [ ] Sound effects and music
- [ ] Tutorial system
- [ ] Multiplayer support

## License

This combat system is provided as-is for use in your Unity projects.

## Support

For questions or issues, please refer to the code comments and Unity documentation.
