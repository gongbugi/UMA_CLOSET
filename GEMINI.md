# GEMINI.md

This file provides context to the Gemini AI assistant for working with the "UMA_CLOSET" project.

## Project Overview

This is a Unity 3D project designed to create a virtual clothing try-on or "closet" experience. It utilizes the **UMA (Unity Multipurpose Avatar)** framework to dynamically create and customize a character avatar. The user can select different clothing items from a UI, which are then applied to the avatar.

- **Unity Version**: `6000.1.9f1` (from `ProjectSettings/ProjectVersion.txt`)
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Platform Focus**: Likely WebGL or standalone PC, given the UI and interaction model.

## Core Architecture

The project's logic is primarily driven by a few key C# scripts that manage the UI, wardrobe data, and the avatar itself.

### Key C# Scripts

-   **`WardrobeManager.cs`**: This is the central script for managing clothing.
    -   It loads outfit data from `outfits.json`.
    -   It dynamically creates UI buttons for different clothing categories (Tops, Bottoms) and subcategories.
    -   It handles the logic for filtering and displaying outfits.
    -   *(Note: The actual logic for applying the clothes to the UMA avatar seems to have been removed or is incomplete in the provided script, as `ChangeOutfitByIndex` is empty.)*

-   **`UIManager.cs`**: This script controls the user interface animations and camera movements.
    -   It manages the sliding animation of the wardrobe panel (folding and unfolding).
    -   It smoothly transitions the camera between a wide view and a focused view of the avatar.

-   **`OutfitDataLoader.cs`**: Responsible for loading the `outfits.json` file from the `StreamingAssets` folder and making the data available to the `WardrobeManager`.

### Data Flow

1.  **`OutfitDataLoader`** reads `outfits.json` at runtime.
2.  It passes the loaded `OutfitData` list to the **`WardrobeManager`**.
3.  **`WardrobeManager`** populates the UI with buttons based on the outfit data.
4.  User interaction with the UI (e.g., clicking a category button) triggers filtering logic in the `WardrobeManager`.
5.  The **`UIManager`** handles the visual presentation, such as sliding the UI panel in and out and moving the camera.

### Data Structure

-   **`outfits.json`**: A simple JSON file located in `Assets/StreamingAssets` that defines the properties of each clothing item, such as its ID, name, type (Top/Bottom), and slot name for the UMA avatar.

## Building and Running

-   **Running in Editor**: Open the project in the Unity Hub (version `6000.1.9f1` or compatible). Open the main scene (likely `Assets/Scenes/MainScene.unity`) and press the "Play" button.
-   **Building**: Use the `File > Build Settings` menu in Unity to build the project for a target platform like Windows or WebGL.

## Key Dependencies

The project relies on several Unity packages, as defined in `Packages/manifest.json`:

-   **UMA 2**: The core avatar system. It is not a package but is included directly in the `Assets/UMA` folder.
-   **`com.unity.inputsystem`**: For handling user input.
-   **`com.unity.render-pipelines.universal`**: The Universal Render Pipeline for graphics.
-   **`com.unity.addressables`**: Set up but might not be in active use, as scripts currently load from `StreamingAssets`.

## Development Conventions

-   **Korean Comments**: The C# scripts contain comments written in Korean. This convention should be maintained.
-   **UI Prefabs**: The UI is built using prefabs like `OutfitItemButton.prefab` which are instantiated at runtime.
-   **Data-Driven UI**: The wardrobe content is not hardcoded but driven by the contents of `outfits.json`.
