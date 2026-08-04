using UnityEngine;

namespace Luminang.UI.Minigames
{
    public class UIBobbingEffect : MonoBehaviour
    {
        [Header("Bobbing Settings")]
        [Tooltip("How far up and down it moves (in pixels)")]
        public float bobAmount = 15f;
        
        [Tooltip("How fast it bobs up and down")]
        public float bobSpeed = 1.5f;

        [Tooltip("Add a slight rocking rotation? (Set to 0 if you only want up/down)")]
        public float tiltAmount = 2f;

        private Transform rectTransform;
        private Vector3 startPos;
        private Quaternion startRot;

        void Start()
        {
            rectTransform = GetComponent<Transform>();
            if (rectTransform != null)
            {
                startPos = rectTransform.localPosition;
                startRot = rectTransform.localRotation;
            }
        }

        void Update()
        {
            if (rectTransform == null) return;

            // Calculate the up/down movement using Sine wave
            float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            
            // Keep the current X and Z, just in case another script (like FishingPlayerController) is moving the boat horizontally!
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, newY, rectTransform.localPosition.z);

            // Add a very subtle left/right rock using Cosine wave (feels more natural combined with Sine)
            if (tiltAmount > 0)
            {
                float newZRot = startRot.eulerAngles.z + Mathf.Cos(Time.time * bobSpeed * 0.8f) * tiltAmount;
                rectTransform.localRotation = Quaternion.Euler(startRot.eulerAngles.x, startRot.eulerAngles.y, newZRot);
            }
        }
    }
}
