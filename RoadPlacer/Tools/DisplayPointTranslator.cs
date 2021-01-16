using UnityEngine;
using ColossalFramework.UI;

namespace RoadPlacer.Tools
{
    class DisplayPointTranslator : MonoBehaviour
    {
        public static Vector2 ConvertWorldPointToMousePoint(Vector3 position)
        {
            Vector2 gameResolution = GetGameResolution();
            Vector2 displayResolution = GetDisplayScreenResolution();
            Vector2 scaleFromGameToDisplay = CalculateScaleBetweenTwoVectors(gameResolution, displayResolution);
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(position);
            // Invert Y because mouse point starts at top left, screen point starts at bottom left
            Vector2 mousePoint = new Vector2(screenPoint.x, gameResolution.y - screenPoint.y);
            mousePoint.Scale(scaleFromGameToDisplay);
            return mousePoint;
        }

        public static Vector2 GetDisplayScreenResolution()
        {
            return new Vector2(Display.main.systemWidth, Display.main.systemHeight);
        }

        public static Vector2 ConvertUIPointToMousePoint(Vector3 position)
        {
            UIView view = UIView.GetAView();
            // If the point comes from the UI, then UIView's GetScreenResolution is correct
            Vector2 gameResolution = view.GetScreenResolution();
            Vector2 displayResolution = GetDisplayScreenResolution();
            Vector2 scaleFromGameToDisplay = CalculateScaleBetweenTwoVectors(gameResolution, displayResolution);
            // Invert Y because mouse point starts at top left, screen point starts at bottom left
            Vector2 mousePoint = new Vector2(position.x, position.y);
            mousePoint.Scale(scaleFromGameToDisplay);
            return mousePoint;
        }

        private static Vector2 CalculateScaleBetweenTwoVectors(Vector2 original, Vector2 target)
        {
            float xScale = target.x / original.x;
            float yScale = target.y / original.y;
            return new Vector2(xScale, yScale);
        }

        public static Vector2 GetGameResolution()
        {
            return new Vector2(Screen.width, Screen.height);
        }

        public static Vector2 CenterScreen()
        {
            return GetGameResolution() / 2f;
        }

        public static bool IsValidPoint(Vector2 originalPosition)
        {
            if (originalPosition.x < 0 || originalPosition.y < 0)
            {
                return false;
            }
            Vector2 resolution = GetDisplayScreenResolution();
            if (originalPosition.x > resolution.x || originalPosition.y > resolution.y)
            {
                return false;
            }
            return true;
        }
    }
}
