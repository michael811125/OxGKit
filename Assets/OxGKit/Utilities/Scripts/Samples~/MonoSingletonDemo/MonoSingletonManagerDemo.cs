using OxGKit.Utilities.Singleton;
using UnityEngine;

public class MonoSingletonManagerDemo : MonoSingleton<MonoSingletonManagerDemo>
{
    // |----------------------------------------------------------------------------|
    // | Note: Except Awake() and OnDestroy(), other Unity methods can be override. |
    // |----------------------------------------------------------------------------|

    public int number = 10;

    protected override void OnCreate()
    {
        /**
         * Do Somethings OnCreate In Here (Call by Awake)
         */

        Debug.Log($"<color=#beff83>[MonoSingleton] {nameof(MonoSingletonManagerDemo)} >> OnCreate <<, instance id: {this.GetInstanceID()}</color>");
    }

    protected override void OnRelease()
    {
        /**
         * Do Somethings OnRelease In Here (Call by OnDestroy)
         */

        Debug.Log($"<color=#ff83aa>[MonoSingleton] {nameof(MonoSingletonManagerDemo)} >> OnRelease <<, instance id: {this.GetInstanceID()}</color>");
    }

    public static void AddNumber()
    {
        GetInstance(true).number++;
        Debug.Log($"<color=#83d5ff>Current Number: <color=#fff283>{GetInstance().number}</color>, instance id: {GetInstance().GetInstanceID()}</color>");
    }
}