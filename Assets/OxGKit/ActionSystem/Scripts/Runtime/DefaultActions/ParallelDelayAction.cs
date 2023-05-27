using Cysharp.Threading.Tasks;
using System;

namespace OxGKit.ActionSystem
{
    public class ParallelDelayAction : ActionBase
    {
        protected QueueSet<ActionBase> _queueRunningActions = new QueueSet<ActionBase>();
        protected float _delayTime = 0f;

        #region Default Constructor
        public ParallelDelayAction()
        {
            this.name = nameof(DelayAction);
        }

        public ParallelDelayAction(int uid) : this()
        {
            this.uid = uid;
        }

        public ParallelDelayAction(string name)
        {
            this.name = string.IsNullOrEmpty(name) ? nameof(ParallelAction) : name;
        }

        public ParallelDelayAction(string name, int uid) : this(name)
        {
            this.uid = uid;
        }
        #endregion

        public ParallelDelayAction(float delayTime) : this()
        {
            this._delayTime = delayTime;
        }

        protected override void OnStart()
        {
            this.SetDuration(-1);

            this.StartParallelActions();
        }

        protected override void OnUpdate(float dt)
        {
            this.UpdateParallelActions(dt);

            if (this._IsAllDone())
            {
                this.MarkAsDone();
                this.ClearActions();
            }
        }

        public ParallelDelayAction AddAction(ActionBase action)
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

        protected async void StartParallelActions()
        {
            foreach (var action in this._queueRunningActions.ToArray())
            {
                action.RunStart();
                if (this._delayTime > 0f) await UniTask.Delay(TimeSpan.FromSeconds(this._delayTime));
            }
        }

        protected void UpdateParallelActions(float dt)
        {
            foreach (var action in this._queueRunningActions.ToArray())
            {
                if (action.IsAllDone()) return;
                action.RunUpdate(dt);
            }
        }

        private bool _IsAllDone()
        {
            foreach (var action in this._queueRunningActions.ToArray())
            {
                if (!action.IsAllDone()) return false;
            }

            return true;
        }
    }
}