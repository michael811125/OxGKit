using UnityEngine;

namespace OxGKit.ActionSystem
{
    public class SequenceAction : ActionBase
    {
        protected QueueSet<ActionBase> _queuePreparingActions = new QueueSet<ActionBase>();
        protected QueueSet<ActionBase> _queuePendingActions = new QueueSet<ActionBase>();
        protected QueueSet<ActionBase> _queueQueuingActions = new QueueSet<ActionBase>();
        protected ActionBase _currentAction = null;

        #region Default Constructor
        public SequenceAction()
        {
            this.name = nameof(SequenceAction);
        }

        public SequenceAction(int uid) : this()
        {
            this.uid = uid;
        }

        public SequenceAction(string name)
        {
            this.name = string.IsNullOrEmpty(name) ? nameof(SequenceAction) : name;
        }

        public SequenceAction(string name, int uid) : this(name)
        {
            this.uid = uid;
        }
        #endregion

        protected override void OnStart()
        {
            this.SetDuration(-1);

            if (this._queuePreparingActions.Count == 0)
            {
                this.MarkAsDone();
                return;
            }

            this.ExecutePreparingActions();
            this.ShiftPendingAction();
        }

        protected override void OnUpdate(float dt)
        {
            // Start action
            this.StartQueuedActions();

            if (this._currentAction != null)
            {
                this._currentAction.RunUpdate(dt);

                if (!this._currentAction.IsAllDone()) return;
                else this._currentAction = null;
            }

            // Mark as done if there is no any pending action 
            if (!this.HasPendingAction()) this.MarkAsDone();
            else this.ShiftPendingAction();
        }

        /// <summary>
        /// Add an action
        /// </summary>
        /// <param name="action"></param>
        public SequenceAction AddAction(ActionBase action)
        {
            if (action != null)
            {
                if (!this._queuePreparingActions.Contains(action))
                {
                    this._queuePreparingActions.Enqueue(action);
                }
            }
            return this;
        }

        /// <summary>
        /// Queue an action
        /// </summary>
        /// <param name="action"></param>
        public SequenceAction QueueAction(ActionBase action)
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
        /// Put all preparing actions into pending list
        /// </summary>
        protected void ExecutePreparingActions()
        {
            // Reset pending cache
            this._queuePendingActions.Clear();

            // Put all preparing actions into pending cache
            foreach (var action in this._queuePreparingActions.ToArray())
            {
                this._queuePendingActions.Enqueue(action);
            }

            // After claer preparing cache
            this._queuePreparingActions.Clear();
        }

        protected bool HasPendingAction()
        {
            return this._queuePendingActions.Count > 0;
        }

        protected bool HasQueuingAction()
        {
            return this._queueQueuingActions.Count > 0;
        }

        /// <summary>
        /// Shift pending action be active action
        /// </summary>
        protected void ShiftPendingAction()
        {
            if (!this.HasPendingAction()) return;

            ActionBase activeAction = this._queuePendingActions.Dequeue();
            this._currentAction = activeAction;

            if (this._currentAction != null) this._currentAction.RunStart();
        }

        protected void StartQueuedActions()
        {
            if (this._queueQueuingActions.Count == 0) return;

            // Put all queue actions into queuing cache
            foreach (var queueAction in this._queueQueuingActions.ToArray())
            {
                this._queuePendingActions.Enqueue(queueAction);
            }

            // After clear queuing cache
            this._queueQueuingActions.Clear();
        }

        public void RemoveAction(int uid)
        {
            if (this._currentAction != null && this._currentAction.uid == uid)
            {
                string name = this._currentAction.name;
                this._currentAction.MarkAllDone();
                this._currentAction = null;
                Logger.OnRemove(name, uid, this.name);
            }

            if (this.HasPendingAction())
            {
                foreach (var action in this._queuePendingActions.ToArray())
                {
                    if (action.uid == uid)
                    {
                        action.MarkAllDone();
                        this._queuePendingActions.Remove(action);
                        Logger.OnRemove(action.name, uid, this.name);
                        break;
                    }
                }
            }

            if (this.HasQueuingAction())
            {
                foreach (var action in this._queueQueuingActions.ToArray())
                {
                    if (action.uid == uid)
                    {
                        action.MarkAllDone();
                        this._queueQueuingActions.Remove(action);
                        Logger.OnRemove(action.name, uid, this.name);
                        break;
                    }
                }
            }
        }
    }
}