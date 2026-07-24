# NPC, Lesson, and Minigame System Architecture

This document provides a comprehensive guide to all scripts and systems relating to NPCs, animations, dialogues, lessons, and minigames. Use this as a reference guide for team alignment.

---

## 1. NPC Interaction & Behavior Scripts

### 📄 [InteractableNPC.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/InteractableNPC.cs)
* **What it does**: The entry point for any NPC in the scene. Detects player proximity and triggers dialogue interactions.
* **Where to attach**: Directly to the root NPC GameObject.
* **Key Configuration fields**:
  * `Default Dialogue`: The start [DialogueNode](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Dialogue/DialogueNode.cs) asset to play when first interacting.
  * `Is Organizer` / `Quest Dialogues`: Configures dialogue branches based on active quest lines.
  * `Minigame Settings`: Configures what category/language of minigame this NPC uses.
  * `Dialogue Events (Custom Events)`: Connects string event tags (triggered by dialogue nodes) to UnityEvents in the scene.

### 📄 [InteractableBase.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/InteractableBase.cs)
* **What it does**: The abstract parent class for all interactable objects.
* **Purpose**: Provides shared proximity detection logic and displays prompt titles (like "Talk" or "Inspect").

### 📄 [InteractionManager.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/InteractionManager.cs)
* **What it does**: Singleton manager that continually tracks the player's distance to nearby interactable objects.
* **Purpose**: Coordinates showing/hiding the screen-space "Talk" button prompt.

### 📄 [QuestIndicator.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/QuestIndicator.cs)
* **What it does**: Renders indicators (like exclamation or question marks) above an NPC's head.
* **Purpose**: Visually prompts the player if the NPC has a quest to give, update, or complete.

---

## 2. Dialogue Engine

### 📄 [DialogueNode.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Dialogue/DialogueNode.cs)
* **What it is**: A `ScriptableObject` asset representing a single bubble of dialogue.
* **Key Fields**:
  * `Speaker Name` & `Speaker Portrait`: Metadata shown on the screen.
  * `Dialogue Text` & `Translated Text`: The default English line and regional translation.
  * `Animation Trigger`: Animator trigger name to run when this line is read.
  * `Trigger Event Name`: A custom event string fired *immediately* when the line opens.
  * `End Event Name`: A custom event string fired *when the player clicks to advance* past the line.
  * `Choices`: Array of branching selections (decisions or wrong/right options).

### 📄 [DialogueManager.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Dialogue/DialogueManager.cs)
* **What it does**: The global controller driving active conversations.
* **Purpose**: Connects dialogue asset paths, tracks node history (allowing players to click "Previous"), sets animator triggers on NPCs, and routes selection callbacks.

### 📄 [DialogueUIController.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Dialogue/DialogueUIController.cs)
* **What it does**: Controls the presentation layer of the Dialogue Panel.
* **Purpose**: Animates UI visibility, populates text blocks, spawns choice button prefabs, and routes click listeners.

---

## 3. Lessons & Speech Evaluation

### 📄 [LessonManager.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/LessonManager.cs)
* **What it does**: The core controller of the Lesson interface panel.
* **Purpose**: Dynamic vocabulary loader. Downloads translation and audio lists from the database matching the given category (e.g. `Greetings`) and language ID, spawns rows dynamically, and manages pronunciation audio playback.
* **Callbacks**: `On Lesson Complete` UnityEvent runs once the player exits the lesson window.

### 📄 [LessonIntroPanel.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/LessonIntroPanel.cs)
* **What it does**: Renders overlay informational cards before a lesson begins.
* **Purpose**: Helps frame the regional context before loading database lists.

### 📄 [STTDialogueAdapter.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/Speech/STTDialogueAdapter.cs)
* **What it does**: Connects the Speech-to-Text validation output back to the active dialogue system.
* **Purpose**: When the player pronounces words correctly, this script tells [DialogueManager](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Dialogue/DialogueManager.cs) to programmatically advance the conversation forward.

### 📄 [STTGameController.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/Speech/STTGameController.cs)
* **What it does**: System controller for the recording and evaluation gameplay loop.
* **Purpose**: Fires event handlers before and after validating user speech inputs.

---

## 4. Minigames & Quest Progress

### 📄 [MinigameManager.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/MinigameManager.cs)
* **What it does**: Manages spawning and starting local dialect minigames (Matching Game, Word Rush).
* **Purpose**: Standardized entry hook to instantiate minigame UI panels, assign category tags, and load regional data.

### 📄 [ObjectiveManager.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/ObjectiveManager.cs)
* **What it does**: Drives current checkpoints and active quest objectives.
* **Purpose**: Displays the active quest task on the HUD (e.g. "Talk to Apo Lakay") and tracks progression.

### 📄 [JournalManager.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/JournalManager.cs)
* **What it does**: Keeps track of player progression milestones.
* **Purpose**: Handles unlocking glossary terms, history records, and region details.

---

## 5. Cutscenes

### 📄 [RegionalCutsceneController.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/RegionalCutsceneController.cs)
* **What it does**: Orchestrates regional intro videos between dialogues.
* **Purpose**: Blends transitions (fading to black, playing video, fading back) and auto-advances the conversation to the post-video dialogue node once finished.

---

## 6. NPC Animation Scripts

### 📄 [KalawIdleTest.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/AnimalAnimation/KalawIdleTest.cs)
* **What it does**: Script custom to the Kalaw bird companion.
* **Purpose**: Loops through random still idles, look-around animations, and pointing gestures to make the bird feel alive when sitting in trees.

### 📄 [KalawInteraction.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/AnimalAnimation/KalawInteraction.cs)
* **What it does**: Resets the bird's status after completing dialogues.
* **Purpose**: Resumes idle routines and updates proximity triggers.

### 📄 [HorsePatrol.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/AnimalAnimation/HorsePatrol.cs)
* **What it does**: Waypoint-based movement.
* **Purpose**: Patrols horses smoothly around XZ coordinates, auto-locks height, and adds optional random idle pauses at each waypoint.

### 📄 [HorseEating.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/AnimalAnimation/HorseEating.cs)
* **What it does**: Loops eating clips.
* **Purpose**: Automates feeding animations with customizable rest pauses.

### 📄 [WheelRotator.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/AnimalAnimation/WheelRotator.cs)
* **What it does**: Rotates wagon/kalesa wheels dynamically.
* **Purpose**: Simulates wheel rolling forward or backward depending on the movement distance and direction of the parent wagon.

---

## 7. Quest Path Tracker & Navigation System (Genshin Style)

### 📄 [QuestPathTracker.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/QuestPathTracker.cs)
* **What it does**: Renders a Genshin Impact-style navigation corridor of glowing golden sparkles floating at ground level directly from the player's feet to active quest objectives.
* **Where to attach**: Add to a central manager GameObject in any scene (e.g. `GenshinQuestPathTracker`).
* **Key Features & Optimizations**:
  * **Ground Feet Anchoring (`groundYOffset = 0.05f`)**: Automatically raycasts down to the floor at the player's feet so the path begins cleanly on the ground plane instead of at chest/head height.
  * **Stationary Hovering & Unsynced Phase Offsets**: Sparkles hover in place along the path and pulse/bob independently at different times, creating an organic twinkling effect.
  * **Scattered Trail Corridor (`pathScatterWidth = 0.6f`)**: Sparkles are randomly scattered across a path width corridor following straight/NavMesh direction (no artificial wave patterns).
  * **NavMesh Ground Pathing**: Calculates ground paths using Unity's `NavMesh.CalculatePath` to navigate realistically around walls, buildings, and terrain obstacles.
  * **Mobile (APK) Optimization**: Throttles path recalculation to every `0.25s` (4 times/sec) or when the player moves > 0.5m. Zero runtime GC allocations and capped particle limits (30–50 max) guarantee 60 FPS on Android mobile hardware.
  * **Smart Dialogue & Proximity Hiding**: Automatically hides when in dialogue (`DialogueManager.Instance.IsInDialogue`) or when the player gets within proximity of the target (`stopDistance = 3m`).

### 📄 [QuestTargetMarker.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/UI/Interactions/QuestTargetMarker.cs)
* **What it does**: Target registration component placed on NPCs, pickup items, or location triggers.
* **Purpose**: Registers world positions with `QuestPathTracker`. When its `requiredObjective` matches `ObjectiveManager.Instance.CurrentObjective`, the tracker draws a sparkle path to this target. Works independently across both environment scenes.

### 📄 [QuestPathTrackerSetup.cs](file:///c:/Users/dejes/Luminang/Assets/Scripts/Editor/QuestPathTrackerSetup.cs)
* **What it does**: Unity Editor helper script adding automated menu items under **`Luminang > Quest System`**.
* **Purpose**: Allows single-click creation of tracker GameObjects in scenes and quick attachment of `QuestTargetMarker` components to selected NPCs/objects.

---

### 🚀 Quick Setup Instructions for Scenes

1. **Add Tracker to Scene**:
   * Open an environment scene in Unity.
   * Go to top menu: **`Luminang > Quest System > Add Genshin Quest Path Tracker to Scene`**.
   * A GameObject named `GenshinQuestPathTracker` will automatically be created and pre-configured with LineRenderer & Particle System.

2. **Mark Quest Objectives**:
   * Select your target NPC or Objective Trigger in the hierarchy.
   * Go to top menu: **`Luminang > Quest System > Attach QuestTargetMarker to Selected GameObject`**.
   * In the Inspector, set `requiredObjective` to match your quest ID (e.g. `"Talk to Kalaw"`).

3. **Multi-Scene Usage**:
   * Follow steps 1 & 2 for both environment scenes. The tracker system works standalone in each scene and automatically syncs with active objectives.


