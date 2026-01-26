using UnityEngine;

namespace Shared.View.Ad
{
    [ExecuteInEditMode]
    public class BannerAdEditorVisualizer : MonoBehaviour
    {
        public Texture2D bannerTexture; // Assign this in the Inspector
        private const int BannerHeight = 90;

        void OnGUI()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && UnityEditor.SceneView.currentDrawingSceneView != null)
            {
                return; // Prevent rendering in the Scene View
            }

            // Get screen width
            float screenWidth = Screen.width;

            // Define the banner rectangle
            Rect bannerRect = new Rect(0, Screen.height - BannerHeight, screenWidth, BannerHeight);

            // Draw the texture if it exists
            if (bannerTexture != null)
            {
                GUI.DrawTexture(bannerRect, bannerTexture, ScaleMode.StretchToFill);
            }
            else
            {
                // Draw a placeholder background if no texture is set
                GUI.color = new Color(0.3f, 0.3f, 0.3f, 1); // Gray color
                GUI.Box(bannerRect, GUIContent.none);

                // Add placeholder text
                GUIStyle textStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 20,
                    normal = { textColor = Color.white }
                };
                GUI.Label(bannerRect, "Ad Placeholder (No Image)", textStyle);
            }
#endif
        }
    }
}