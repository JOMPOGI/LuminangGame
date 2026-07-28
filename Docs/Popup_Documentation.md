# Luminang: Achievement Popup System Documentation

The **Popup System** allows you to display one-time achievement or milestone sprites (like "Level Complete" or "Welcome") directly in the middle of a dialogue sequence. When a popup triggers, the dialogue box temporarily hides, the popup fades in, and the player must click to dismiss it and resume the dialogue.

---

## 1. How It Works Under The Hood

- **One-Time Only:** The system acts like an Xbox/Steam achievement. It uses `PlayerPrefs` to remember which popups have been shown. Once a popup is dismissed, it will **never** appear again for that player, even if they replay the same dialogue node.
- **Queueing:** If you trigger multiple popups at the same time (e.g., `ShowPopup:complete_level1,welcome_level2`), the manager will queue them and show them one by one. The player clicks to dismiss the first, and the second one smoothly fades in.
- **Dialogue Interruption:** While the popup is active, `DialogueManager` hides the dialogue UI and waits. Once all queued popups are dismissed, the dialogue automatically resumes.

---

## 2. Setting Up a New Popup

### A. Add the Sprite to the Database
1. Select the **`PopupManager`** GameObject in your scene hierarchy (usually under `UI > Managers`).
2. Look at the `PopupManager` script in the Inspector and find the **Popup Database** list.
3. Click the **`+`** button to add a new entry.
4. **Popup Name:** Type a unique, memorable ID (e.g., `welcome_level1`). This MUST exactly match what you type in the Dialogue Node later.
5. **Popup Sprite:** Drag and drop your popup image from the `Assets/Sprites/UI/PopUps/` folder into this slot.

### B. Trigger it from a Dialogue Node
1. Open your Dialogue graph and select the node where you want the popup to appear.
2. In the node's **Choices** list, find the specific choice/button you want to trigger the popup.
3. In the **`Choice Event`** field, type:  
   `ShowPopup:YOUR_POPUP_NAME`  
   *(Example: `ShowPopup:welcome_level1`)*

**To trigger multiple popups in a row:**  
Separate their names with a comma (no spaces needed):  
`ShowPopup:complete_level1,welcome_level2`

---

## 3. Testing and Debugging

Because popups are strictly one-time-only, they can be tricky to test repeatedly. The `PopupManager` has two built-in tools to help with this:

### Option 1: The "Always Show For Testing" Toggle
In the `PopupManager` Inspector, there is a checkbox called **`Always Show For Testing`**. 
- **If CHECKED:** The system ignores the `PlayerPrefs` memory. Popups will trigger every single time, making it easy to test fading and timing.
- **CRITICAL:** You must uncheck this box before building your final game, otherwise players will see the achievements every single time they play a level!

### Option 2: Resetting the Memory
If you want to test the popups exactly as a player would experience them (showing once and then hiding forever), you can reset the system's memory:
1. Right-click on the `PopupManager (Script)` title bar in the Inspector.
2. Select **`Reset All Popups`** from the context menu at the bottom.
3. This wipes the `PlayerPrefs` memory for all popups, allowing you to earn them again.

---

## 4. Customizing the Fade Animation

You can adjust how fast the popup appears and disappears:
1. Select the **`PopupManager`**.
2. Find the **`Fade Duration`** setting in the Inspector.
3. The default is `0.3` seconds. Increase the number (e.g., `0.8`) for a slow, dramatic fade, or decrease it (e.g., `0.1`) for a snappy, instant appearance.
