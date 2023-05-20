using System;

namespace OxGKit.ActionSystem
{
    public class DelegateAction : ActionBase
    {
        public event Action onAction = null;

        #region Default Constructor
        public DelegateAction()
        {
            this.name = nameof(DelegateAction);
        }

        public DelegateAction(int uid) : this()
        {
            this.uid = uid;
        }

        public DelegateAction(string name)
        {
            this.name = string.IsNullOrEmpty(name) ? nameof(DelegateAction) : name;
        }

        public DelegateAction(string name, int uid) : this(name)
        {
            this.uid = uid;
        }
        #endregion

        public static ActionBase CreateDelegateAction(Action onAction, float delayTime = 0f)
        {
            if (delayTime > 0f)
            {
                SequenceAction seqAction = new SequenceAction();

                DelayAction delayAction = DelayAction.CreateDelayAction(delayTime);
                seqAction.AddAction(delayAction);

                DelegateAction delegateAction = new DelegateAction();
                delegateAction.onAction += onAction;
                delegateAction.onAction += delegateAction.MarkAsDone;
                seqAction.AddAction(delegateAction);

                return seqAction;
            }
            else
            {
                DelegateAction delegateAction = new DelegateAction();
                delegateAction.onAction += onAction;
                delegateAction.onAction += delegateAction.MarkAsDone;

                return delegateAction;
            }
        }

        protected override void OnStart()
        {
            this.SetDuration(-1);

            this.onAction?.Invoke();
            this.onAction = null;
        }
    }
}