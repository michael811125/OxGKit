using UnityEngine;
using UnityEngine.InputSystem;

public class MonoSingletonDemo : MonoBehaviour
{
    public bool initializeSingletonsOnAwake = false;

    private void Awake()
    {
        if (this.initializeSingletonsOnAwake)
        {
            // Or use GetInstance() to initialize
            MonoSingletonManagerDemo.InitInstance();
            NewSingletonManagerDemo.InitInstance();
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            MonoSingletonManagerDemo.AddNumber();
            NewSingletonManagerDemo.AddNumber();
        }
        else if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            MonoSingletonManagerDemo.DestroyInstance();
            NewSingletonManagerDemo.DestroyInstance();
            // Immediately trigger GC for the new singleton, use with caution, this is just for example.
            System.GC.Collect();
        }
    }
}
