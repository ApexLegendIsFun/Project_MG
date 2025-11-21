# Test Scene Generator Guide

Complete guide for using the Combat Test Scene Generator system.

## 🎯 Overview

The Test Scene Generator provides three ways to create battle scenes:

1. **Quick Setup** - Instant battles with no configuration (fastest)
2. **Preset System** - Reusable battle configurations (recommended)
3. **Runtime Generation** - Programmatic scene creation

## 🚀 Quick Start

### Method 1: Quick Setup (30 seconds)

**Via Editor Window:**
1. Open `Tools → Combat → Scene Generator`
2. Click "Quick Setup" section
3. Configure player/enemy counts
4. Click "Generate Quick Battle"
5. Press Play!

**Via Component:**
1. Create empty GameObject
2. Add `QuickCombatSetup` component
3. Configure stats in Inspector
4. Press Play!

### Method 2: Using Presets (Recommended)

1. Open `Tools → Combat → Scene Generator`
2. Create/select a Battle Preset
3. Click "Generate From Preset"
4. Press Play!

### Method 3: Runtime Generation

```csharp
// Add to any GameObject
var generator = gameObject.AddComponent<CombatTestSceneGenerator>();

// Assign preset (or create one at runtime)
// Call generate
generator.GenerateScene();
```

## 📦 System Components

### 1. Character Templates

**Purpose**: Reusable character configurations

**Location**: `Assets/Data/CharacterTemplates/`

**Included Templates**:
- **Warrior** - High HP/Defense tank (Blue square)
- **Mage** - High MP spellcaster (Purple diamond)
- **Rogue** - High Speed attacker (Green triangle)
- **Goblin** - Weak enemy (Brown circle)
- **Orc** - Tough enemy (Green square)
- **Dragon** - Boss enemy (Red diamond)

**Creating Custom Templates**:
```
1. Right-click in Project → Create → Combat → Character Template
2. Configure:
   - Name, description
   - Stats (HP, MP, Attack, Defense, Speed)
   - Visual (color, shape, size)
   - Abilities (optional)
3. Save in CharacterTemplates folder
```

**Template Properties**:
| Property | Description |
|----------|-------------|
| Character Name | Display name |
| Description | Template description |
| Is Player | Player or enemy flag |
| MaxHP/MaxMP | Health and mana |
| Attack/Defense/Speed | Combat stats |
| Sprite Color | Character color |
| Shape | Square/Circle/Triangle/Diamond |
| Sprite Size | Scale multiplier |
| Custom Sprite | Optional sprite override |
| Abilities | List of combat abilities |
| Starting Effects | Status effects at battle start |

### 2. Battle Presets

**Purpose**: Complete battle scenario configurations

**Location**: `Assets/Data/BattlePresets/`

**Included Presets**:
- **1v1 Basic Battle** - Simple one-on-one
- **2v2 Party Battle** - Two vs two team fight
- **Boss Fight - Dragon** - Epic boss battle

**Creating Custom Presets**:
```
1. Right-click in Project → Create → Combat → Battle Preset
2. Add character entries:
   - Assign character templates
   - Optionally override stats
3. Configure battle settings
4. Save in BattlePresets folder
```

**Preset Properties**:
| Property | Description |
|----------|-------------|
| Battle Name | Scenario name |
| Description | Battle description |
| Player Characters | List of player character entries |
| Enemy Characters | List of enemy character entries |
| Turn Delay | Delay between turns (seconds) |
| Auto Start Battle | Start immediately on load |
| Player/Enemy Formation | Character positioning pattern |
| Area Centers | Base positions for teams |
| Formation Spacing | Distance between characters |
| Generate UI | Auto-create HUD and menus |
| Enable Damage Numbers | Show floating damage text |
| Enable Camera Shake | Impact feedback |
| Background Color | Camera background |

**Formation Types**:
- **Line** - Vertical line (default)
- **Horizontal Line** - Horizontal arrangement
- **Grid** - 2-column grid
- **V** - V-shaped formation
- **Circle** - Circular arrangement
- **Random** - Random positions in area

### 3. Combat Test Scene Generator

**Component**: `CombatTestSceneGenerator`

**Features**:
- ✅ Complete scene setup (Camera, Canvas, EventSystem)
- ✅ Automatic character creation from templates
- ✅ Full UI generation (HUD, action menu, damage numbers)
- ✅ Camera shake and visual effects
- ✅ Multi-character support
- ✅ Formation-based positioning
- ✅ One-click generation

**Usage**:
```csharp
// Attach to GameObject
CombatTestSceneGenerator generator;

// Assign preset in Inspector OR
// Create preset at runtime

// Generate scene
generator.GenerateScene();

// Or use context menu:
// Right-click component → Generate Battle Scene
```

**Context Menu Commands**:
- `Generate Battle Scene` - Create complete battle
- `Clear Generated Scene` - Remove all combat objects

### 4. Editor Window

**Access**: `Tools → Combat → Scene Generator`

**Features**:
- **Quick Setup Tab** - Fast battle creation
- **Preset Setup Tab** - Preset-based generation
- **Advanced Options** - Scene management

**Quick Setup Options**:
- Battle name
- Player count (1-5)
- Enemy count (1-5)
- Generate UI toggle
- Auto-start toggle

**Preset Setup Features**:
- Preset selection
- Preset info display
- One-click generation
- Direct preset editing
- New preset creation

**Advanced Options**:
- Clear existing scene
- Create example presets
- Create example templates
- Open folders

## 🎨 Customization

### Custom Character Shapes

The system generates simple shapes automatically:

```csharp
public enum CharacterShape
{
    Square,     // Solid square
    Circle,     // Filled circle
    Triangle,   // Upward triangle
    Diamond     // Diamond/rhombus
}
```

### Custom Sprites

To use custom sprites instead of generated shapes:

1. Import your sprite
2. Open Character Template
3. Assign to `Custom Sprite` field
4. Generator will use your sprite

### Stat Overrides

Override template stats in Battle Preset:

1. Add character entry to preset
2. Assign template
3. Enable `Override Stats`
4. Set custom stat values

### Custom Formations

Define formation positions in code:

```csharp
public Vector3 GetCharacterPosition(bool isPlayer, int index, int total)
{
    // Custom position calculation
    return calculatedPosition;
}
```

## 🔧 Advanced Usage

### Runtime Character Creation

```csharp
// Create character from template
CharacterTemplate template = // load template
Vector3 position = new Vector3(0, 0, 0);
CombatCharacter character = template.CreateCharacter(position);

// Add to battle
turnManager.AddCharacter(character, isPlayer: true);
```

### Runtime Preset Creation

```csharp
// Create preset
BattlePreset preset = ScriptableObject.CreateInstance<BattlePreset>();

// Configure via SerializedObject
// or save as asset for reuse
```

### Custom UI Generation

Extend `CombatTestSceneGenerator`:

```csharp
public class CustomSceneGenerator : CombatTestSceneGenerator
{
    protected override void GenerateUI()
    {
        base.GenerateUI();
        // Add custom UI elements
    }
}
```

### Procedural Battle Generation

```csharp
public void GenerateRandomBattle()
{
    var preset = CreateRandomPreset();
    var generator = gameObject.AddComponent<CombatTestSceneGenerator>();
    // Assign preset
    generator.GenerateScene();
}

private BattlePreset CreateRandomPreset()
{
    // Randomly select templates
    // Create preset with random enemies
    // Return configured preset
}
```

## 📊 Example Workflows

### Workflow 1: Quick Testing

```
1. Open Scene Generator (Tools → Combat → Scene Generator)
2. Quick Setup → Set counts
3. Click "Generate Quick Battle"
4. Press Play → Test combat
5. Adjust stats if needed
6. Regenerate
```

### Workflow 2: Balancing Characters

```
1. Create Character Templates for each class
2. Create Battle Preset for testing
3. Generate and test
4. Adjust template stats
5. Regenerate to test changes
6. Repeat until balanced
```

### Workflow 3: Creating Boss Fight

```
1. Create boss Character Template (high stats)
2. Create player party templates
3. Create Boss Fight preset:
   - 3 players in Grid formation
   - 1 boss in center
4. Generate and test
5. Adjust boss stats/abilities
```

### Workflow 4: Multi-Wave Battles

```csharp
public IEnumerator MultiWaveBattle()
{
    // Wave 1
    GenerateWave(wave1Preset);
    yield return WaitForBattleEnd();

    // Wave 2
    ClearEnemies();
    AddNewEnemies(wave2Preset);
    StartNewWave();
}
```

## 🐛 Troubleshooting

### Characters not appearing

**Problem**: Scene generates but no characters visible

**Solutions**:
- Check camera position and size
- Verify character positions (z = 0)
- Check sprite colors (not transparent)
- Verify templates assigned to preset

### Stats not applying

**Problem**: Characters have wrong stats

**Solutions**:
- Check `Override Stats` is enabled if using overrides
- Verify template has correct values
- Check initialization is called properly
- Use Debug.Log to verify stat values

### UI not showing

**Problem**: HUD or action menu missing

**Solutions**:
- Check `Generate UI` is enabled in preset
- Verify Canvas exists in scene
- Check EventSystem is present
- Look for UI elements in Canvas children

### Battle not starting

**Problem**: Scene generates but battle doesn't begin

**Solutions**:
- Check `Auto Start Battle` in preset
- Verify TurnManager has characters
- Check at least 1 player and 1 enemy
- Call `turnManager.StartBattle()` manually

### Formation looks wrong

**Problem**: Characters positioned incorrectly

**Solutions**:
- Adjust `Formation Spacing` in preset
- Change `Area Center` positions
- Try different formation type
- Check character count matches formation

## 💡 Tips & Best Practices

### Performance
- Use object pooling for repeated battles
- Cache sprite textures for generated shapes
- Limit particle effects in large battles
- Use simplified UI for stress testing

### Organization
- Group templates by type (Players/, Enemies/, Bosses/)
- Name presets descriptively
- Version control template changes
- Keep templates minimal, override in presets

### Testing
- Create presets for each test scenario
- Use Quick Setup for rapid iteration
- Save successful preset configurations
- Test with different formation patterns

### Balancing
- Start with basic stats
- Test 1v1 before multi-character
- Gradually increase difficulty
- Document stat changes

### Development
- Create templates early in development
- Update presets as mechanics change
- Use version control for assets
- Share templates across team

## 🎓 Examples

### Example 1: Create Warrior Template

```
1. Right-click → Create → Combat → Character Template
2. Name: "Warrior"
3. Stats:
   - HP: 150
   - MP: 30
   - Attack: 20
   - Defense: 12
   - Speed: 8
4. Visual:
   - Color: Blue (0.2, 0.4, 0.8)
   - Shape: Square
   - Size: (1.2, 1.2)
5. Save
```

### Example 2: Create Boss Fight

```
1. Create Dragon template (stats: 300/100/35/15/10)
2. Create Battle Preset "Dragon Boss Fight"
3. Add player characters:
   - Warrior (template)
   - Mage (template)
   - Rogue (template)
4. Add enemy:
   - Dragon (template)
5. Set formation:
   - Players: Grid
   - Enemy: Line (single)
6. Generate and test
```

### Example 3: Quick Random Battle

```csharp
void GenerateRandomEncounter()
{
    int enemyCount = Random.Range(1, 4);

    // Use Quick Setup component
    quickSetup.quickEnemyCount = enemyCount;
    quickSetup.SetupQuickBattle();
    quickSetup.StartBattle();
}
```

## 📚 Additional Resources

- [Main Combat README](README.md) - Full system documentation
- [Setup Guide](SETUP_GUIDE.md) - Manual scene creation
- [Quick Reference](QUICK_REFERENCE.md) - Code snippets
- [Architecture Guide](ARCHITECTURE.md) - System design

## 🔮 Future Enhancements

Planned features:
- [ ] Preset preview before generation
- [ ] Ability auto-assignment from folders
- [ ] Formation visual editor
- [ ] Template inheritance
- [ ] Battle replay system
- [ ] Automated balancing tools
- [ ] Wave-based battle support
- [ ] Preset sharing/export

---

**Ready to create test battles?** Open `Tools → Combat → Scene Generator` and start testing! 🎮✨
