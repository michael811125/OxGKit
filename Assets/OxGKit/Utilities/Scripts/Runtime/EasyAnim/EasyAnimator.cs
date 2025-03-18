using System;
using UnityEngine;

namespace OxGKit.Utilities.EasyAnim
{
    public class EasyAnimator : EasyAnim
    {
        [SerializeField]
        protected Animator _animator = null;

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
            foreach (AnimatorControllerParameter param in this._animator.parameters)
            {
                if (param.name == paramName)
                    return true;
            }
            return false;
        }
    }
}