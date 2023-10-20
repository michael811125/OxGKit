using UnityEngine;
using UnityEngine.InputSystem;

public class MonoSingletonDemo : MonoBehaviour
{
    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            MonoSingletonManagerDemo.AddNumber();
        }
        else if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            MonoSingletonManagerDemo.DestroyInstance(true);
        }
    }
}
