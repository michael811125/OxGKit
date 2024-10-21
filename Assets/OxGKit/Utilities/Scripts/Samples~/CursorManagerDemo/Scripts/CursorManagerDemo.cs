using OxGKit.Utilities.CursorAnim;
using UnityEngine;

public class CursorManagerDemo : MonoBehaviour
{
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
