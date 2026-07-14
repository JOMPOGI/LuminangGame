# Universal Narrative & Dialogue Script Template

This document provides the standard narrative and dialogue framework that every region in LUMINANG will strictly follow. By using standardized placeholders, we ensure consistent pacing, mechanics, and story progression across all languages.

---

### 1. Region Arrival
*(Cinematic: `[Video Filename]`)*
**Narrator:** "`[Region Introduction Narration]`"
*(Fade from video to gameplay. Player spawns at the region's designated hub.)*

### 2. Meet the Regional Guide
**`[Guide Name]`**: "Ah, a new face! Welcome. I am `[Guide Name]`. The Great Fading has quieted our streets, but your arrival brings hope."
**`[Guide Name]`**: "You see, our language is bound to the Language Crystal. Without it, our culture sleeps. But with the Anting-anting you carry, you have the power to hear the old words again. Shall we begin?"

### 3. Regional Story Introduction
**`[Guide Name]`**: "Before we speak, you must understand who we are. `[Region Name]` is known for `[Historical Landmark]` and our proud tradition of `[Traditions]`. Our community thrives on `[Community Value]`. Restoring our words means restoring our very soul."

### 4. Introduce Language Category
**`[Guide Name]`**: "Today, we start with `[Category Name]`. To truly walk among us, you must know these words."

### 5. Lesson Introduction
**`[Guide Name]`**: "This lesson focuses on `[Lesson Name]`. You will use these phrases every day when `[Everyday Use]`. Culturally, this is important because `[Cultural Context]`."

### 6. Language Learning
**`[Guide Name]`**: "Listen closely to the word: **`[Vocabulary Word]`**."
- **Translation:** `[Translation]`
- **Meaning:** `[Meaning]`
- **Context:** `[Context]`
- **Example:** "`[Example Sentence]`"
*(Play Pronunciation Audio)*
**`[Guide Name]`**: "Now, it is your turn. Speak into the Anting-anting. Say: `[Vocabulary Word]`."
- *[STT Evaluation triggered]*
  - **Success:** **`[Guide Name]`**: "Excellent! You have the spirit of a true local."
  - **Retry:** **`[Guide Name]`**: "Not quite. Listen to the rhythm. Try again: `[Vocabulary Word]`."

### 7. Guided Practice (STT)
**`[Guide Name]`**: "Let us put those words together."
- *[Player repeats predefined phrases]*
- *[Whisper Speech-to-Text / Semantic Similarity Scoring evaluates the pronunciation against the LUMINANG Dataset]*

### 8. NPC Interaction
**`[NPC Name]`**: "`[NPC Greeting]`"
*(Player selects and speaks response)*
**Player (STT Expected):** "`[Expected Player Response]`"
- *[STT Evaluation]*
**`[NPC Name]`**: "`[NPC Reaction]`"
**`[Guide Name]`**: "`[Guide Feedback - e.g., "Well done! They understood you perfectly."]`"

### 9. Language Minigame
**`[Guide Name]`**: "Before you use this knowledge out in the open, let us test your memory."
*(Minigame UI opens. Options: Match the Picture, Word Matching, Listening Challenge, Memory Match, Arrange the Phrase, Find the Correct Object, Two Truths One Lie)*

### 10. Story Quest
**`[Guide Name]`**: "Someone in the village needs our help. Your task is: **`[Quest Title]`**."
- **Description:** `[Quest Description]`
- **Involves:** `[NPCs Involved]`
- **Required Language:** `[Expected Language Usage]`
*(Upon Quest Completion)*
**`[NPC Name]`**: "`[Quest Completion Dialogue]`"

### 11. Next Lesson
**`[Guide Name]`**: "You handled that beautifully. We have finished `[Lesson Name]`. Next, we must learn..."

### 12. Next Language Category
**`[Guide Name]`**: "Congratulations! You have mastered `[Category Name]`. But our journey continues. Let us look toward the next step."

### 13. Crystal Resonance Trial
**`[Guide Name]`**: "The time has come. The Crystal is resonating with the words you have learned. You must face the final trial."
- **Scenario:** `[Trial Scenario]`
- **Interactions:** `[Trial NPC Interactions]`
- **Spoken Responses:** `[Expected Spoken Responses]`
*(Upon Trial Completion)*
**`[Guide Name]`**: "`[Completion Dialogue]`"

### 14. Crystal Rekindled
*(Cinematic: `[Video Filename]`)*
**Narrator:** "The Crystal shines once more. The Great Fading recedes from `[Region Name]`. The voices of the ancestors echo in the streets. The language returns."
*(Fade back to gameplay)*
**`[Guide Name]`**: "You have done it! The light has returned to us."

### 15. Journal Update
*(System Prompt / UI Notification)*
- **Journal Updated:** New vocabulary added (`[Vocabulary Word]`, `[Translation]`, `[Pronunciation]`).
- **Lore Added:** `[Cultural Notes]`, `[Regional History]`.
- **Achievement Unlocked:** `[Achievement Name]`

### 16. Region Complete
**`[Guide Name]`**: "Our chapter ends here, but LUMINANG still needs you. There are other crystals, other lands. Go forth, carry our words with you, and rekindle the light!"
