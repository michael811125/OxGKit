using OxGKit.ActionSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionDemo : MonoBehaviour
{
    private ActionRunner _seqActionRunner;
    private ActionRunner _parActionRunner;

    private void Awake()
    {
        this._seqActionRunner = new ActionRunner("SeqActionRunner");
        this._parActionRunner = new ActionRunner("ParActionRunner");
    }

    private void Update()
    {
        // Call by Main MonoBehaviour (Main Program)
        this._seqActionRunner?.OnUpdate(Time.deltaTime);
        this._parActionRunner?.OnUpdate(Time.deltaTime);

        // For demo
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            this._StartSequenceAction();
        }
        else if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            this._StartParallelAction();
        }
    }

    private void _StartSequenceAction()
    {
        /*
         * You can make your own action and combine whatever action you want.
         */

        var seqAction = new SequenceAction("Colorful Sequence Callback Actions");

        // Append action 1 into seqAction
        var callback = DelegateAction.CreateDelegateAction(() => Debug.Log("<color=#ff195f>[Seq] Hello Red</color>"), 0f);
        seqAction.AddAction(callback);

        // Append action 2 into seqAction
        callback = DelegateAction.CreateDelegateAction(() => Debug.Log("<color=#ffe719>[Seq] Hello Yellow</color>"), 1f);
        seqAction.AddAction(callback);

        // Append action 3 into seqAction
        callback = DelegateAction.CreateDelegateAction(() => Debug.Log("<color=#49ff19>[Seq] Hello Green</color>"), 2f);
        seqAction.AddAction(callback);

        // Append action 4 into seqAction
        callback = DelegateAction.CreateDelegateAction(() => Debug.Log("<color=#19e5ff>[Seq] Hello Blue</color>"), 3f);
        seqAction.AddAction(callback);

        // Use ActionRunner to start sequence actions
        this._seqActionRunner.RunAction(seqAction);
    }

    private void _StartParallelAction()
    {
        /*
         * You can make your own action and combine whatever action you want.
         */

        var parAction = new ParallelAction("Colorful Parallel Callback Actions");

        // Append action 1 into parAction
        var callback = DelegateAction.CreateDelegateAction(() => Debug.Log("<color=#ff195f>[Par] Hello Red</color>"));
        parAction.AddAction(callback);

        // Append action 2 into parAction
        callback = DelegateAction.CreateDelegateAction(() => Debug.Log("<color=#ffe719>[Par] Hello Yellow</color>"));
        parAction.AddAction(callback);

        // Append action 3 into parAction
        callback = DelegateAction.CreateDelegateAction(() => Debug.Log("<color=#49ff19>[Par] Hello Green</color>"));
        parAction.AddAction(callback);

        // Append action 4 into parAction
        callback = DelegateAction.CreateDelegateAction(() => Debug.Log("<color=#19e5ff>[Par] Hello Blue</color>"));
        parAction.AddAction(callback);

        // Use ActionRunner to start parallel actions
        this._parActionRunner.RunAction(parAction);
    }
}
