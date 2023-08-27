using System.Collections.Generic;

namespace OxGKit.ActionSystem
{
    public class ActionRunner
    {
        public string name = nameof(ActionRunner);
        protected QueueSet<ActionBase> _queueRunningActions; // Running actions
        protected QueueSet<ActionBase> _queueQueuingActions; // Queuing actions
        protected List<ActionBase> _listDoneActions;         // Done actions

        public ActionRunner()
        {
            this._queueRunningActions = new QueueSet<ActionBase>();
            this._queueQueuingActions = new QueueSet<ActionBase>();
            this._listDoneActions = new List<ActionBase>();
        }

        public ActionRunner(string name) : this()
        {
            this.name = name;
        }

        /// <summary>
        /// Call by Main MonoBehaviour (Main Program)
        /// </summary>
        /// <param name="dt"></param>
        public void OnUpdate(float dt)
        {
            // Clear done list per frame
            this._listDoneActions.Clear();

            // Keep update and check if action is done will put into done list
            if (this._queueRunningActions.Count > 0)
            {
                foreach (var action in this._queueRunningActions.ToArray())
                {
                    if (!action.IsStarted()) continue;

                    action.RunUpdate(dt);

                    if (action.IsAllDone()) this._listDoneActions.Add(action);
                }
            }

            // Remove done action from running cache
            if (this._listDoneActions.Count > 0)
            {
                foreach (var action in this._listDoneActions)
                {
                    this._queueRunningActions.Remove(action);
                    Reporter.OnDone(action.name, this.name);
                }
            }

            // Start queuing action
            this._StartQueuedActions();
        }

        private void _Reset()
        {
            this._queueRunningActions.Clear();
            this._queueQueuingActions.Clear();
            this._listDoneActions.Clear();
        }

        private void _StartAction(ActionBase action)
        {
            if (action == null) return;

            action.RunStart();
            this._queueRunningActions.Enqueue(action);
        }

        private void _StartQueuedActions()
        {
            if (this._queueQueuingActions.Count == 0) return;

            // Put all queuing actions into running cache
            foreach (var queueAction in this._queueQueuingActions.ToArray())
            {
                // Start and enqueue
                this._StartAction(queueAction);
            }

            // After clear queuing cache
            this._queueQueuingActions.Clear();
        }

        #region Public Methods
        /// <summary>
        /// Run an action
        /// </summary>
        /// <param name="action"></param>
        public void RunAction(ActionBase action)
        {
            if (action == null) return;

            this._Reset();
            this._StartAction(action);
        }

        /// <summary>
        /// Queue an action
        /// </summary>
        /// <param name="action"></param>
        public ActionRunner QueueAction(ActionBase action)
        {
            if (action != null)
            {
                if (!this._queueQueuingActions.Contains(action))
                {
                    this._queueQueuingActions.Enqueue(action);
                }
            }
            return this;
        }

        /// <summary>
        /// Remove an action by UID
        /// </summary>
        /// <param name="uid"></param>
        public void RemoveAction(int uid)
        {
            foreach (var action in this._queueRunningActions.ToArray())
            {
                if (action.uid == uid)
                {
                    action.MarkAllDone();
                    this._queueRunningActions.Remove(action);
                    Reporter.OnRemove(action.name, uid, this.name);
                    break;
                }
            }

            foreach (var action in this._queueQueuingActions.ToArray())
            {
                if (action.uid == uid)
                {
                    action.MarkAllDone();
                    this._queueQueuingActions.Remove(action);
                    Reporter.OnRemove(action.name, uid, this.name);
                    break;
                }
            }
        }

        /// <summary>
        /// Clear all actions
        /// </summary>
        public void Release()
        {
            this._Reset();
        }
        #endregion
    }
}
