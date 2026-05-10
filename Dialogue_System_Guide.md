# Luminang Developer Guide: Dialogue & Interaction System

This guide explains how to use the custom cinematic dialogue and quest systems built for the Luminang project. These tools allow you to create professional NPC interactions, cinematic camera transitions, and dynamic quest objectives with zero extra coding.

---

## 🛠 1. Creating a New NPC
To turn any 3D model into an interactive NPC:

1.  **Select the NPC** in the Hierarchy.
2.  **Add the `Interactable NPC` Script**.
3.  **Setup Proximity**: Add a **Capsule Collider** (or similar) to the NPC and check the **`Is Trigger`** box. The player must enter this trigger for the "Talk" prompt to appear.
4.  **Animator**: If your NPC has animations (Idle, Waving, etc.), drag their **Animator** component into the `Npc Animator` slot.

---

## 🎬 2. Cinematic Camera Close-ups
We use a custom lerping system that bypasses complex Cinemachine settings for reliable results.

1.  **Create a Cam Spot**: Right-click your NPC -> Create Empty. Name it `DialogueCam_Spot`.
2.  **Position it**: Move and rotate it so it frames the NPC’s face exactly how you want it to look during dialogue.
3.  **Hook up the events** in the `InteractableNPC` component:
    *   **On Interact ()**: Drag the NPC here -> Select `InteractableNPC.EnterCloseUp` -> Drag your `DialogueCam_Spot` into the box.
    *   **On Dialogue End ()**: Drag the NPC here -> Select `InteractableNPC.ExitCloseUp`.

---

## 💬 3. Building Dialogues
Dialogues are built using **Dialogue Node** assets (ScriptableObjects).

1.  **Create a Node**: Right-click in your Project folder -> `Create -> Dialogue -> Dialogue Node`.
2.  **Speaker Settings**:
    *   `Speaker Name`: The name displayed at the top of the box.
    *   `Speaker Portrait`: Drag in a 2D Sprite. It will automatically slide into view when the dialogue starts!
3.  **Dialogue Text**: Type what the NPC says.
4.  **Choices**: Add items to the `Choices` list. 
    *   If you leave the `Choices` list **empty**, the dialogue will end after the text finishes.
    *   If you add **one choice**, a "Next >>" button appears.
    *   If you add **multiple choices**, buttons will appear for the player to choose from.

---

## 🚩 4. Objectives & Quest Indicators
The system automatically tracks what the player is supposed to be doing.

### Setting a New Objective:
In the NPC’s **`On Dialogue End ()`** list, add a new entry:
*   Select `InteractableNPC.SetNewObjective`.
*   Type the task (e.g., `"Talk to Apo Lakay"`). This will slide onto the player's screen automatically.

### Using Quest Indicators (Blue Pointers):
1.  Find the **`Quest_Indicator_Prefab`** in the `Assets/Prefabs/Dialogue&Quest` folder.
2.  Drag it onto the target NPC or location.
3.  In the **`Required Objective`** field, type the **EXACT** same text you used in the step above.
4.  The pointer will remain invisible until the player is actually on that specific objective.

---

## ⚡ 5. Automatic "Proximity" Triggers
Use the **`ProximityDisappear`** script for moments where the NPC should react as soon as the player gets close (without clicking anything).

*   **Trigger Distance**: The range (cyan circle in Scene view).
*   **On Triggered ()**: Use this to deactivate the NPC or trigger specific scene events.
*   **Dialogue/Objective**: You can assign a dialogue to start or an objective to change immediately upon entering the range.

---

## 💡 Pro-Tips for the Team
*   **Layering**: Portraits appear behind the dialogue box. If a portrait is blocking the text, move it up in the Hierarchy.
*   **Typing Speed**: You can adjust how fast text appears in the `DialogueSystem_Root` object. Setting it to `0` makes text appear instantly.
*   **Case Sensitivity**: Ensure your objective strings match exactly (e.g., "Find Kalaw" is different from "find kalaw").
