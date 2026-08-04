using UnityEngine;

namespace Luminang.UI.Minigames
{
    public class UIWaterReflection : MonoBehaviour
    {
        [Header("Target to Follow")]
        [Tooltip("The real PlayerGroup that this reflection should follow")]
        public Transform targetToFollow;

        [Header("Ripple Settings")]
        public float rippleSpeed = 2f;
        public float rippleWidth = 10f; // How much it distorts left/right
        
        [Header("Vertical Mirroring")]
        [Tooltip("If the real boat bobs UP by 10, the reflection should bob DOWN by 10 to keep the water line intact.")]
        public float waterLineY = -200f; // The Y position where the reflection touches the real boat

        private RectTransform rectTransform;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (rectTransform == null || targetToFollow == null) return;

            // 1. Follow the X position of the real boat exactly
            float targetX = targetToFollow.localPosition.x;

            // 2. Mirror the Y position. If the boat goes up, reflection goes down.
            // Distance from the water line:
            float diffY = targetToFollow.localPosition.y - waterLineY;
            float targetY = waterLineY - diffY;

            // 3. Add the water ripple distortion (sine wave on X axis based on Y position and time)
            // We use localPosition.y to make the bottom of the reflection sway differently than the top
            float rippleOffset = Mathf.Sin(Time.time * rippleSpeed) * rippleWidth;

            // Apply the final position
            rectTransform.localPosition = new Vector3(targetX + rippleOffset, targetY, targetToFollow.localPosition.z);

            // 4. Mirror the rotation (if the boat tilts right, reflection tilts left)
            Vector3 targetRot = targetToFollow.localEulerAngles;
            rectTransform.localRotation = Quaternion.Euler(targetRot.x, targetRot.y, -targetRot.z);
        }
    }
}
