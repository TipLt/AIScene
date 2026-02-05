# AIScene - Brainrot Runner AI System

Unity scripts for Enemy AI detection, player/enemy collision elimination, and object pooling for the Brainrot Runner game.

## Features

- **Enemy AI with NavMesh**: Enemies detect and chase the nearest player using Unity's NavMesh system
- **Collision-based Elimination**: When a player unit collides with an enemy unit, both are deactivated (hidden)
- **Object Pooling**: Efficient memory management for spawning/despawning player and enemy units
- **Number Triggers**: Track gates that modify player count (+2, -1, etc.)

## Project Structure

```
Assets/Scripts/
├── AI/
│   └── EnemyAI.cs           # NavMesh-based enemy detection and chasing
├── Combat/
│   └── CollisionHandler.cs  # Player-enemy collision elimination
├── Management/
│   ├── GameManager.cs       # Central game management
│   ├── PlayerManager.cs     # Player unit spawning and management
│   ├── EnemyManager.cs      # Enemy unit spawning and management
│   ├── NumberTrigger.cs     # +/- number gates on track
│   └── EnemySpawnTrigger.cs # Enemy spawn trigger points
└── Pooling/
    └── ObjectPool.cs        # Generic object pooling system
```

## Setup Instructions

### 1. Scene Setup

1. Create an empty scene in Unity
2. Bake a NavMesh on your track/ground (Window > AI > Navigation)

### 2. Tags Setup

Create the following tags:
- `Player` - For player units
- `Enemy` - For enemy units

### 3. Player Setup

1. Create a parent GameObject named `Player`
2. Create a player unit prefab with:
   - A Collider component (set as trigger)
   - `CollisionHandler` component (isEnemy = false)
   - Tag set to "Player"
3. Create a `PlayerManager` in the scene with:
   - `ObjectPool` component configured with player prefab
   - Reference to the Player parent transform

### 4. Enemy Setup

1. Create an Enemy parent GameObject
2. Create an enemy prefab with:
   - NavMeshAgent component
   - `EnemyAI` component
   - `CollisionHandler` component (isEnemy = true)
   - A Collider component (set as trigger)
   - Tag set to "Enemy"
3. Create an `EnemyManager` in the scene with:
   - `ObjectPool` component configured with enemy prefab

### 5. Track Setup

1. Add `NumberTrigger` components to number gate objects (+2, -1, etc.)
2. Add `EnemySpawnTrigger` components where enemies should spawn
3. Configure colliders as triggers

### 6. Game Manager

1. Create a GameManager object with `GameManager` component
2. Reference the PlayerManager and EnemyManager

## Component Reference

### EnemyAI

| Property | Description |
|----------|-------------|
| Detection Radius | Distance at which enemy detects players |
| Chase Update Interval | How often to update chase target |
| Move Speed | NavMeshAgent movement speed |
| Stopping Distance | Distance to stop from target |

### CollisionHandler

| Property | Description |
|----------|-------------|
| Enemy Tag | Tag used to identify enemies |
| Player Tag | Tag used to identify players |
| Is Enemy | Whether this is an enemy unit |

### ObjectPool

| Property | Description |
|----------|-------------|
| Prefab | The prefab to pool |
| Initial Pool Size | Number of objects to pre-instantiate |
| Expandable | Allow pool to grow when empty |

### NumberTrigger

| Property | Description |
|----------|-------------|
| Value | +/- value to apply (e.g., +2, -1) |
| Trigger Once | Only activate once per game |

## Usage Example

```csharp
// Spawn enemies via code
enemyManager.SpawnEnemyGroupAtPosition(new Vector3(0, 0, 10), 5);

// Modify player count
playerManager.ModifyPlayerCount(+3); // Add 3 players
playerManager.ModifyPlayerCount(-2); // Remove 2 players

// Handle number pickup in GameManager
GameManager.Instance.OnNumberPickup(+2);
```

## License

MIT License