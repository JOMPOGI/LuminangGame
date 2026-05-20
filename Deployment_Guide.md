# Luminang Game - Cloud Deployment Guide

This guide details how to deploy your Python NLP backend server to a cloud hosting platform (like **Render.com** or **Railway.app**) so it can be accessed by your deployed **Unity Android APK** (or iOS/PC builds) over the internet.

---

## Prerequisite Files Already Created

I have created these configuration files in your `Backend/` folder to make the server deployment-ready:
1. **[requirements.txt](file:///c:/Users/Asus/Desktop/LuminangGame/Backend/requirements.txt)**: Specifies all required Python libraries (compiled for lightweight CPU-only operations to keep cloud costs free/minimal).
2. **[Dockerfile](file:///c:/Users/Asus/Desktop/LuminangGame/Backend/Dockerfile)**: Installs `ffmpeg` (needed for Whisper audio transcription) and prepares a lightweight container.
3. **[LuminangPhrases.json](file:///c:/Users/Asus/Desktop/LuminangGame/Backend/LuminangPhrases.json)**: Copied directly into the backend folder so the server can run standalone.

---

## Step 1: Push Your Backend to GitHub

Cloud hosting platforms pull your code directly from GitHub.

1. Open your terminal/git bash.
2. Initialize and push your code (you can push just the `Backend` directory or the whole project). If pushing just the `Backend` folder:
   ```bash
   cd c:/Users/Asus/Desktop/LuminangGame/Backend
   git init
   git add .
   git commit -m "Initialize NLP backend server"
   # Create a new repository on GitHub, then link it:
   git branch -M main
   git remote add origin https://github.com/YOUR_USERNAME/luminang-backend.git
   git push -u origin main
   ```

---

## Step 2: Deploy to Render.com (Free Tier Available)

**Render** is one of the easiest platforms to run Docker-based FastAPI servers:

1. Sign up for a free account at [Render.com](https://render.com).
2. Click **New +** in the dashboard and select **Web Service**.
3. Connect your GitHub account and select your `luminang-backend` repository.
4. Configure the Web Service:
   * **Name**: `luminang-nlp-backend` (or any name you prefer).
   * **Region**: Choose the region closest to your target players (e.g., Singapore/Oregon).
   * **Runtime**: Select **Docker** (Render will automatically detect the `Dockerfile` we created).
   * **Instance Type**: Select **Free** (or Starter if you want faster responses without cold starts).
5. Click **Create Web Service**.

Render will now pull your repository, build the Docker container (installing Whisper and E5 embeddings), and start the server. Once finished, you will see a public URL at the top left of your Render dashboard (e.g., `https://luminang-nlp-backend.onrender.com`).

> [!NOTE]
> On the **Free Tier**, Render puts the service to "sleep" after 15 minutes of inactivity. The first request after it sleeps can take 50–90 seconds to wake up (due to loading the Sentence Transformer model). For production, upgrading to Render's **Starter Tier** ($7/month) keeps it active 24/7.

---

## Step 3: Update Unity C# URL

Once your cloud service is live, update the endpoint URL in your Unity codebase:

1. Open **[PhraseEvaluator.cs](file:///c:/Users/Asus/Desktop/LuminangGame/Assets/Scripts/Speech/PhraseEvaluator.cs)**.
2. Change the `BACKEND_URL` constant from `localhost` to your new Render URL:

```csharp
// Replace this:
private const string BACKEND_URL = "http://localhost:8000";

// With your Render Web Service URL (make sure it starts with https://):
private const string BACKEND_URL = "https://luminang-nlp-backend.onrender.com";
```

3. Save the script.

---

## Step 4: Build Your APK!

1. Open Unity $\rightarrow$ **File** $\rightarrow$ **Build Settings**.
2. Select **Android** and click **Switch Platform**.
3. Set your player options (package name, etc.) and click **Build**.
4. Install the generated APK on your Android device.

Your mobile game will now communicate seamlessly with your cloud NLP server!
