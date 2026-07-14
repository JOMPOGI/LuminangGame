# Complete Ilocos (Calle Crisologo) Chapter Implementation Guide

## Objective
This document merges the Universal Architecture, Regional Content, and Narrative Template to provide a complete, production-ready blueprint for the **Ilocos (Calle Crisologo) Chapter**. All placeholders are replaced with exact Ilokano dialogue and specific story beats. It also provides the structural Unity guidance to implement this seamlessly into the LUMINANG project.

---

## 1. Complete Narrative & Dialogue Script (Ilocos Specific)

### 1.1 Region Arrival
*(Cinematic: `Ilocos_Intro.mp4`)*
**Narrator:** "Welcome to Ilocos, a region known for its enduring history, remarkable architecture, and resilient traditions. The cobblestone streets of Calle Crisologo preserve one of the country's best examples of Spanish colonial heritage, while the Ilokano language continues to reflect generations of stories, craftsmanship, and community. From weaving traditional abel textiles to preparing regional delicacies, language remains at the heart of everyday life."
*(Fade from video to gameplay. Player spawns at the Calle Crisologo Plaza.)*

### 1.2 Meet the Regional Guide
*(Kalaw approaches the player.)*
**Kalaw**: "Ah, a new face! Welcome. I am Kalaw. The Great Fading has quieted our streets, but your arrival brings hope."
**Kalaw**: "You see, our language is bound to the Language Crystal. Without it, our culture sleeps. But with the Anting-anting you carry, you have the power to hear the old words again. Shall we begin?"

### 1.3 Regional Story Introduction
**Kalaw**: "Before we speak, you must understand who we are. Ilocos is known for the ancestral houses of Calle Crisologo and our proud tradition of Abel weaving. Our community thrives on deep respect for our elders and heritage. Restoring our words means restoring our very soul."

---

### 1.4 Category 1: Conversational & Social
**Kalaw**: "Today, we start with the foundations of our community. To truly walk among us, you must know these words."

#### Lesson 1.1: Greetings & Gratitude
**Kalaw**: "This lesson focuses on Greetings and Gratitude. You will use these phrases every day when passing neighbors or thanking vendors. Culturally, this is important because respect is the pillar of Ilokano life."
**Kalaw**: "Listen closely to the phrase: **Naimbag a bigat**."
- **Translation:** Good morning.
- **Meaning:** Used to greet someone respectfully in the morning.
- **Example:** "Naimbag a bigat, Apo."
*(Play Audio: Naimbag a bigat)*
**Kalaw**: "Now, it is your turn. Speak into the Anting-anting. Say: Naimbag a bigat."
*(STT Evaluation)*
- **Success:** **Kalaw**: "Excellent! You have the spirit of a true local."
- **Retry:** **Kalaw**: "Not quite. Listen to the rhythm. Try again: Naimbag a bigat."

#### Story Quest: The Morning Rounds
**Kalaw**: "Someone in the village needs our help. Your task is: **The Morning Rounds**."
- **Description:** Greet the village elder and the master weaver to start the day.
- **Involves:** Apo Lakay, Lola Nida
- **Expected Language:** Naimbag a bigat, Agyamanak (Thank you)
**Apo Lakay**: "You have a bright aura, traveler."
**Player (STT)**: "Naimbag a bigat, Apo."
**Apo Lakay**: "Ah, the old words! Naimbag a bigat to you as well."
*(Upon Quest Completion)*
**Kalaw**: "You handled that beautifully. We have finished Greetings. Next, we must learn..."

---

### 1.5 Category 2: Functional & Navigational
**Kalaw**: "Congratulations! You have mastered Conversational phrases. But our journey continues. Let us look toward the next step."

#### Lesson 2.1: Directions & Counting
**Kalaw**: "This lesson focuses on Directions and Counting. You will use these phrases every day when finding your way through our winding streets or buying goods in the plaza."
**Kalaw**: "Listen closely: **Kanan**, **Kannigid**, **Diretso**."
- **Translation:** Right, Left, Straight.
*(Play Audio)*
**Kalaw**: "Say: Kanan, Kannigid, Diretso."
*(STT Evaluation)*

#### Story Quest: The Lost Fabric
**Kalaw**: "Someone in the village needs our help. Your task is: **The Lost Fabric**."
- **Description:** Help Lito deliver Abel fabric to Aling Riza's stall.
- **Involves:** Lito, Aling Riza
- **Expected Language:** Diretso, Kannigid
**Lito**: "My hands are full, which way is Aling Riza's stall from the plaza?"
**Player (STT)**: "Diretso, then Kannigid."
**Lito**: "Thank you! I will head straight and turn left."

---

### 1.6 Category 3: Grammatical Foundations
**Kalaw**: "You are learning quickly. Now, we must understand how to link our thoughts."
*(Teaches Action Verbs, Linking Verbs, Pronouns, and Interrogatives, concluding with a minigame: Arrange the Phrase.)*

---

### 1.7 Crystal Resonance Trial
**Kalaw**: "The time has come. The Crystal is resonating with the words you have learned. You must face the final trial."
- **Scenario:** **The Heritage Festival Preparation**. The player must navigate Calle Crisologo, greet the arriving merchants, ask for directions to the plaza using proper interrogatives, and buy three items (using counting) from Mang Lance.
- **Expected Spoken Responses:** "Naimbag a bigat," "Pangaasi," "Tallo," "Diretso," "Agyamanak."
*(Upon Trial Completion)*
**Kalaw**: "You did it! Your voice carries the weight of the ancestors!"

### 1.8 Crystal Rekindled & Region Complete
*(Cinematic: `Ilocos_Restoration.mp4`)*
**Narrator:** "The Crystal shines once more. The Great Fading recedes from Ilocos. The voices of the ancestors echo in the streets. The language returns."
*(Fade back to gameplay. Calle Crisologo visually shifts—lanterns light up, colors become more vibrant, ambient market noise increases.)*
**Kalaw**: "You have done it! The light has returned to us."
**Kalaw**: "Our chapter ends here, but LUMINANG still needs you. There are other crystals, other lands. Go forth, carry our words with you, and rekindle the light!"

---

## 2. Journal Implementation
- **Vocabulary Tab:** Automatically adds *Naimbag a bigat*, *Agyamanak*, *Kanan*, *Maysa*, etc., with audio playback buttons.
- **Lore Tab:** Unlocks notes on *Calle Crisologo History* and *Abel Weaving*.
- **Achievements:** Unlocks "Voice of the North" (Ilocos Crystal Restored).

---

## 3. Unity Implementation Guidance

### 3.1 Scene Hierarchy Recommendation
Maintain a clean, scalable hierarchy within `Calle_Crisologo.unity`:
```text
▼ Calle_Crisologo
  ▼ Environment
    ▼ Static_Geometry (Buildings, Roads)
    ▼ Lighting_&_PostProcessing
    ▼ NavMesh
  ▼ Gameplay
    ▼ SpawnPoints
    ▼ Crystal_Interactable
  ▼ Characters
    - PlayerArmature
    ▼ NPCs (Kalaw, Apo_Lakay, Lola_Nida, etc.)
  ▼ Managers
    - RegionManager
    - STTGameController
    - DialogueManager
    - QuestManager
  ▼ UI_Canvas
    - MainHUD
    - DialoguePanel
    - STT_Overlay
    - JournalPanel
```

### 3.2 UI/UX Configuration
- **Main HUD:** Minimalist. Keep quest trackers docked gracefully (e.g., top-right) and ensure the Anting-anting (STT trigger button) is prominent but non-intrusive on mobile.
- **Dialogue Panel:** Ensure a dark, semi-transparent backing for high readability against the 3D background. Include portrait slots for NPCs.
- **STT Interface:** A visual waveform or pulsing mic icon should appear when the player is expected to speak, alongside the expected text.

### 3.3 Script Architecture & Responsibilities
Adopt a decoupled, event-driven architecture to prevent spaghetti code:
- `RegionManager.cs`: Handles region-wide state (Initial load, Cutscene triggers, Crystal restoration state). Listens to `QuestManager`.
- `DialogueManager.cs`: Reads from ScriptableObjects containing the dialogue text, audio clips, and STT expected phrases. Fires events when STT is required.
- `STTGameController.cs` / `GroqWhisperManager.cs`: Listens for STT events from the DialogueManager, activates recording, processes Whisper AI payload, calculates semantic similarity, and fires `OnSpeechEvaluated(bool success)`.
- `QuestManager.cs` / `ObjectiveManager.cs`: Advances objectives based on successful STT events or proximity triggers.
- `JournalManager.cs`: Listens to `OnQuestComplete` or `OnLessonComplete` events to populate ScriptableObject journal data into the UI.

### 3.4 Prefabs & Asset Organization
Ensure folders are named consistently for future regions:
```text
Assets/
  ▼ Regions/
    ▼ Ilocos/
      ▼ Art/
        - Models, Materials, Textures
      ▼ Audio/
        - Voiceovers, Ambient, SFX
      ▼ Data/ (ScriptableObjects)
        - Dialogues, Quests, Vocabulary
      ▼ Prefabs/
        - NPCs, Specific Props
  ▼ CoreSystems/
    - Scripts, Generic UI Prefabs
```
