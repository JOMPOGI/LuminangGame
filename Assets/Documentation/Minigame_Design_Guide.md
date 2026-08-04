# 🎮 Luminang — Mini-Game Design Guide

> **Last Updated:** July 2026  
> **Purpose:** Reference document for all planned lesson mini-games, their mechanics, STT integration, and spaced recall rules.

---

## 📌 About Luminang

**Luminang** (to journey/travel) is a Filipino regional language learning game set in a Philippine cultural world.  
Players travel through regions and learn real local languages (**Ilokano**, **Cebuano**) by interacting with NPCs, exploring a map, and completing lessons.

### Core Loop Per Lesson
```
NPC Teaching Phase
  └── NPC teaches a word/phrase
  └── Player REPEATS it using STT (Speech-to-Text)
  └── Meaning and usage of the word is shown

NPC Conversation Phase
  └── Player talks to different NPCs around the area
  └── Player picks the correct response from multiple choices
  └── Tests comprehension in a social context

Mini-Game Phase  ← THIS DOCUMENT COVERS THIS PART
  └── A fun, themed mini-game pops up
  └── Tests active recall of the lesson vocabulary
  └── Always includes an STT moment
  └── Includes a Recall Round using words from PREVIOUS lessons
```

---

## 🔁 The Recall Rule (Applies to ALL Mini-Games from L2 onwards)

Every mini-game from **Lesson 2 onwards** must include a **Recall Round** — a short 2–3 question segment (either at the start or embedded mid-game) that pulls vocabulary from **all previously completed lessons**.

**How it works in code:**
- When loading mini-game data for Lesson N, fetch vocab from **Lessons 1 through N**
- **New lesson words** → primary targets (correct answers, main game objects)
- **Old lesson words** → distractors, wrong choices, or bonus targets

Since `CurriculumManager` already fetches by `categoryKey`, extend it to accept a **list of categoryKeys** — the current lesson + all past ones.

---

## 🎙️ STT Integration Rule (Applies to ALL Mini-Games)

Every mini-game must have **at least one STT moment** built into it. This is Luminang's core differentiator.

**STT Moment Types:**

| Type | Description |
|---|---|
| **Confirm STT** | Player taps their answer first, then SPEAKS it to confirm. |
| **Answer STT** | Player must SPEAK the answer directly with no tap. |
| **Bonus STT** | Optional spoken bonus at the end for extra coins/XP. |

**STT Flow in a mini-game:**
```
Player sees question/situation
  → Player taps their answer (or game auto-triggers STT)
  → STT mic activates with a visual pulse indicator
  → Player speaks the word/phrase
  → Game compares spoken text to expected answer
  → Correct: celebrate + proceed
  → Wrong: gentle correction shown, player tries again (max 2 tries)
```

---

## 📚 Lesson Mini-Game Breakdown

---

### CHAPTER 1 — Conversational & Social

---

#### L1 — Greetings
**🎣 Mini-Game: "Cast the Right Greeting" (Fishing Game)**

**Theme:** A riverside fishing scene. An old NPC sits on a bamboo dock.

**Gameplay:**
- Different fish swim by, each with a greeting written on them
- An NPC shouts a situation: "It's morning! What do you say?"
- Player casts their fishing line at the correct fish
- Miss it and it swims away — one more chance before it's gone
- 5 rounds, each with a different time-of-day situation

**STT Integration:**
- After catching the correct fish, the fish "talks" — player must SAY the greeting out loud (STT) to reel it in completely
- Wrong pronunciation = fish wiggles free, try again

**Recall Round:** None (first lesson)

**Win Condition:** Catch at least 4/5 fish correctly + speak them via STT

---

#### L2 — Expressions of Gratitude
**🎁 Mini-Game: "Salamat Sorter" (Gift Wrap Rush)**

**Theme:** A gift shop at a local fiesta. NPCs bring gifts one by one.

**Gameplay:**
- NPCs hand the player gifts one after another
- For each gift, player must pick the correct response phrase (thank you / sorry / excuse me) from floating ribbon options
- Correct choice wraps the gift with a ribbon animation
- Wrong choice drops and breaks the gift
- Beat the timer to wrap all gifts before the fiesta starts

**STT Integration:**
- Every 3rd gift, the ribbon options disappear — player must SPEAK the phrase directly via STT with no hint
- This is the Bonus STT moment — success adds a golden ribbon to the gift

**Recall Round (L1):**
- First gift scenario: "You arrive at the gift shop. Greet the shopkeeper!"
- Player must say or pick a greeting before the main game begins

**Win Condition:** Wrap 7/10 gifts + complete at least 1 STT moment

---

#### L3 — Responses (Yes / No / Maybe)
**🪃 Mini-Game: "Oo o Dili?" (Whack-a-Word)**

**Theme:** A town plaza. Word moles pop out of holes in the ground.

**Gameplay:**
- A situation is shown at the top: "Someone offers you food. Do you want it?"
- Response words pop out of holes rapidly (oo, dili, basin, sige, etc.)
- Player whacks ONLY the correct response word(s)
- Wrong hits = buzzer + red flash
- Gets faster each round
- 8 situations total

**STT Integration:**
- After each correct whack, a STT mic bubble appears above the mole hole
- Player must SPEAK the word before the mole goes back down (3-second window)
- Speaking it correctly scores double points

**Recall Round (L1–L2):**
- 2 situations use greeting or gratitude words as the correct answer
- "Someone says good morning to you. What do you say back?" → player whacks a greeting word

**Win Condition:** 6/8 correct + at least 4 STT confirmations

---

#### L4 — Identity Expressions
**🎤 Mini-Game: "Who Are You?" (Introductions Spotlight)**

**Theme:** A stage at a local cultural show. Crowd watching, NPC host with a mic.

**Gameplay:**
- Player character stands on a spotlight stage
- NPC host asks questions: "What's your name?", "Where are you from?"
- Answer tiles light up around the stage — player taps the correct one
- Correct answers light up the stage more; wrong answers dim the lights
- Complete the full self-introduction to get a standing ovation

**STT Integration:**
- This mini-game is primarily STT-driven — the host asks, player SPEAKS the answer
- Tap choices are a fallback (appear after 3-second delay)
- Full STT answers (no tap) earn bonus spotlight points

**Recall Round (L1–L3):**
- The host greets the player first — player must respond (L1 recall)
- At the end: host says "Salamat!" — player must respond correctly (L2 recall)

**Win Condition:** Complete full introduction + 3 STT answers

---

### CHAPTER 2 — Functional & Navigational

---

#### L5 — Requests
**🛒 Mini-Game: "Palengke Panic" (Market Rush)**

**Theme:** A crowded wet market. Vendors and customers everywhere.

**Gameplay:**
- Customer NPCs appear with thought bubbles showing what they need
- Player taps the correct request phrase card for each customer's need
- Timer counts down — more customers served = higher score
- 10 customers in 60 seconds

**STT Integration:**
- Every time player taps a phrase, they must also SPEAK it before the customer accepts it
- Customers have an impatience meter — speaking too slow = customer leaves
- Trains players to recall AND say phrases quickly (real-world simulation)

**Recall Round (L1–L4):**
- 3 customers use previous lesson phrases
- "A vendor greets you as you enter — respond!" (L1 recall)
- "You bumped into someone — what do you say?" (L2 recall)

**Win Condition:** Serve 7/10 customers + all answered with STT

---

#### L6 — Directions
**🚌 Mini-Game: "Jeepney Driver" (Route Navigation)**

**Theme:** Player drives a colorful jeepney through a Philippine town.

**Gameplay:**
- Passengers shout their destinations in English from the roadside
- A road with left/right/straight/stop turns appears ahead
- Player taps the correct directional word card at each turn
- Miss a turn = wrong stop = unhappy passenger
- 5 passengers, each with a 3-part route

**STT Integration:**
- Player must SHOUT the direction word (STT) at each turn — like calling it out like a real driver
- This is the primary input method — tap is only the fallback
- Creates a fun "driving and shouting directions" experience

**Recall Round (L1–L5):**
- A passenger asks something before boarding — player must respond with a request/greeting phrase

**Win Condition:** Deliver 4/5 passengers correctly + 10+ STT direction calls

---

#### L7 — Count
**🥭 Mini-Game: "Fruit Stand Flash" (Memory Count)**

**Theme:** A colorful fruit stand at a tiangge (local bazaar).

**Gameplay:**
- Vendor flashes a tray of fruits for 2 seconds, then covers it
- Player must say or tap the correct number word for how many fruits they saw
- Gets faster and uses larger numbers each round
- 10 rounds total

**STT Integration:**
- Player SPEAKS the number word (STT) as their primary answer
- Tap fallback appears after 3 seconds if STT is not triggered
- Bonus: Vendor asks "Pila?" (How many?) — player speaks the number

**Recall Round (L1–L6):**
- Round 1: vendor greets the player — player responds (L1)
- Mid-game: vendor asks player to "Wait!" — player must say the correct word (L5)

**Win Condition:** 8/10 correct counts + 6+ STT answers

---

### CHAPTER 3 — Grammatical Foundations

---

#### L8 — Action Verbs
**🏃 Mini-Game: "Verb Relay Race"**

**Theme:** A barangay sports relay race on a dirt track. Cheering crowd.

**Gameplay:**
- Player character is at the starting line next to NPC runners
- Referee shouts an English action (eat, sleep, go, come, drink)
- Player taps the correct Ilokano/Cebuano verb card
- Character performs the animated action and moves forward in the race
- First to complete 5 correct verb actions wins

**STT Integration:**
- Player must SPEAK the verb to trigger the character action — character won't move until STT confirmed
- Wrong pronunciation = character stumbles and loses time
- Creates a fun "shout to run" mechanic

**Recall Round (L1–L7):**
- Before the race: referee greets contestants — player responds (L1)
- After winning: player thanks the referee (L2)

**Win Condition:** Win the race (5 verbs fastest) + all STT confirmed

---

#### L9 — Linking Verbs
**🪞 Mini-Game: "Mirror Match" (Emotion Mirror)**

**Theme:** A dressing room with a large ornate mirror. NPC characters walk up to it.

**Gameplay:**
- An NPC appears with a visible emotion/state (happy, tired, hungry, etc.)
- A sentence with a blank appears: "Siya ___ malipayon."
- Player taps the correct linking verb
- The NPC reflects correctly in the mirror when right
- Wrong answers shatter the mirror temporarily

**STT Integration:**
- After tapping the verb, player must SPEAK the full sentence (not just the verb)
- "Siya malipayon." — player speaks it, STT validates it
- Correct full-sentence STT = mirror glows gold

**Recall Round (L1–L8):**
- The NPC arriving at the mirror does an action first (action verb from L8) — player must identify it

**Win Condition:** 7/10 correct mirrors + 5 full-sentence STT

---

#### L10 — Pronouns
**👥 Mini-Game: "Crowd Pointer"**

**Theme:** A fiesta crowd scene with many NPC characters visible.

**Gameplay:**
- A sentence appears with a blank pronoun: "___ dances well."
- Player taps the correct person/group in the crowd
  - One person = siya
  - Player himself = ako
  - Two+ people = sila
  - Player + others = kita / kami
- Crowd gets bigger and more complex each round

**STT Integration:**
- After tapping the group, player SPEAKS the full sentence with the correct pronoun
- If STT is correct on first try, the selected NPCs do a little dance celebration

**Recall Round (L1–L9):**
- Sentences use verbs (L8), linking verbs (L9), and familiar earlier vocab
- Forces players to use all grammar knowledge together

**Win Condition:** 8/12 correct groups + 6 full-sentence STT

---

#### L11 — Interrogatives
**🕵️ Mini-Game: "Detective Quiz" (Mystery Investigation)**

**Theme:** A mysterious old bahay na bato (Filipino stone house). Player is a detective.

**Gameplay:**
- A mystery scene is shown (e.g., a missing item from a market stall)
- Clues are on a corkboard, each locked
- To unlock each clue, player picks the correct question word (Who? What? Where? When? Why? How?)
- Each unlocked clue reveals part of the mystery
- Solve the full mystery before time runs out

**STT Integration:**
- To unlock each clue, player SPEAKS the question word and the mini-question
- "Kinsa? → Who took it?" — player speaks "Kinsa?" to unlock the clue
- The detective's magnifying glass glows when STT is confirmed

**Recall Round (L1–L10):**
- One clue requires the player to respond to a greeting from a witness NPC (L1)
- One clue requires a number question (L7 + L11 combo)

**Win Condition:** Solve the full mystery (unlock all 7 clues) + all STT confirmed

---

### CHAPTER 4 — Sentence Building

---

#### L12 — Sentence Building
**🧩 Mini-Game: "Build the Bridge" (Sentence Puzzle)**

**Theme:** A wide river with two villages on either side. NPCs need to cross.

**Gameplay:**
- Word tiles float on the water: subjects, verbs, objects
- Player drags and snaps them in the correct sentence order to build a bridge plank by plank
- Wrong order = tiles sink into the water
- Each bridge = one complete sentence
- Build 5 bridges to let all NPCs cross

**STT Integration:**
- After each bridge is built, player SPEAKS the full sentence (STT) to solidify it
- The NPC only crosses when the sentence is spoken correctly
- Wrong pronunciation = NPC hesitates, player tries again

**Recall Round (All lessons):**
- Sentences include vocab from all previous chapters
- This lesson IS the recall — it synthesizes everything learned

**Win Condition:** Build all 5 bridges + speak all sentences via STT

---

### CHAPTER 5 — Final Assessment

---

#### L13 — Final Assessment
**🏆 Mini-Game: "Festival Parade" (Full Gauntlet)**

**Theme:** A grand fiesta parade through town. Every float represents a past lesson.

**Gameplay:**
- Floats pass by one after another, each representing a lesson chapter:
  - Greetings float → fishing mechanic (L1 style)
  - Gratitude float → gift sorting (L2 style)
  - Directions float → quick direction tap (L6 style)
  - Sentence float → drag-and-build (L12 style)
- Each float throws 2–3 questions at the player
- Correct answers add confetti and fireworks to the float
- Wrong answers make the float dim

**STT Integration:**
- Every single answer in this mini-game requires STT confirmation — no exceptions
- This is the ultimate test of active speaking, not just recognition

**Win Condition:** Score 80%+ across all floats → Completion Badge ceremony

---

## 🛠️ Implementation Notes

### STT System Hook
All mini-games should call the existing STT system via `STTVoiceVisualizerAdapter` and validate using the expected phrase passed from the lesson vocabulary data.

```
Expected flow:
MinigameManager.StartMinigame()
  → Load vocab from CurriculumManager (current + past categoryKeys)
  → On STT trigger: STTManager.StartListening(expectedPhrase)
  → On result: compare to expectedPhrase (fuzzy match recommended)
  → Callback: OnSTTCorrect() / OnSTTWrong()
```

### Recall Data Loading (Pseudo-code)
```
List<string> categoryKeysToLoad = GetAllPreviousCategories(currentLessonIndex);
categoryKeysToLoad.Add(currentCategoryKey);
var allVocab = await CurriculumManager.GetMatchingPairs(categoryKeysToLoad, languageId);

foreach (var word in allVocab)
    word.isRecall = word.categoryKey != currentCategoryKey;
```

### Mini-Game Difficulty Scaling

| Lesson | STT Strictness | Timer Pressure | Recall % |
|---|---|---|---|
| L1 | Loose (1–2 syllables) | None | 0% |
| L2–L4 | Medium | Low | 20% |
| L5–L7 | Medium | Medium | 30% |
| L8–L11 | Strict (full words) | Medium-High | 40% |
| L12 | Strict (full sentences) | High | 50% |
| L13 | Strict (full sentences) | High | 100% (all review) |

---

*This document should be updated as mini-games are finalized and built.*
