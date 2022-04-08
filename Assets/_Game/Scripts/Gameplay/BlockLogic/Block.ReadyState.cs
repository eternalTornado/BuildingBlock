using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BlockLogic
{
    public class ReadyState : BaseState
    {
        public ReadyState(Block main) : base(main) { }

        public override void EnterState()
        {
            base.EnterState();

            main.rb2d.isKinematic = true;

            SpawnSequence();
        }

        public override void OnRelease()
        {
            base.OnRelease();

            main.TransitionToState(main.DropState);
        }

        private Tweener tween;
        private void SpawnSequence()
        {
            float value = 0f;
            main.sprRenderer.transform.localScale = Vector3.zero;
            tween = DOTween.To(x => value = x, 0f, 1f, 0.5f);
            tween.OnUpdate(() =>
            {
                main.sprRenderer.transform.localScale = Vector3.Lerp(
                    Vector3.zero,
                    Vector3.one,
                    value
                );
            });
        }
    }
}
