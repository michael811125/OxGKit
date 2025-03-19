using System;
using UnityEngine;

namespace OxGKit.Utilities.EasyAnim
{
    [AddComponentMenu("OxGKit/Utilities/EasyAnim/" + nameof(EasyAnimation))]
    public class EasyAnimation : EasyAnim
    {
        [SerializeField]
        protected Animation _animation = null;

        private void Awake()
        {
            if (this._animation == null)
                this._animation = this.GetComponent<Animation>();
        }

        public Animation GetAnimation()
        {
            return this._animation;
        }

        public override void Play(string animName, Action animEnd)
        {
            // Set anim end callback
            this.SetAnimEnd(animEnd);

            if (this.HasAnim(animName))
            {
                // Play animation by anim name
                this._animation.Play(animName);
            }
            // If cannot found anim name just call end back directly
            else
                this.AnimEnd();
        }

        public override bool HasAnim(string animName)
        {
            return this._animation.GetClip(animName) != null;
        }
    }
}