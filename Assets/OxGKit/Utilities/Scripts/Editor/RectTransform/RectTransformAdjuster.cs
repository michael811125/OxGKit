using UnityEngine;
using UnityEditor;

namespace OxGKit.Utilities.RectTrans.Editor
{
    public class RectTransformAdjuster
    {
        [MenuItem("GameObject/Adjust RectTransform Anchors (Shift+R) #r")]
        public static void AdjustAnchors()
        {
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                var rectTransform = gameObject.GetComponent<RectTransform>();
                if (rectTransform == null) continue;
                // Record RectTransform (allow undo)
                Undo.RecordObject(rectTransform, $"Custom Anchors {gameObject.name}");
                _AdjustRectTransform(gameObject);
            }
        }

        private static void _AdjustRectTransform(GameObject gameObject)
        {
            RectTransform transform = gameObject.GetComponent<RectTransform>();
            if (transform == null || transform.parent == null)
            {
                return;
            }

            Bounds parentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform.parent);

            Vector2 parentSize = new Vector2(parentBounds.size.x, parentBounds.size.y);
            // convert anchor ration in to pixel position
            Vector2 posMin = new Vector2(parentSize.x * transform.anchorMin.x, parentSize.y * transform.anchorMin.y);
            Vector2 posMax = new Vector2(parentSize.x * transform.anchorMax.x, parentSize.y * transform.anchorMax.y);

            // add offset
            posMin = posMin + transform.offsetMin;
            posMax = posMax + transform.offsetMax;

            // convert from pixel position to anchor ratio again
            posMin = new Vector2(posMin.x / parentBounds.size.x, posMin.y / parentBounds.size.y);
            posMax = new Vector2(posMax.x / parentBounds.size.x, posMax.y / parentBounds.size.y);

            transform.anchorMin = posMin;
            transform.anchorMax = posMax;

            transform.offsetMin = Vector2.zero;
            transform.offsetMax = Vector2.zero;
        }
    }
}