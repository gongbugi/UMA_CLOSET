# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 3D project called "UMA_CLOSET" that implements a virtual wardrobe system using the UMA (Unity Multipurpose Avatar) system. The project focuses on character customization and outfit management with a dynamic UI interface.

**Unity Version**: Unity 6000.1.9f1 (ed7b183fd33d)  
**Render Pipeline**: Universal Render Pipeline (URP)  
**Current Branch**: WebGL (준비된 WebGL 빌드용 브랜치)

## Core Architecture

### Data Loading Architecture

The system uses a hybrid data loading approach combining JSON configuration with ScriptableObject assets:

- **JSON Configuration** (`Assets/StreamingAssets/outfits.json`): External outfit definitions with texture paths and metadata
- **OutfitDataLoader** (`Assets/Scripts/OutfitDataLoader.cs`): Singleton service that loads JSON data and creates runtime OutfitData instances
- **Dynamic Asset Loading**: Textures loaded from Resources at runtime based on JSON paths

### Component Interaction Flow

1. **OutfitDataLoader** (Singleton) → Loads JSON data and creates OutfitData instances
2. **WardrobeManager** → Subscribes to OutfitDataLoader events and manages UI state
3. **UMA Avatar Integration** → WardrobeManager applies outfit changes via UMA recipes/overlays
4. **UI Management** → Category filtering, button states, and visual feedback
5. **UIManager** → Handles wardrobe panel animations and camera transitions

### Main Components

- **OutfitDataLoader** (`Assets/Scripts/OutfitDataLoader.cs`): Singleton service responsible for loading outfit data from JSON (`Assets/StreamingAssets/outfits.json`) and creating runtime OutfitData instances. Supports both local file loading and future server integration. Handles thumbnail downloading and caching.

- **WardrobeManager** (`Assets/Scripts/WardrobeManager.cs`): Central wardrobe system managing outfit selection, category filtering (tops/bottoms with subcategories), and UI state management. Subscribes to OutfitDataLoader events and applies outfit changes to UMA avatar using UMA recipe system. Handles texture replacement through overlay modification.

- **OutfitData** (`Assets/Scripts/OutfitData.cs`): ScriptableObject-based data structure defining outfit properties with comprehensive categorization system (ClothingType, TopCategory, BottomCategory enums). Now created dynamically at runtime from JSON data.

- **AvatarRotator** (`Assets/Scripts/AvatarRotator.cs`): Mouse interaction handler for avatar rotation using Unity's new Input System. Includes UI collision detection to prevent rotation when interacting with UI elements.

- **UIManager** (`Assets/Scripts/UIManager.cs`): UI animation controller managing wardrobe panel fold/unfold animations with smooth camera transitions. Handles camera positioning between focused and wide views.

### UMA Integration Architecture

The project integrates deeply with UMA 2.13+ system:
- **Avatar System**: Uses `DynamicCharacterAvatar` for runtime character generation
- **Recipe System**: Uses convention-based recipe loading (e.g., `Custom/Tshirt_Recipe`, `Custom/Shorts_Recipe`)
- **Overlay System**: Visual appearance controlled through UMA overlay system with runtime texture replacement
- **Slot Management**: Outfits mapped to UMA slots ("Chest", "Legs") automatically
- **Asset Storage**: UMA-compatible assets in `Assets/Resources/Custom/` loaded at runtime

### Data Flow Pattern

```
JSON File → OutfitDataLoader → OutfitData (Runtime) → WardrobeManager → UMA Recipe/Overlay → Visual Update
UI Events → Category Filtering → Button State Management → Outfit Selection → UMA Avatar BuildCharacter()
```

## Development Commands

**Unity Project Management:**
- Open project via Unity Hub (Unity 6000.1.9f1 required)
- Build: File → Build Settings → Build  
- Test: Press Play in Unity Editor
- Package Management: Window → Package Manager

**Asset Creation Workflow:**
- **Outfit Data**: Edit `Assets/StreamingAssets/outfits.json` to add new outfits
- **UMA Recipes**: Create recipes in `Assets/Resources/Custom/` following naming convention (e.g., `Tshirt_Recipe`, `Shorts_Recipe`)
- **UMA Overlays**: Create overlays in `Assets/Resources/Custom/` (e.g., `Tshirt_Overlay`, `Shorts_Overlay`) 
- **Textures**: Place outfit textures in `Assets/Resources/` matching JSON `texturePath` values (e.g., `Tshirt_001.jpg`, `Tshirt_002.jpg`)
- **UMA Integration**: Use UMA tools in `Assets/UMA/Examples/` for slot/overlay creation

**JSON Outfit Configuration:**
```json
{
  "id": "tshirt_001",
  "outfitName": "기본 티셔츠",
  "type": "Top",
  "topCategory": "Tshirt",
  "bottomCategory": "Shorts",
  "texturePath": "Tshirt_001",
  "thumbnailUrl": "https://example.com/thumbnails/shirt1.jpg"
}
```

**Note**: The system uses convention-based mapping where outfit types automatically map to UMA recipes and overlays (e.g., Tshirt → `Custom/Tshirt_Recipe` and `Custom/Tshirt_Overlay`).

## Key Dependencies

- **Unity Addressables** (2.6.0): Asset management system (configured but not actively used)
- **Unity Input System** (1.14.0): Mouse/touch interaction handling  
- **Unity Render Pipelines Universal** (17.1.0): URP rendering pipeline
- **UMA System**: Character avatar system (included in Assets/UMA/)
- **TextMeshPro**: UI text rendering
- **UnityWebRequest**: Used for JSON loading and thumbnail downloading

## System Limitations & Patterns

- **Korean Language Support**: Code contains Korean comments - maintain this convention
- **Resource Loading**: Uses Resources.Load pattern for outfit data and UMA assets
- **Single Avatar**: System designed for single character instance
- **Category Constraints**: Fixed category system (extend OutfitData enums for new categories)
- **UMA Slot Names**: Hardcoded slot references ("Chest", "Legs") in WardrobeManager:307-325
- **Texture Path Convention**: Texture paths in JSON should match Resources folder structure without extensions
- **WebGL Optimization**: Current branch optimized for WebGL deployment
- **Convention-Based Mapping**: Recipe and overlay paths determined by outfit type/category through GetRecipePathByConvention and GetOverlayPathByConvention methods
- **UMA Version**: UMA 2.14f6