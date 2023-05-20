using System;

namespace OxGKit.ActionSystem
{
    public class DelayAction : ActionBase
    {
        public float delayTime;
        public Action onEndAction = null;

        #region Default Constructor
        public DelayAction()
        {
            this.name = nameof(DelayAction);
        }

        public DelayAction(int uid) : this()
        {
            this.uid = uid;
        }

        public DelayAction(string name)
        {
            this.name = string.IsNullOrEmpty(name) ? nameof(DelayAction) : name;
        }

        public DelayAction(string name, int uid) : this(name)
        {
            this.uid = uid;
        }
        #endregion

        public static DelayAction CreateDelayAction(float delayTime, Action onEndAction = null)
        {
            DelayAction action = new DelayAction();
            action.delayTime = delayTime;
            action.onEndAction = onEndAction;

            return action;
        }

        protected override void OnStart()
        {
            this.SetDuration(this.delayTime);
        }

        protected override void OnDone()
        {
            this.onEndAction?.Invoke();
            this.onEndAction = null;
        }
    }
}