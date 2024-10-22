using OxGKit.Utilities.CursorAnim;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManagerDemo : MonoBehaviour
{
    public Text cursorVisibleTxt;
    public Text cursorLockModeTxt;

    private bool _cursorVisible = true;
    private CursorLockMode _cursorLockMode;
    private int _cursorLockCount = 0;

    private void Start()
    {
        // Initialize cursor visible = true
        this._cursorVisible = true;
        CursorManager.GetInstance().SetCursorVisible(this._cursorVisible);
        this.cursorVisibleTxt.text = $"Visible: {this._cursorVisible}";

        // Initialize cursor lock mode = none
        this._cursorLockMode = CursorLockMode.None;
        CursorManager.GetInstance().SetCursorLockState(this._cursorLockMode);
        this.cursorLockModeTxt.text = $"Lock Mode: {this._cursorLockMode}";
    }

    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            this._SwitchCursorVisible();
            this.cursorVisibleTxt.text = $"Visible: {this._cursorVisible}";
        }
        else if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            this._CycleCursorLockMode();
            this.cursorLockModeTxt.text = $"Lock Mode: {this._cursorLockMode}";
        }
    }

    private void _SwitchCursorVisible()
    {
        this._cursorVisible = !this._cursorVisible;
        CursorManager.GetInstance().SetCursorVisible(this._cursorVisible);
    }

    private void _CycleCursorLockMode()
    {
        this._cursorLockCount++;
        this._cursorLockMode = (CursorLockMode)(this._cursorLockCount % 3);
        CursorManager.GetInstance().SetCursorLockState(this._cursorLockMode);
    }

    public void SetDefaultCursor()
    {
        string stateName = "Default";
        CursorManager.GetInstance().SetCursorState(stateName);
    }

    public void SetHoverCursor()
    {
        string stateName = "Hover";
        CursorManager.GetInstance().SetCursorState(stateName);
    }

    public void SetLoadingCursor()
    {
        string stateName = "Loading";
        CursorManager.GetInstance().SetCursorState(stateName);
    }
}
