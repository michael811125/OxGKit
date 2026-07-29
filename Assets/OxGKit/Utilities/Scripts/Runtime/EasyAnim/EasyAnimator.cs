using System;
using UnityEngine;

namespace OxGKit.Utilities.EasyAnim
{
    [AddComponentMenu("OxGKit/Utilities/EasyAnim/" + nameof(EasyAnimator))]
    public class EasyAnimator : EasyAnim
    {
        [SerializeField]
        protected Animator _animator = null;

        /// <summary>
        /// Cached animator parameters (Animator.parameters allocates a new array on every access)
        /// </summary>
        private AnimatorControllerParameter[] _cachedParameters = null;

        /// <summary>
        /// Records the controller of the cached parameters (re-cache when the controller changed)
        /// </summary>
        private RuntimeAnimatorController _cachedController = null;

        private void Awake()
        {
            if (this._animator == null)
                this._animator = this.GetComponent<Animator>();
        }

        public Animator GetAnimation()
        {
            return this._animator;
        }

        public override void Play(string paramName, Action animEnd)
        {
            // Set anim end callback
            this.SetAnimEnd(animEnd);

            if (this.HasAnim(paramName))
            {
                // Reset first to make sure is clear param set
                this._animator.ResetTrigger(paramName);

                // Play animation by param name
                this._animator.SetTrigger(paramName);
            }
            // If cannot found param name just call end back directly
            else
                this.AnimEnd();
        }

        public override bool HasAnim(string paramName)
        {
            // Refresh the cache only when the controller reference or parameter count changed
            if (this._cachedParameters == null ||
                this._cachedController != this._animator.runtimeAnimatorController ||
                this._cachedParameters.Length != this._animator.parameterCount)
            {
                this._cachedController = this._animator.runtimeAnimatorController;
                this._cachedParameters = this._animator.parameters;
            }

            foreach (AnimatorControllerParameter param in this._cachedParameters)
            {
                if (param.name == paramName)
                    return true;
            }
            return false;
        }
    }
}