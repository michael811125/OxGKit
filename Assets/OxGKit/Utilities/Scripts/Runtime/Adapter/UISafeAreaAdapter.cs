using OxGKit.LoggingSystem;
using UnityEngine;

namespace OxGKit.Utilities.Adapter
{
    [DisallowMultipleComponent]
    [AddComponentMenu("OxGKit/Utilities/Adapter/" + nameof(UISafeAreaAdapter))]
    public class UISafeAreaAdapter : MonoBehaviour
    {
        public bool refreshAlways = false;
        public RectTransform panel;

        private Resolution _lastResolution;

        /// <summary>
        /// Records the last applied safe area (skips redundant refreshes and per-frame log string allocations)
        /// </summary>
        private Rect _lastSafeArea;

        private void Awake()
        {
            this._lastResolution = Screen.currentResolution;
            this._InitPanel();
        }

        private void Start()
        {
            this.RefreshViewSize();
        }

        private void LateUpdate()
        {
            Resolution currentResolution = Screen.currentResolution;
            bool resolutionChanged = this._lastResolution.width != currentResolution.width ||
                                     this._lastResolution.height != currentResolution.height;
            if (this.refreshAlways || resolutionChanged)
            {
                // Only reapply when the resolution or safe area actually changed
                if (resolutionChanged || this._lastSafeArea != Screen.safeArea)
                {
                    this.RefreshViewSize();
                    this._lastResolution = currentResolution;
                }
            }
        }

        private void _InitPanel()
        {
            if (this.panel == null)
                this.panel = this.GetComponent<RectTransform>();
        }

        public void RefreshViewSize()
        {
            if (this.panel == null)
                return;

            Rect safeArea = Screen.safeArea;

            Logging.PrintInfo<Logger>($"Current Safe Area w: {safeArea.width}, h: {safeArea.height}, x: {safeArea.position.x}, y: {safeArea.position.y}");
            Logging.PrintInfo<Logger>($"Current Resolution w: {Screen.currentResolution.width}, h: {Screen.currentResolution.height}, dpi: {Screen.dpi}");

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            this.panel.anchorMin = anchorMin;
            this.panel.anchorMax = anchorMax;

            // Record the applied safe area
            this._lastSafeArea = safeArea;
        }
    }
}
