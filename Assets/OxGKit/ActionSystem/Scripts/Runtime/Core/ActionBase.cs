namespace OxGKit.ActionSystem
{
    public abstract class ActionBase
    {
        public int uid;                          // Unique Id
        public string name = nameof(ActionBase); // Action name

        protected bool _isDone = false;          // Is action done flag
        protected bool _isAllDone = false;       // Is action all done flag (Including sub actions)
        protected bool _isStarted = false;       // Is action started flag

        protected float _duration = 0f;          // Action duration time
        protected float _timeElapsed = 0f;       // Records elapsed time

        /// <summary>
        /// Run action
        /// </summary>
        public void RunStart()
        {
            this._Init();
            this.OnStart();
        }

        private void _Init()
        {
            // Reset first
            this.Reset();
            // After mark as started
            this._isStarted = true;
        }

        public void Reset()
        {
            this._isStarted = false;
            this._isDone = false;
            this._isAllDone = false;
            this._timeElapsed = 0.0f;
            this._queueSubActions.Clear();
        }

        protected virtual void OnStart() { }
        protected virtual void OnUpdate(float dt) { }
        protected virtual void OnDone() { }

        public void RunUpdate(float dt)
        {
            if (this._isAllDone) return;

            this._timeElapsed += dt;

            // If action is not keep run update
            if (!this._isDone)
            {
                if (this.CheckAndReduceTime()) this.MarkAsDone();
                else this.OnUpdate(dt);
            }

            // Check there are any sub actions needs to do
            if (this._queueSubActions.Count > 0)
            {
                this.UpdateSubActions(dt);

                // If action is done, also sub actions are all done
                if (this._isDone && this.IsSubActionAllDone())
                {
                    this.MarkAsAllDone();
                    this.ClearSubActions();
                }
            }
        }

        public bool IsStarted()
        {
            return this._isStarted;
        }

        public bool IsDone()
        {
            return this._isAllDone;
        }

        public void MarkAsAllDone()
        {
            this.OnDone();
            this._isAllDone = true;
            this.MarkSubAllDone();
        }

        protected void MarkAsDone()
        {
            if (this._isDone) return;

            this._isDone = true;
            if (this.IsSubActionAllDone()) this.MarkAsAllDone();
        }

        /// <summary>
        /// If set duration = -1 must call MarkAsDone manually
        /// </summary>
        /// <param name="duration"></param>
        public void SetDuration(float duration)
        {
            this._duration = duration;
        }

        /// <summary>
        /// Check action duration
        /// </summary>
        /// <returns></returns>
        protected bool CheckAndReduceTime()
        {
            if (this._duration <= -1) return false;

            if (this._timeElapsed >= this._duration) return true;

            return false;
        }

        /// <summary>
        /// Get elapsed time
        /// </summary>
        /// <returns></returns>
        public float GetTimeElapsed()
        {
            return this._duration;
        }

        /// <summary>
        /// Get elapsed time ratio (0 to 1)
        /// </summary>
        /// <returns></returns>
        public float GetTimeElapsedRatio()
        {
            if (this._duration <= 0) return 0.0f;
            return (this._timeElapsed / this._duration);
        }

        #region Sub Action
        protected QueueSet<ActionBase> _queueSubActions = new QueueSet<ActionBase>();

        /// <summary>
        /// Add a sub action
        /// </summary>
        /// <param name="action"></param>
        protected ActionBase AddSubAction(ActionBase action)
        {
            if (action != null)
            {
                if (!this._queueSubActions.Contains(action))
                {
                    action.OnStart();
                    this._queueSubActions.Enqueue(action);
                }
            }

            return this;
        }

        protected void MarkSubAllDone()
        {
            foreach (var subAction in this._queueSubActions.ToArray())
            {
                if (!subAction.IsDone()) subAction.MarkAsAllDone();
            }
        }

        /// <summary>
        /// Check sub action are all done
        /// </summary>
        /// <returns></returns>
        protected bool IsSubActionAllDone()
        {
            foreach (var subAction in this._queueSubActions.ToArray())
            {
                if (!subAction.IsDone()) return false;
            }

            return true;
        }

        /// <summary>
        /// Update all sub actions
        /// </summary>
        /// <param name="dt"></param>
        protected void UpdateSubActions(float dt)
        {
            foreach (var subAction in this._queueSubActions.ToArray())
            {
                subAction.RunUpdate(dt);
            }
        }

        /// <summary>
        /// Clear sub actions
        /// </summary>
        protected void ClearSubActions()
        {
            this._queueSubActions.Clear();
        }
        #endregion
    }
}

