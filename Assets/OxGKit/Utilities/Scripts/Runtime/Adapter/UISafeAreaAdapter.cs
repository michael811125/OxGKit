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
            if (this.refreshAlways ||
                this._lastResolution.width != Screen.currentResolution.width ||
                this._lastResolution.height != Screen.currentResolution.height)
            {
                this.RefreshViewSize();
                this._lastResolution = Screen.currentResolution;
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

            Logging.Print<Logger>($"<color=#FFFF00>Current Safe Area w: {Screen.safeArea.width}, h: {Screen.safeArea.height}, x: {Screen.safeArea.position.x}, y: {Screen.safeArea.position.y}</color>");
            Logging.Print<Logger>($"<color=#32CD32>Current Resolution w: {Screen.currentResolution.width}, h: {Screen.currentResolution.height}, dpi: {Screen.dpi}</color>");

            Vector2 anchorMin = Screen.safeArea.position;
            Vector2 anchorMax = Screen.safeArea.position + Screen.safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            this.panel.anchorMin = anchorMin;
            this.panel.anchorMax = anchorMax;
        }
    }
}
