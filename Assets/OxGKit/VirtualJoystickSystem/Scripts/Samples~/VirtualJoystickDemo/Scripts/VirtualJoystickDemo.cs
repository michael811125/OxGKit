using OxGKit.VirtualJoystickSystem;
using UnityEngine;
using UnityEngine.UI;

public class VirtualJoystickDemo : MonoBehaviour
{
    public Text leftAreaTxt;
    public Text rightAreaTxt;

    public VirtualJoystick leftStick;
    public VirtualJoystick rightStick;

    private void Start()
    {
        if (this.leftStick != null)
            this.leftStick.onStickInput = this._OnLeftStickInput;
        if (this.rightStick != null)
            this.rightStick.onStickInput = this._OnRightStickInput;
    }

    private void _OnLeftStickInput(Vector2 v2)
    {
        this.leftAreaTxt.text = $"[Left] {v2:F2}";
    }

    private void _OnRightStickInput(Vector2 v2)
    {
        this.rightAreaTxt.text = $"[Right] {v2:F2}";
    }
}
