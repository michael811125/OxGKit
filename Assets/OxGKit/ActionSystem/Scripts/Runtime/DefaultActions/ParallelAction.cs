namespace OxGKit.ActionSystem
{
    public class ParallelAction : ActionBase
    {
        protected QueueSet<ActionBase> _queueRunningActions = new QueueSet<ActionBase>();

        #region Default Constructor
        public ParallelAction()
        {
            this.name = nameof(DelayAction);
        }

        public ParallelAction(int uid) : this()
        {
            this.uid = uid;
        }

        public ParallelAction(string name)
        {
            this.name = string.IsNullOrEmpty(name) ? nameof(ParallelAction) : name;
        }

        public ParallelAction(string name, int uid) : this(name)
        {
            this.uid = uid;
        }
        #endregion

        protected override void OnStart()
        {
            this.SetDuration(-1);

            if (this._queueRunningActions.Count == 0)
            {
                this.MarkAsDone();
                return;
            }

            this.StartParallelActions();
        }

        protected override void OnUpdate(float dt)
        {
            this.UpdateParallelActions(dt);

            if (this.IsAllDone())
            {
                this.MarkAsDone();
                this.ClearActions();
            }
        }

        public ParallelAction AddAction(ActionBase action)
        {
            if (action != null)
            {
                if (!this._queueRunningActions.Contains(action))
                {
                    this._queueRunningActions.Enqueue(action);
                }
            }
            return this;
        }

        protected void ClearActions()
        {
            this._queueRunningActions.Clear();
        }

        protected void StartParallelActions()
        {
            foreach (var action in this._queueRunningActions.ToArray())
            {
                if (action.IsDone()) return;
                action.RunStart();
            }
        }

        protected void UpdateParallelActions(float dt)
        {
            foreach (var action in this._queueRunningActions.ToArray())
            {
                if (action.IsDone()) return;
                action.RunUpdate(dt);
            }
        }

        protected bool IsAllDone()
        {
            foreach (var action in this._queueRunningActions.ToArray())
            {
                if (!action.IsDone()) return false;
            }

            return true;
        }
    }
}