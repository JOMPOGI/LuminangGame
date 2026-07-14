using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Luminang.UI.Minigames
{
    public class VoiceVisualizer : MonoBehaviour
    {
        [Header("Settings")]
        public string microphoneName;
        public int sampleCount = 64;
        public float sensitivity = 100f;
        public float smoothSpeed = 10f;
        public float minHeight = 10f;
        public float maxHeight = 100f;

        [Header("UI References")]
        public RectTransform[] bars;
        public Image micButtonImage;
        public Sprite micOnSprite;
        public Sprite micOffSprite;
        public TMPro.TMP_Text listeningStatusText;

        [Header("Text Settings")]
        public string onText = "Listening...";
        public string offText = "Tap to Speak";
        public string lockedText = "Tap a card...";

        private AudioSource audioSource;
        private float[] samples;
        private bool isListening = false;
        private bool isEnabled = false;

        private void Start()
        {
            // Setup AudioSource for Microphone
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = 0f; // Mute for feedback
            samples = new float[sampleCount];

            // Start in locked state
            SetReady(false);
        }

        public void SetReady(bool ready)
        {
            isEnabled = ready;
            
            // Handle Button Interactability
            if (micButtonImage != null)
            {
                Button btn = micButtonImage.GetComponentInParent<Button>();
                if (btn != null) btn.interactable = ready;
                
                // Gray out the icon if not ready
                micButtonImage.color = ready ? Color.white : new Color(1, 1, 1, 0.5f);
            }

            // Update Text
            if (!ready)
            {
                if (listeningStatusText != null) listeningStatusText.text = lockedText;
                StopListening(); // Force stop if it was somehow listening
            }
            else
            {
                if (listeningStatusText != null) listeningStatusText.text = offText;
            }
        }

        public void ToggleListening()
        {
            if (isListening) StopListening();
            else StartListening();
        }

        public void StartListening()
        {
            if (isListening) return;
            StartCoroutine(BeginMicrophone());
        }

        private System.Collections.IEnumerator BeginMicrophone()
        {
            if (Microphone.devices.Length > 0)
            {
                microphoneName = Microphone.devices[0];
                Debug.Log($"Attempting to start mic: {microphoneName}");

                audioSource.clip = Microphone.Start(microphoneName, true, 10, 44100);
                audioSource.loop = true;
                
                // Wait without freezing the game
                while (!(Microphone.GetPosition(microphoneName) > 0)) 
                { 
                    yield return null; 
                }

                audioSource.Play();
                isListening = true;
                
                // Update UI
                if (micButtonImage != null && micOnSprite != null) micButtonImage.sprite = micOnSprite;
                if (listeningStatusText != null) listeningStatusText.text = onText;

                Debug.Log("Microphone is now LIVE!");
            }
            else
            {
                Debug.LogError("No microphone devices found on this computer!");
            }
        }
        
        public void StopListening()
        {
            if (!isListening) return;

            Microphone.End(microphoneName);
            audioSource.Stop();
            isListening = false;

            // Update UI
            if (micButtonImage != null && micOffSprite != null) micButtonImage.sprite = micOffSprite;
            if (listeningStatusText != null) listeningStatusText.text = offText;

            // Reset bars to minimum height
            foreach (var bar in bars)
            {
                bar.sizeDelta = new Vector2(bar.sizeDelta.x, minHeight);
            }
        }

        private void Update()
        {
            if (!isListening || audioSource == null || !audioSource.isPlaying) return;

            // Get spectrum data (frequencies)
            audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);

            // Calculate average "intensity" of the low/mid frequencies
            float intensity = 0;
            for (int i = 0; i < sampleCount / 2; i++) // Look at the first half of frequencies
            {
                intensity += samples[i];
            }
            intensity /= (sampleCount / 2);
            
            // Log for debugging
            if (intensity > 0) Debug.Log($"Mic Intensity: {intensity}");

            float targetHeight = Mathf.Clamp(intensity * sensitivity, minHeight, maxHeight);

            // Update each bar with a little variation for each one
            for (int i = 0; i < bars.Length; i++)
            {
                // We add a little random "jitter" to each bar to make it look like a real wave
                float randomOffset = Random.Range(0.8f, 1.2f);
                float currentHeight = bars[i].sizeDelta.y;
                float newHeight = Mathf.Lerp(currentHeight, targetHeight * randomOffset, Time.deltaTime * smoothSpeed);
                
                bars[i].sizeDelta = new Vector2(bars[i].sizeDelta.x, newHeight);
            }
        }

        private void OnDisable()
        {
            if (Microphone.IsRecording(microphoneName))
            {
                Microphone.End(microphoneName);
            }
        }
    }
}
