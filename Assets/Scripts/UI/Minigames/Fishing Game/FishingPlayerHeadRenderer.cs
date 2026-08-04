using UnityEngine;
using UnityEngine.UI;

namespace Luminang.UI.Minigames
{
    [ExecuteAlways]
    public class FishingPlayerHeadRenderer : MonoBehaviour
    {
        [Header("3D Setup")]
        [Tooltip("The camera that is pointing at the 3D character's face")]
        public Camera headCamera;
        [Tooltip("The actual 3D character model in the scene")]
        public Transform player3DModel;
        [Tooltip("Offset to position the camera exactly at face level (e.g., Y = 1.5)")]
        public Vector3 faceOffset = new Vector3(0, 1.5f, 1f);

        [Header("2D Setup")]
        [Tooltip("The RawImage on your 2D boat that displays the head")]
        public RawImage headDisplayUI;
        [Tooltip("The Render Texture file you created")]
        public RenderTexture headRenderTexture;

        private void Start()
        {
            SetupCamera();
        }

        private void SetupCamera()
        {
            if (headCamera != null && headRenderTexture != null)
            {
                // Assign the render texture to the camera and the UI
                headCamera.targetTexture = headRenderTexture;
                if (headDisplayUI != null)
                {
                    headDisplayUI.texture = headRenderTexture;
                }
            }
        }

        private void LateUpdate()
        {
            // Keep the camera locked exactly on the player's face
            if (player3DModel != null && headCamera != null)
            {
                // Position the camera relative to the character's actual rotation
                // faceOffset.x = left/right, faceOffset.y = up/down, faceOffset.z = distance in front of face
                Vector3 headPosition = player3DModel.position + (player3DModel.up * faceOffset.y) + (player3DModel.right * faceOffset.x);
                
                // Place camera in front of the face
                headCamera.transform.position = headPosition + (player3DModel.forward * faceOffset.z);
                
                // Make the camera look perfectly back at the head
                headCamera.transform.LookAt(headPosition);
            }
        }
    }
}
