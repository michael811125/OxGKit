using OxGKit.SingletonSystem;
using UnityEngine;

public class NewSingletonManagerDemo : NewSingleton<NewSingletonManagerDemo>
{
    public int number = 100;

    public NewSingletonManagerDemo()
    {
        Debug.Log($"<color=#beff83>[NewSingleton] {nameof(NewSingletonManagerDemo)} >> OnConstructor <<, instance id: {this.GetHashCode()}</color>");
    }

    ~NewSingletonManagerDemo()
    {
        Debug.Log($"<color=#ff83aa>[NewSingleton] {nameof(NewSingletonManagerDemo)} >> OnDestructor <<</color>");
    }

    public static void AddNumber()
    {
        GetInstance().number++;
        Debug.Log($"<color=#d6aaff>[NewSingleton] Current Number: <color=#fff283>{GetInstance().number}</color>, instance id: {GetInstance().GetHashCode()}</color>");
    }
}
