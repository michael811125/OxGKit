using OxGKit.SingletonSystem;
using UnityEngine;

public class MonoSingletonManagerDemo : MonoSingleton<MonoSingletonManagerDemo>
{
    // |-------------------------------------------------------------------------------------|
    // | Note: Except Awake(), Start() and OnDestroy(), other Unity methods can be override. |
    // |-------------------------------------------------------------------------------------|

    public int number = 10;

    protected override void OnCreate()
    {
        /**
         * Do Somethings OnCreate In Here (Call by Unity.Awake)
         */

        Debug.Log($"<color=#beff83>[MonoSingleton] {nameof(MonoSingletonManagerDemo)} >> OnCreate <<, instance id: {this.GetInstanceID()}</color>");
    }

    protected override void OnStart()
    {
        /**
         * Do Somethings OnStart In Here (Call by Unity.Start)
         */

        Debug.Log($"<color=#83fffc>[MonoSingleton] {nameof(MonoSingletonManagerDemo)} >> OnStart <<, instance id: {this.GetInstanceID()}</color>");
    }

    protected override void OnRelease()
    {
        /**
         * Do Somethings OnRelease In Here (Call by Unity.OnDestroy)
         */

        Debug.Log($"<color=#ff83aa>[MonoSingleton] {nameof(MonoSingletonManagerDemo)} >> OnRelease <<, instance id: {this.GetInstanceID()}</color>");
    }

    public static void AddNumber()
    {
        GetInstance().number++;
        Debug.Log($"<color=#d6aaff>[MonoSingleton] Current Number: <color=#fff283>{GetInstance().number}</color>, instance id: {GetInstance().GetInstanceID()}</color>");
    }
}