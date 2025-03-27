using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OxGKit.ButtonSystem
{
    public class InputFieldPlaceholderRemover : MonoBehaviour
    {
        private InputField _inputField;
        private TMP_InputField _tmpInputField;

        private void Awake()
        {
            if (this._inputField == null)
                this._inputField = this.gameObject.GetComponent<InputField>();
            if (this._tmpInputField == null)
                this._tmpInputField = this.gameObject.GetComponent<TMP_InputField>();
        }

        private void Start()
        {
            // TMP InputField
            if (this._tmpInputField != null)
            {
                this._tmpInputField.onSelect.AddListener((e) =>
                {
                    this._tmpInputField.placeholder.gameObject.SetActive(false);
                });

                this._tmpInputField.onDeselect.AddListener((e) =>
                {
                    this._tmpInputField.placeholder.gameObject.SetActive(true);
                });
            }
        }

        private void Update()
        {
            // Legacy InputField
            if (this._inputField != null)
            {
                if (this._inputField.isFocused && this._inputField.placeholder.gameObject.activeSelf)
                {
                    this._inputField.placeholder.gameObject.SetActive(false);
                }
                else if (!this._inputField.isFocused && !this._inputField.placeholder.gameObject.activeSelf)
                {
                    this._inputField.placeholder.gameObject.SetActive(true);
                }
            }
        }
    }
}