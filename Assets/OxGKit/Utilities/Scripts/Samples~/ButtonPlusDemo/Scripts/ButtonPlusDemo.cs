using UnityEngine;

public class ButtonPlusDemo : MonoBehaviour
{
    public void OnClickEvent()
    {
        Debug.Log("<color=#94ff3e>ButtonPlus - <color=#ffd63e>Click</color></color>");
    }

    public void OnLongClickPressedEvent()
    {
        Debug.Log("<color=#94ff3e>ButtonPlus - <color=#3edbff>Long Click Pressed</color></color>");
    }

    public void OnLongClickReleasedEvent()
    {
        Debug.Log("<color=#94ff3e>ButtonPlus - <color=#ff5eb1>Long Click Released</color></color>");
    }
}
