# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 3D project called "UMA_CLOSET" that implements a virtual wardrobe system using the UMA (Unity Multipurpose Avatar) system. The project focuses on character customization and outfit management with a dynamic UI interface.

**Unity Version**: Unity 6000.1.9f1 (ed7b183fd33d)
**Render Pipeline**: Universal Render Pipeline (URP)

## Core Architecture

### Component Interaction Flow

The system follows a hierarchical component architecture where `AvatarLoader` acts as the central coordinator:

1. **AvatarLoader** → Instantiates UMA avatar and connects all managers
2. **WardrobeManager** → Handles outfit selection and applies changes to avatar
3. **UIManager** → Controls UI animations and camera movements
4. **AnimationManager** → Manages avatar animation states
5. **AvatarRotator** → Handles mouse-based avatar rotation

### Main Components

- **WardrobeManager** (`Assets/Scripts/WardrobeManager.cs`): Central wardrobe system managing outfit selection, category filtering (tops/bottoms with subcategories), and UI state management. Uses `Resources.LoadAll<OutfitData>()` to dynamically load outfit assets at runtime.

- **AvatarLoader** (`Assets/Scripts/AvatarLoader.cs`): Avatar lifecycle manager responsible for instantiating UMA avatars from Resources folder and coordinating between all system components. Handles PlayerInput setup and component interconnections.

- **UIManager** (`Assets/Scripts/UIManager.cs`): UI animation controller managing wardrobe panel fold/unfold animations with smooth camera transitions. Uses coroutines for smooth UI transitions and maintains panel state persistence.

- **AnimationManager** (`Assets/Scripts/AnimationManager.cs`): Avatar animation controller with trigger-based animation system. Currently supports Idle and Walk animations via Animator triggers.

- **AvatarRotator** (`Assets/Scripts/AvatarRotator.cs`): Mouse interaction handler for avatar rotation using Unity's new Input System. Includes UI collision detection to prevent rotation when interacting with UI elements.

- **OutfitData** (`Assets/Scripts/OutfitData.cs`): ScriptableObject-based data structure defining outfit properties with comprehensive categorization system (ClothingType, TopCategory, BottomCategory, SleeveType enums).

### UMA Integration Architecture

The project integrates deeply with UMA 2.13+ system:
- **Avatar System**: Uses `DynamicCharacterAvatar` for runtime character generation
- **Slot Management**: Outfits mapped to UMA slots ("Chest", "Legs") via `slotName` properties
- **Overlay System**: Visual appearance controlled through UMA overlay system
- **Recipe System**: Custom T-shirt recipe demonstrates UMA asset creation workflow
- **Asset Storage**: UMA-compatible assets in `Assets/Resources/OutfitDatas/` loaded at runtime

### Data Flow Pattern

```
OutfitData (ScriptableObject) → WardrobeManager → UMA Avatar → Visual Update
UI Events → Category Filtering → Dynamic UI Population → Outfit Selection
```

## Development Commands

**Unity Project Management:**
- Open project via Unity Hub (Unity 6000.1.9f1 required)
- Build: File → Build Settings → Build
- Test: Press Play in Unity Editor
- Package Management: Window → Package Manager

**Asset Creation Workflow:**
- Create OutfitData: Right-click → Create → UMA Closet → Outfit Data
- UMA Recipe Creation: Use UMA tools in Assets/UMA/Examples/
- Import 3D Models: Place in Assets/Custom/ with UMA slot/overlay setup

## Key Dependencies

- **Unity Addressables** (2.6.0): Asset management system (configured but not actively used)
- **Unity Input System** (1.14.0): Mouse/touch interaction handling
- **Unity Render Pipelines Universal** (17.1.0): URP rendering pipeline
- **UMA System**: Character avatar system (included in Assets/UMA/)
- **TextMeshPro**: UI text rendering

## System Limitations & Patterns

- **Korean Language Support**: Code contains Korean comments - maintain this convention
- **Resource Loading**: Uses Resources.LoadAll pattern for outfit data (consider Addressables migration for larger projects)
- **Single Avatar**: System designed for single character instance
- **Category Constraints**: Fixed category system (extend OutfitData enums for new categories)
- **UMA Slot Names**: Hardcoded slot references ("Chest", "Legs") in WardrobeManager:193-195