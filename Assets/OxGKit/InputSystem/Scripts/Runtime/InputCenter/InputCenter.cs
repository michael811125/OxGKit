using OxGKit.LoggingSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OxGKit.InputSystem
{
    #region API
    public static class Inputs
    {
        /// <summary>
        /// Control Map (Input Action Asset)
        /// </summary>
        public static class CM
        {
            /// <summary>
            /// Set Control Map On or Off
            /// </summary>
            /// <typeparam name="TIInputActionCollection"></typeparam>
            /// <param name="active"></param>
            public static void SetActive<TIInputActionCollection>(bool active) where TIInputActionCollection : IInputActionCollection
            {
                InputCenter.GetInstance().SetActive<TIInputActionCollection>(active);
            }

            /// <summary>
            /// Check Control Map Active Status
            /// </summary>
            /// <typeparam name="TIInputActionCollection"></typeparam>
            /// <returns></returns>
            public static bool IsActive<TIInputActionCollection>() where TIInputActionCollection : IInputActionCollection
            {
                return InputCenter.GetInstance().IsActive<TIInputActionCollection>();
            }

            /// <summary>
            /// Register Control Map (Input Action Asset) [the control map default is enable while register]
            /// </summary>
            /// <typeparam name="TIInputActionCollection"></typeparam>
            public static void RegisterControlMap<TIInputActionCollection>() where TIInputActionCollection : IInputActionCollection, new()
            {
                InputCenter.GetInstance().RegisterControlMap<TIInputActionCollection>();
            }

            /// <summary>
            /// Get Control Map (Input Action Asset)
            /// </summary>
            /// <typeparam name="TIInputActionCollection"></typeparam>
            /// <returns></returns>
            public static TIInputActionCollection GetControlMap<TIInputActionCollection>() where TIInputActionCollection : IInputActionCollection
            {
                return InputCenter.GetInstance().GetControlMap<TIInputActionCollection>();
            }
        }

        /// <summary>
        /// Input Action
        /// </summary>
        public static class IA
        {
            /// <summary>
            /// Call by Main Monobehaviour
            /// </summary>
            /// <param name="dt"></param>
            public static void UpdateInputActions(float dt)
            {
                InputCenter.GetInstance().OnUpdateInputActions(dt);
            }

            /// <summary>
            /// Register Input Action
            /// </summary>
            /// <typeparam name="TInputAction"></typeparam>
            public static void RegisterInputAction<TInputAction>() where TInputAction : IInputAction, new()
            {
                InputCenter.GetInstance().RegisterInputAction<TInputAction>();
            }

            /// <summary>
            /// Get Input Action
            /// </summary>
            /// <typeparam name="TInputAction"></typeparam>
            /// <returns></returns>
            public static TInputAction GetInputAction<TInputAction>() where TInputAction : IInputAction
            {
                return InputCenter.GetInstance().GetInputAction<TInputAction>();
            }
        }
    }
    #endregion

    #region Wrapper
    internal class InputCenter
    {
        private struct ControlMap
        {
            public IInputActionCollection inputActionCollection;
            public bool activeSelf;

            public ControlMap(IInputActionCollection inputActionCollection)
            {
                this.inputActionCollection = inputActionCollection;
                this.inputActionCollection.Enable();
                this.activeSelf = true;
            }
        }

        private static readonly object _locker = new object();
        private static InputCenter _instance = null;
        internal static InputCenter GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    _instance = new InputCenter();
                }
            }
            return _instance;
        }

        private Dictionary<int, ControlMap> _dictControlMaps = new Dictionary<int, ControlMap>();
        private Dictionary<int, IInputAction> _dictInputActions = new Dictionary<int, IInputAction>();

        #region Control Map (Input Action Asset)
        protected bool HasControlMap(int id)
        {
            return this._dictControlMaps.ContainsKey(id);
        }

        /// <summary>
        /// Register Control Map (Input Action Asset)
        /// </summary>
        /// <typeparam name="TIInputActionCollection"></typeparam>
        public void RegisterControlMap<TIInputActionCollection>() where TIInputActionCollection : IInputActionCollection, new()
        {
            var type = typeof(TIInputActionCollection);
            var hash = type.GetHashCode();
            TIInputActionCollection @new = new TIInputActionCollection();
            this.RegisterControlMap(hash, @new);
        }

        protected void RegisterControlMap(int id, IInputActionCollection inputActionCollection)
        {
            if (this.HasControlMap(id))
            {
                Logging.Print<Logger>($"<color=#ff4cc6>[ControlMap] <{inputActionCollection.GetType().Name}> already exist.</color>");
                return;
            }
            var @new = new ControlMap(inputActionCollection);
            this._dictControlMaps.Add(id, @new);
        }

        /// <summary>
        /// Get Input Action Asset
        /// </summary>
        /// <typeparam name="TIInputActionCollection"></typeparam>
        /// <returns></returns>
        public TIInputActionCollection GetControlMap<TIInputActionCollection>() where TIInputActionCollection : IInputActionCollection
        {
            var type = typeof(TIInputActionCollection);
            var hash = type.GetHashCode();
            return GetControlMap<TIInputActionCollection>(hash);
        }

        protected TIInputActionCollection GetControlMap<TIInputActionCollection>(int id) where TIInputActionCollection : IInputActionCollection
        {
            if (!this.HasControlMap(id)) return default;
            return (TIInputActionCollection)this._dictControlMaps[id].inputActionCollection;
        }

        /// <summary>
        /// Set Control Map On or Off
        /// </summary>
        /// <typeparam name="TIInputActionCollection"></typeparam>
        /// <param name="active"></param>
        public void SetActive<TIInputActionCollection>(bool active) where TIInputActionCollection : IInputActionCollection
        {
            var type = typeof(TIInputActionCollection);
            var id = type.GetHashCode();
            if (!this.HasControlMap(id))
            {
                Logging.Print<Logger>($"<color=#ff4cc6>[ControlMap] <{type.Name}> cannot found.</color>");
                return;
            }

            var ctrlMap = this._dictControlMaps[id];
            if (active)
            {
                ctrlMap.inputActionCollection.Enable();
                ctrlMap.activeSelf = true;
            }
            else
            {
                ctrlMap.inputActionCollection.Disable();
                ctrlMap.activeSelf = false;
            }
        }

        /// <summary>
        /// Check Control Map Active Status
        /// </summary>
        /// <typeparam name="TIInputActionCollection"></typeparam>
        /// <returns></returns>
        public bool IsActive<TIInputActionCollection>() where TIInputActionCollection : IInputActionCollection
        {
            var type = typeof(TIInputActionCollection);
            var id = type.GetHashCode();
            if (!this.HasControlMap(id))
            {
                Logging.Print<Logger>($"<color=#ff4cc6>[ControlMap] <{type.Name}> cannot found.</color>");
                return false;
            }

            var ctrlMap = this._dictControlMaps[id];
            return ctrlMap.activeSelf;
        }
        #endregion

        #region Input Action
        protected bool HasInputAction(int id)
        {
            return this._dictInputActions.ContainsKey(id);
        }

        /// <summary>
        /// Register Input Action
        /// </summary>
        /// <typeparam name="TInputAction"></typeparam>
        public void RegisterInputAction<TInputAction>() where TInputAction : IInputAction, new()
        {
            var type = typeof(TInputAction);
            var hash = type.GetHashCode();
            TInputAction @new = new TInputAction();
            this.RegisterInputAction(hash, @new);
        }

        protected void RegisterInputAction(int id, IInputAction inputAction)
        {
            if (this.HasInputAction(id))
            {
                Logging.Print<Logger>($"<color=#ff604c>[InputAction] <{inputAction.GetType().Name} already exist.></color>");
                return;
            }
            inputAction.OnInit();
            this._dictInputActions.Add(id, inputAction);
        }

        /// <summary>
        /// Get Input Action
        /// </summary>
        /// <typeparam name="TInputAction"></typeparam>
        /// <returns></returns>
        public TInputAction GetInputAction<TInputAction>() where TInputAction : IInputAction
        {
            var type = typeof(TInputAction);
            var hash = type.GetHashCode();
            return GetInputAction<TInputAction>(hash);
        }

        protected TInputAction GetInputAction<TInputAction>(int id) where TInputAction : IInputAction
        {
            if (!this.HasInputAction(id)) return default;
            return (TInputAction)this._dictInputActions[id];
        }
        #endregion

        #region Other
        /// <summary>
        /// Call by Main MonoBehaviour
        /// </summary>
        /// <param name="dt"></param>
        public void OnUpdateInputActions(float dt)
        {
            foreach (var inputAction in this._dictInputActions.Values)
            {
                inputAction.OnUpdate(dt);
            }
        }
        #endregion
    }
    #endregion
}