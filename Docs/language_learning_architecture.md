## 1. Speech-to-Text & Pronunciation Scoring
**Challenge**: Simple transcription often fails on regional languages (e.g., misinterpreting Ilonggo as Tagalog).
**Solution: Hybrid Scoring System**:
1.  **Constrained STT (Word Accuracy)**: Checks *what* was said (the word itself). Prevents Tagalog bias by using the target word as a prompt.
2.  **Audio Similarity (Pronunciation & Diction)**: Compares the frequency signature (MFCC) and timing of the player's voice against the Native Voice Actor.
    - **Timing Normalization (DTW)**: We use *Dynamic Time Warping* to "align" the two voices. This means if the native speaker is fast and the player is slow, the system "stretches" the audio to match them up correctly. The player is **not penalized** for being slow, as long as the sounds are correct.
    - **Pronunciation**: Are the vowels and consonants clear?
    - **Diction/Accent**: Does the rhythm and tone match the native speaker's "blueprint"?
3.  **Result**: Accuracy Score = (Word Accuracy * 0.4) + (Pronunciation & Diction Match * 0.6).

### 2. Audio Pre-processing (Handling Poor Mics)
**Problem**: Low-end mobile microphones can be noisy or quiet.
**Solution**:
- **Normalization**: Automatically adjust the volume of the recording so it's consistent every time.
- **Noise Gate**: Filter out low-level background hiss before analysis.
- **Visual Feedback**: A real-time volume bar to help the player know if they are speaking too quietly.

---

## 2. Database Schema (Supabase)
### Profiles
- `id` (UUID), `xp`, `coins`, `last_language_id`, `last_category_id`, `last_active`
- *Note: These 'last active' fields allow the player to resume exactly where they stopped.*

### Learning Data
- **Languages (Regions)**: `id`, `name`, `region`
    - *Example Mapping: 1, 'Ilokano', 'Luzon' | 2, 'Cebuano', 'Visayas' | 3, 'Maranao', 'Mindanao'*
- **Categories**: `id`, `name` (Food, Greetings, etc.)
- **Words**: `id`, `language_id`, `category_id`, `native_text`, `translation`, `audio_reference_url`

### Economy & Customization
- **Items**: `id`, `name`, `type` (e.g., 'Head', 'Torso', 'Legs'), `price`, `asset_id`
- **UserInventory**: `user_id`, `item_id`, `is_equipped`

### Progress & Journal
- **UserProgress**: `id`, `user_id`, `word_id`, `mastery_level`, `last_practiced`
- **Journal**: A collection of learned words displaying:
    - **Native Text & Translation**
    - **Visual Aid**: The image associated with the word.
    - **Pronunciation Button**: Plays the native speaker's audio clip.

---

## 3. Educational Flow (Scaffolding)
1. **Pre-test**: Diagnostic matching to assess baseline.
2. **Category Selection**: Choose a region/category from the map.
3. **Mini-Games**:
    - **Flashcards/Matching**: Build visual-to-text connection.
    - **Listening**: Internalize native pronunciation.
    - **Speaking (Primary Integration)**: Practice dictation and receive accuracy scoring.
    - **Sentence Building**: Contextualize words into full phrases.
4. **Post-test**: Verify mastery and unlock next region.

## 3. Save & Resume Logic
- **Save on Completion**: Every time a mini-game is finished or a category is unlocked, the `profiles` table is updated with the current `last_language_id` and `last_category_id`.
- **Load on Startup**: When the game starts, Unity fetches the player's profile and automatically directs them to the Map or the last active Category.

## 4. Economy & Customization System
### Coin Earning
- Players earn **Coins** based on their performance (Accuracy Score) in mini-games.
- Mastery of a full category provides a "Completion Bonus".

### 3. Character Store (Customization)
- **3D Store Interaction**: An in-game zone where players can browse items.
- **Customization Logic**:
    - **Visual Swap**: Using `SkinnedMeshRenderer` to swap clothing meshes or materials on the player character.
    - **Inventory Sync**: When an item is bought, it is added to `UserInventory` in Supabase; when equipped, `is_equipped` is toggled.

## 5. Content & Curriculum (Example Words)

These categories are designed to be relevant across all three regions (Luzon, Visayas, Mindanao).

### Category 1: Greetings & Social
| English | Ilokano (Luzon) | Cebuano (Visayas) | Maranao (Mindanao) |
| :--- | :--- | :--- | :--- |
| Good Morning | Naimbag a bigat | Maayong buntag | Mapiya kapipita |
| How are you? | Kumusta ka? | Kumusta ka? | Antona-a masosowa ka? |
| Thank you | Agyamanak | Salamat | Salamat |

### Category 2: Survival & Dining
| English | Ilokano | Cebuano | Maranao |
| :--- | :--- | :--- | :--- |
| Let's eat | Mangan tayon | Mangaon ta | Mangan tano |
| Delicious | Naimas | Lami | Mapiya i rasan |
| Water | Danum | Tubig | Ig |

### Category 3: Directions & Exploration
| English | Ilokano | Cebuano | Maranao |
| :--- | :--- | :--- | :--- |
| Where is... | Sadino ti... | Asa ang... | Anda so... |
| Left / Right | Kannigid / Kusto | Wala / Tuo | Diwang / Kanan |
| Help | Tulungandak | Tabangi ko | Kaogopan ako |

---

## 6. Voice Acting Strategy
- Record native speakers for all dialogue and word bank entries.
- Use these recordings as the reference for both the player to hear and (optionally) for audio similarity checks.

---

*This document was generated by Antigravity on 2026-03-30.*

---

## 7. Third-Party Services & Dependencies

### 🗄️ Backend — Supabase
- **Purpose**: Backend-as-a-Service (BaaS) for authentication, database, and row-level security.
- **Project URL**: `[SEE .env OR SUPABASE DASHBOARD — NOT COMMITTED TO GIT]`
- **Features Used**:
  - `Auth` — Email/password signup & login with email confirmation
  - `PostgreSQL Database` — `profiles` table storing user data (username, email, xp, coins, etc.)
  - `Row Level Security (RLS)` — Users can only read/write their own profile
  - `Database Triggers` — `handle_new_user()` auto-creates a profile row on signup
- **Dashboard**: See project owner for access credentials
- **Unity SDK**: `supabase-csharp` (installed via NuGetForUnity)

---

### 📦 Unity Package Manager

#### NuGetForUnity
- **Purpose**: Allows installing NuGet (C#/.NET) packages directly inside Unity.
- **Packages installed via NuGet**:
  - `supabase-csharp` — Supabase Unity SDK
  - `Newtonsoft.Json` — JSON serialization (required by Supabase)

#### TextMeshPro (TMP)
- **Purpose**: High-quality text rendering for all UI fields and labels.
- **Source**: Built-in Unity Package Manager

#### Unity Starter Assets (Third Person & First Person)
- **Purpose**: Provides `ThirdPersonController` and `FirstPersonController` player movement scripts.
- **Source**: Unity Asset Store

---

### 📧 Email Delivery — Gmail SMTP
- **Purpose**: Sends Supabase authentication emails (signup confirmation, password reset) to players.
- **Provider**: Gmail SMTP via Google App Password
- **SMTP Host**: `smtp.gmail.com`
- **Port**: `587`
- **Sender**: `luminang.official@gmail.com`
- **Setup**: Requires 2-Step Verification and a Google App Password (not the regular Gmail password).
- **Limitation**: Best for development and early access. For production, consider a dedicated transactional email service with a verified domain.

---

### 🔑 Authentication Flow Summary
```
Player signs up (username + email + password)
  → Supabase Auth creates user in auth.users
  → Trigger fires: handle_new_user() inserts row in public.profiles (id, username, email)
  → Gmail SMTP sends confirmation email to player
  → Player clicks confirmation link → account is active
  → Player logs in with username + password
    → LoginManager looks up email by username in profiles table
    → Supabase Auth.SignIn(email, password) is called
    → On success → redirect to MainMenuScene
```

---

### 📱 Development Tools
| Tool | Purpose |
|---|---|
| Unity 6 | Game engine |
| Supabase | Backend / Auth / Database |
| Gmail SMTP | Transactional email delivery |
| NuGetForUnity | C# package manager for Unity |
| Unity Remote | Mobile device testing without building an APK |
| Resend *(attempted)* | Email provider (replaced by Gmail SMTP due to domain restriction on free tier) |

---

*Last updated: 2026-03-31*
