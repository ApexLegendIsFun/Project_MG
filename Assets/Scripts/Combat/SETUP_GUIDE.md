# Combat System Setup Guide

Step-by-step guide to set up your first combat scene.

## Step 1: Create Combat Scene

1. **Create New Scene**
   - File → New Scene
   - Save as "CombatTest" in `Assets/Scenes/`

2. **Setup Camera**
   - Main Camera should be at position (0, 0, -10)
   - Set Size to 5 for good 2D view
   - Background color: dark blue or black

## Step 2: Create Canvas

1. **Add UI Canvas**
   - Right-click in Hierarchy → UI → Canvas
   - Set Canvas Scaler:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920x1080

2. **Add Event System** (auto-created with Canvas)

## Step 3: Create Combat Manager

1. **Create GameObject**
   - Create Empty GameObject: "CombatManager"
   - Position: (0, 0, 0)

2. **Add Components**
   - Add Component → `TurnManager`
   - Add Component → `CombatManager`

3. **Add Camera Shake**
   - Add Component → `CameraShake` to Main Camera

## Step 4: Create Player Character

1. **Create GameObject**
   - Create Empty GameObject: "Player"
   - Position: (-3, 0, 0)

2. **Add Visual**
   - Add Child GameObject: "Sprite"
   - Add Component → Sprite Renderer
   - Set sprite to a player character sprite (or use Unity square for testing)
   - Set color to blue (for easy identification)

3. **Add Combat Character**
   - Select "Player" GameObject
   - Add Component → `CombatCharacter`
   - Configure in Inspector:
     ```
     Character Name: "Hero"
     Is Player: ✓ (checked)

     Stats:
       Max HP: 100
       Max MP: 50
       Attack: 15
       Defense: 8
       Speed: 12
     ```

4. **Setup Combat Position**
   - Create Child Empty GameObject: "CombatPosition"
   - Position it where you want the character during combat
   - Assign to Combat Character's Combat Position field

## Step 5: Create Enemy Character

1. **Create GameObject**
   - Create Empty GameObject: "Enemy"
   - Position: (3, 0, 0)

2. **Add Visual**
   - Add Child GameObject: "Sprite"
   - Add Component → Sprite Renderer
   - Set sprite to an enemy sprite (or use Unity circle for testing)
   - Set color to red (for easy identification)

3. **Add Combat Character**
   - Select "Enemy" GameObject
   - Add Component → `CombatCharacter`
   - Configure in Inspector:
     ```
     Character Name: "Goblin"
     Is Player: ☐ (unchecked)

     Stats:
       Max HP: 80
       Max MP: 30
       Attack: 12
       Defense: 5
       Speed: 10
     ```

4. **Setup Combat Position**
   - Create Child Empty GameObject: "CombatPosition"
   - Position it where you want the enemy during combat
   - Assign to Combat Character's Combat Position field

## Step 6: Create Player HUD

1. **Create HUD Panel**
   - Right-click Canvas → UI → Panel
   - Rename to "PlayerHUD"
   - Position at bottom-left corner
   - Recommended size: 300x150

2. **Add Character Name**
   - Right-click PlayerHUD → UI → Text - TextMeshPro
   - Rename to "NameText"
   - Position at top of panel
   - Set text: "Hero"
   - Font size: 24

3. **Add HP Slider**
   - Right-click PlayerHUD → UI → Slider
   - Rename to "HPSlider"
   - Position below name
   - Configure:
     ```
     Min Value: 0
     Max Value: 100
     Value: 100
     ```
   - Set Fill color to green

4. **Add HP Text**
   - Right-click HPSlider → UI → Text - TextMeshPro
   - Rename to "HPText"
   - Position centered on slider
   - Set text: "100/100"
   - Font size: 18

5. **Add MP Slider** (repeat HP slider steps)
   - Create slider for MP below HP
   - Set Fill color to blue

6. **Add Turn Indicator** (optional)
   - Right-click PlayerHUD → UI → Image
   - Rename to "TurnIndicator"
   - Position at corner of panel
   - Set color to yellow
   - Initially disable in hierarchy

7. **Add CombatHUD Component**
   - Select PlayerHUD GameObject
   - Add Component → `CombatHUD`
   - Assign references:
     ```
     Character: Player GameObject
     Character Name Text: NameText
     HP Slider: HPSlider
     HP Text: HPText
     MP Slider: MPSlider
     MP Text: MPText
     Turn Indicator: TurnIndicator
     ```

## Step 7: Create Enemy HUD

Repeat Step 6 for enemy, but:
- Position at top-right corner
- Assign Enemy GameObject as Character
- Different colors if desired

## Step 8: Create Action Menu

1. **Create Menu Panel**
   - Right-click Canvas → UI → Panel
   - Rename to "ActionMenuPanel"
   - Position at bottom-center
   - Size: 600x200

2. **Add Action Buttons**
   - Right-click ActionMenuPanel → UI → Button - TextMeshPro
   - Create 4 buttons:
     - "AttackButton" - Text: "Attack"
     - "SkillsButton" - Text: "Skills"
     - "DefendButton" - Text: "Defend"
     - "FleeButton" - Text: "Flee"
   - Arrange horizontally
   - Button size: 120x60 each

3. **Add ActionMenu Component**
   - Select ActionMenuPanel
   - Add Component → `ActionMenu`
   - Assign button references:
     ```
     Attack Button: AttackButton
     Skills Button: SkillsButton
     Defend Button: DefendButton
     Flee Button: FleeButton
     Main Menu Panel: ActionMenuPanel
     ```

## Step 9: Create Damage Number Prefab

1. **Create Damage Number**
   - Right-click Canvas → UI → Text - TextMeshPro
   - Rename to "DamageNumber"
   - Configure:
     ```
     Text: "999"
     Font Size: 48
     Color: Red
     Alignment: Center
     ```

2. **Add Components**
   - Add Component → `DamageNumber`
   - Add Component → Canvas Group
   - Configure DamageNumber:
     ```
     Float Speed: 2
     Fade Speed: 1
     Lifetime: 1.5
     Damage Color: Red
     Heal Color: Green
     ```

3. **Create Prefab**
   - Drag DamageNumber to `Assets/Prefabs/Combat/`
   - Delete from scene (we'll spawn it via script)

4. **Setup Spawner**
   - Select CombatManager
   - Add Component → `DamageNumberSpawner`
   - Assign references:
     ```
     Damage Number Prefab: DamageNumber prefab
     Canvas: Main Canvas
     ```

## Step 10: Wire Up Combat Manager

1. **Select CombatManager GameObject**

2. **Configure TurnManager**
   ```
   Player Characters: (Size: 1)
     Element 0: Player GameObject

   Enemy Characters: (Size: 1)
     Element 0: Enemy GameObject

   Turn Delay: 0.5
   ```

3. **Configure CombatManager**
   ```
   Turn Manager: TurnManager component
   Action Menu: ActionMenuPanel
   Damage Number Spawner: DamageNumberSpawner

   Player HUDs: (Size: 1)
     Element 0: PlayerHUD

   Enemy HUDs: (Size: 1)
     Element 0: EnemyHUD

   Start On Awake: ✓ (checked)
   ```

## Step 11: Test!

1. **Save Scene**
   - Ctrl+S or File → Save

2. **Press Play**
   - Combat should start automatically
   - Turn order calculated by Speed
   - Action menu appears on player's turn
   - Click "Attack" to attack enemy
   - Enemy should counter-attack on their turn
   - HP bars should update
   - Combat continues until win/loss

## Expected Behavior

✅ **On Play:**
- Console shows "Battle started!"
- Turn order is calculated
- First character's turn begins

✅ **Player Turn:**
- Action menu appears
- Console shows "Hero's turn!"
- Turn indicator lights up on player HUD

✅ **Clicking Attack:**
- Target selection (auto-selects first enemy for now)
- Attack animation plays (if animator setup)
- Damage number appears above enemy
- Enemy HP decreases
- Action menu hides

✅ **Enemy Turn:**
- Console shows "Goblin's turn!"
- Enemy automatically attacks player
- Damage number appears above player
- Player HP decreases

✅ **Victory/Defeat:**
- When all enemies defeated: "Victory!"
- When all players defeated: "Defeat!"

## Troubleshooting

### Combat doesn't start
- Check "Start On Awake" is enabled on CombatManager
- Verify TurnManager has character references

### Action menu doesn't appear
- Check ActionMenu component has TurnManager reference
- Verify player character is marked as "Is Player"

### No damage numbers
- Check DamageNumberSpawner has prefab assigned
- Verify Canvas reference is set
- Check camera is tagged "MainCamera"

### HP bars don't update
- Verify CombatHUD has character reference
- Check UI slider references are assigned
- Ensure stats are initialized

### Characters don't attack
- Check characters have stats configured
- Verify Attack stat > 0
- Check console for errors

## Next Steps

Once basic combat works:

1. **Add Animations**
   - Create Animator Controller
   - Add attack, hit, idle animations
   - Assign to character

2. **Add Abilities**
   - Create ability ScriptableObjects
   - Add to character's ability list
   - Configure skills panel

3. **Add Visual Effects**
   - Create particle effects
   - Assign to abilities
   - Test camera shake

4. **Polish UI**
   - Add character portraits
   - Status effect icons
   - Better button visuals
   - Sound effects

5. **Expand Combat**
   - Multiple party members
   - More enemy types
   - Victory/defeat screens
   - Rewards system

## Quick Test Script

For rapid testing, you can add this to CombatManager:

```csharp
void Update()
{
    // Quick test keys
    if (Input.GetKeyDown(KeyCode.Space))
    {
        // Force player attack
        if (turnManager.CurrentState == CombatState.PlayerTurn)
        {
            var enemy = turnManager.EnemyCharacters[0];
            turnManager.ExecutePlayerAction(enemy, (t) =>
                StartCoroutine(turnManager.CurrentCharacter.Attack(t))
            );
        }
    }
}
```

This lets you press Space to quick-attack during testing.

---

**Congratulations!** You now have a working turn-based combat system! 🎮
