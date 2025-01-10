using OxGKit.Utilities.CursorAnim;
using UnityEngine;
using static OxGKit.Utilities.CursorAnim.CursorManager;

public static class Cursors
{
    /// <summary>
    /// Initialize the CursorManager instance
    /// </summary>
    public static void InitInstance()
    {
        CursorManager.GetInstance();
    }

    /// <summary>
    /// Sets whether to ignore the scale
    /// </summary>
    /// <param name="ignore"></param>
    public static void SetIgnoreScale(bool ignore)
    {
        CursorManager.GetInstance().SetIgnoreScale(ignore);
    }

    /// <summary>
    /// Gets the current cursor lock state
    /// </summary>
    /// <returns></returns>
    public static CursorLockMode GetCurrentCursorLockState()
    {
        return CursorManager.GetInstance().GetCurrentCursorLockState();
    }

    /// <summary>
    /// Sets the cursor lock state
    /// </summary>
    /// <param name="cursorLockMode"></param>
    public static void SetCursorLockState(CursorLockMode cursorLockMode)
    {
        CursorManager.GetInstance().SetCursorLockState(cursorLockMode);
    }

    /// <summary>
    /// Checks if the cursor is visible
    /// </summary>
    /// <returns></returns>
    public static bool IsCursorVisible()
    {
        return CursorManager.GetInstance().IsCursorVisible();
    }

    /// <summary>
    /// Sets the cursor visibility
    /// </summary>
    /// <param name="visible"></param>
    public static void SetCursorVisible(bool visible)
    {
        CursorManager.GetInstance().SetCursorVisible(visible);
    }

    /// <summary>
    /// Sets the scale for all cursor states and resets the rendering
    /// </summary>
    /// <param name="scale"></param>
    public static void SetScaleToAllCursors(Vector2 scale)
    {
        CursorManager.GetInstance().SetScaleToAllCursors(scale);
    }

    /// <summary>
    /// Gets all cursor states
    /// </summary>
    /// <returns></returns>
    public static CursorState[] GetAllCursorStates()
    {
        return CursorManager.GetInstance().GetAllCursorStates();
    }

    /// <summary>
    /// Gets the cursor state corresponding to the given state name
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public static CursorState GetCursorState(string stateName)
    {
        return CursorManager.GetInstance().GetCursorState(stateName);
    }

    /// <summary>
    /// Gets the current cursor state
    /// </summary>
    /// <returns></returns>
    public static CursorState GetCurrentCursorState()
    {
        return CursorManager.GetInstance().GetCurrentCursorState();
    }

    /// <summary>
    /// Sets the cursor state by name
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public static bool SetCursorState(string stateName)
    {
        return CursorManager.GetInstance().SetCursorState(stateName);
    }

    /// <summary>
    /// Resets the cursor state to the default (first element)
    /// </summary>
    public static void ResetCursorState()
    {
        CursorManager.GetInstance().ResetCursorState();
    }

    /// <summary>
    /// Resets the cursor rendering
    /// </summary>
    public static void ResetRender()
    {
        CursorManager.GetInstance().ResetRender();
    }

    /// <summary>
    /// Removes the cursor rendering
    /// <para>If you want to restore visibility, you can use the ResetCursorState method to initialize and reset the rendering</para>
    /// </summary>
    public static void RemoveCursorRender()
    {
        CursorManager.GetInstance().RemoveCursorRender();
    }
}