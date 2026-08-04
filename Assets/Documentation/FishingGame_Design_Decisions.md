# 🎣 Fishing Game — Design Decisions Log

> This document captures all design decisions made during planning.
> Reference this before implementing or changing anything in the Fishing Game.

---

## 🎮 What is Luminang?

**Luminang** means "to journey/travel" in Filipino.
It is a regional language learning game set in the Philippines.
Players travel through regions and learn real local languages (Ilokano, Cebuano) through NPC interactions, exploration, and mini-games.

---

## 📋 Lesson Flow (Every Lesson)

Each lesson has exactly **3 activities**:

```
1. NPC Teaching Phase
   - An NPC teaches the player a word/phrase
   - Player repeats it using STT (Speech-to-Text)
   - Meaning and usage of the word is shown

2. Talk to NPCs Phase
   - Player walks around and talks to different NPCs
   - Player picks the correct response from multiple choices
   - Tests comprehension in a social context

3. Mini-Game Phase  ← Fishing Game lives here
   - A fun themed mini-game pops up
   - Tests active recall of the lesson vocabulary
   - Always includes STT
```

Completing all 3 activities = lesson complete = next lesson unlocked.

---

## 🐟 Fishing Game — Design Decisions

### Input Method
- **Player clicks on a fish** to select it.
- A tooltip pops up over the selected fish showing its word.
- Player clicks the **"Catch"** button to cast the line.
- The hook latches onto the fish and reels it in.

### Fish Movement
- Fish swim **freely and chaotically** (random directions, different speeds)
- Adds challenge and urgency to catching the right fish

### Data Source
- **Offline / Local JSON** (no internet required during gameplay)
- Two files work together:
  - `Assets/Resources/LuminangPhrases.json` → actual word translations (already exists)
  - `Assets/Data/Minigames/FishingGame/Greetings.json` → round questions, correct phrase IDs, distractor IDs

### Fish Word Signs (Tooltips)
- Player clicks a fish to reveal its word.
- A **tooltip speech bubble** smoothly pops up and follows that specific fish.
- If the player clicks another fish, the tooltip shrinks and pops up on the new fish.

### Bait System
- **20 total baits** given at the start of the game.
- The game consists of **15 rounds** (15 correct fish to catch).
- **EVERY catch attempt costs 1 bait**, whether it is the correct fish or the wrong fish.
- If they catch a wrong fish, they waste a bait and make no progress.
- This gives the player exactly a **5-mistake buffer** (20 baits - 15 rounds = 5 allowed mistakes).
- If baits reach 0 before all 15 rounds are completed → game ends early.

### Wrong Fish Behavior
- Screen **shakes** to indicate wrong answer
- **1 bait is used up** (HUD updates)
- Hook retracts, fish swims away
- Player continues fishing

### Correct Fish Behavior
- Fish gets hooked and floats to the **center of the screen**
- Word/phrase is shown underneath the fish
- A **mic button** appears
- Player must say the word/phrase via STT before proceeding

### STT (Speech-to-Text) Rules
- Player has **3 tries** to say the word correctly
- Pass threshold: **80% accuracy** (from PhraseEvaluator)
- **Try 1 fail** → "Not quite! Try again."
- **Try 2 fail** → "Almost there! One more try."
- **Try 3 fail** → "Nice try! Keep practicing." → fish swims back into the water
- After 3 fails: **no bait is lost** — fish simply returns to the water, player must catch it again
- STT uses existing: `SpeechRecorder` → `GroqWhisperManager` → `PhraseEvaluator.EvaluateSpeech()`

### Win Condition
- Complete all **15 rounds** (catch 15 correct fish + speak them) before running out of bait.
- Game ends when baits run out (0 baits left).
- Minimum to "pass": **10/15** correct catches.

---

## ⭐ Star & Coin Reward System

Stars are based on **how many rounds completed correctly** (correct catch + STT passed):

| Correct Rounds | Stars | Coins Earned |
|---|---|---|
| 5 / 5 | ⭐⭐⭐⭐⭐ | 100% of lesson coins |
| 4 / 5 | ⭐⭐⭐⭐ | 80% |
| 3 / 5 | ⭐⭐⭐ | 60% |
| 2 / 5 | ⭐⭐ | 40% |
| 1 / 5 | ⭐ | 20% |
| 0 / 5 | — | 5% (consolation, always give something) |

Coin amounts come from `LessonsData.json` → `rewards.coins` for each lesson.
(e.g., Greetings = 50 coins. 5 stars = 50 coins. 3 stars = 30 coins.)

### Star Persistence (for replays)
- Always show the player's **best star count** for each lesson
- If replaying: only award coin difference (e.g., had 3★ before, now got 5★ → award 2★ worth of coins)

### No XP System
- There is no XP bar
- Progress is tracked by: lesson completion count (e.g., 1/13 lessons done)

---

## 📊 Progress & Auto-Save System

### Progress Formula
```
Activity Progress  = completedActivities / 3  (per lesson)
Lesson Progress    = completedLessons / 13    (overall)
```

### Auto-Save Trigger
- Every time an activity is completed → save to Supabase automatically
- Saves: `user_id`, `language_key`, `category_key`, `activity`, `is_completed`, `stars`, `coins_earned`

### Objectives
- `ObjectiveManager` tracks the current task in-scene
- Each activity sets an objective:
  1. Teaching done → `ObjectiveManager.SetObjective("Talk to NPCs")`
  2. NPC convos done → `ObjectiveManager.SetObjective("Complete Mini-Game")`
  3. Mini-game done → `ObjectiveManager.SetObjective("")` → lesson complete

### Completion = Unlock
- All 3 activities done → `isCompleted = true` in DB
- Next lesson row in `CategoryListManager` becomes unlocked

---

## 🗂️ File Structure

```
Assets/
  Data/
    LessonsData.json                        ← lesson metadata (title, coins, learnings)
    Minigames/
      FishingGame/
        Greetings.json                      ← L1 fishing game config
        Gratitude.json                      ← L2 (future)
        Responses.json                      ← L3 (future)
        ...
  Resources/
    LuminangPhrases.json                    ← all vocabulary (shared across all games)
  Documentation/
    Minigame_Design_Guide.md                ← all 13 minigame ideas + STT rules
    FishingGame_Design_Decisions.md         ← this file
  Scripts/
    UI/Minigames/
      Fishing Game/                         ← scripts go here (to be created)
        FishingGameManager.cs
        FishController.cs
        FishingLineController.cs
        FishingSTTPanel.cs
        FishingPauseMenu.cs
        FishingGameData.cs
```

---

## 🛑 Pause Menu

A pause button (top-left corner of screen) opens an overlay with:
- ▶ **Resume** — close pause menu, continue game
- 🔁 **Restart** — restart from Round 1, reset baits to 7
- 🏠 **Quit to Map** — return to world map via `SceneNavigationManager`

---

## 🇵🇭 Filipino Visual Theme

The fishing game scene is styled after a **Philippine coastal fishing village** to ground the game in authentic cultural context.

### Scene Background
- **Setting**: A calm Philippine coastal bay — think Ilocos Norte or Visayas coastline
- **Sky**: Warm tropical sky (morning or golden hour lighting)
- **Background layers**: Distant mountains, coconut palm trees on the shore, a small fishing village
- **Water**: Blue-green tropical water with gentle animated waves
- **Seabed**: Sandy bottom with seaweed, sea grass, and coral hints

### Boat
- Player rides a **bangka** (traditional Filipino outrigger canoe with bamboo floats)
- Includes visible outrigger arms (katig) and a simple bamboo pole
- The bangka gently bobs on the water

### Props & Details
- NPC on the dock is dressed as a **manong mangingisda** (elder fisherman)
- Dock/pier made of bamboo and weathered wood
- Background props: fish drying racks, palayok, fishing nets draped on posts
- Bait HUD icons use worm/shrimp designs

---

## 👤 Player Character — Mangingisda Rendering

The player appears in the scene as a **mangingisda (fisherman)** using a hybrid 2D/3D rendering technique to show their customized character.

### Technique: 3D Head on Headless 2D Sprite

```
     [ 👤 Player's 3D Head ]   ← RenderTexture from a dedicated head camera
     [  Headless 2D Sprite  ]  ← animated mangingisda body (idle, cast, reel)
          (on the bangka)
```

### How It Works
1. A **dedicated secondary camera** is aimed tightly at the player's 3D head (neck up)
2. The camera renders only the head to a **RenderTexture**
3. A **RawImage UI element** (with a circular/oval mask) displays the head in the scene
4. The RawImage is **parented and anchored** to the neck position of the headless 2D sprite
5. The 2D sprite plays **fishing animations** (idle bob, cast line, reel in) independently
6. A subtle **head bob** script keeps the head in sync with the body's movement

### What the Player Sees
- Their own character's face, hair, and skin tone on the mangingisda
- The body wears a **traditional Filipino fishing outfit** (salakot hat on the sprite, barong or camisa de chino)
- Feels personalized without needing full 3D animation

### Art Assets Needed
- `mangingisda_body_idle.png` — headless mangingisda body, neutral pose on bangka
- `mangingisda_body_cast.png/.anim` — casting animation frames
- `mangingisda_body_reel.png/.anim` — reeling in animation frames
- Head camera layer mask (renders only the player model, excludes background)

### Implementation Notes
- Use a **separate render layer** (e.g., `PlayerHead`) so only the player model is captured
- The head camera uses an **orthographic projection** for clean 2D-style rendering
- RenderTexture resolution: `256x256` is sufficient for the head size in-scene
- Script: `FishingPlayerHeadRenderer.cs` — handles camera setup, RenderTexture assignment, and head bob

---

## 📐 Scene Layout

```
┌─────────────────────────────────────────────┐
│ [Pause]           [🪱🪱🪱🪱🪱 Baits HUD]   │  ← HUD strip
├─────────────────────────────────────────────┤
│  🌴              ~~~ Philippine Bay ~~~  🌴 │  ← tropical backdrop
│  ┌─────────────────────────────────────┐    │
│  │   NPC Speech Bubble / Question      │    │  ← ~25% of screen height
│  │   "It's morning! How do you greet?" │    │
│  └─────────────────────────────────────┘    │
│     [Manong NPC on bamboo dock]             │
│ ~ ~ ~ ~ ~ ~ WATER SURFACE ~ ~ ~ ~ ~ ~ ~ ~  │
│        [🛶 Bangka + Mangingisda Player]     │  ← 3D head on headless 2D sprite
│                    | (fishing line)         │
│                    ● (hook)                 │
│                                             │
│  🐟[naimbag a bigat]   🐠[paalam]          │  ← fish with word signs
│        🐡[kumusta]   🦈[naimbag a rabii]   │
│                                             │
│  ≋≋≋≋≋≋≋ sandy seabed / coral / seaweed ≋ │
└─────────────────────────────────────────────┘
```

---

*Updated: August 2026*
*Next step: Build the scripts in `Assets/Scripts/UI/Minigames/Fishing Game/`*
