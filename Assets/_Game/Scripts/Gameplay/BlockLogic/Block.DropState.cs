using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dispatcher;

namespace BlockLogic
{
    public class DropState : BaseState
    {
        private Gameplay gameplay => Gameplay.Instance;

        public DropState(Block main) : base(main) { }

        public override void EnterState()
        {
            base.EnterState();

            main.transform.parent = null;
            main.rb2d.isKinematic = false;
        }

        public override void HandleOnCollisionEnter2D(Collision2D collider)
        {
            base.HandleOnCollisionEnter2D(collider);

            var delta = main.transform.position.x - collider.transform.position.x;

            if (Mathf.Abs(delta) > 0.55f)
            {
                main.rb2d.AddForce(Vector2.right * (delta < 0 ? -1 : 1) * 300f);

                main.TransitionToState(main.ReturnPoolState);

                return;
            }

            main.TransitionToState(main.StaticState);
            if (Mathf.Abs(delta) < 0.1f)
            {
                //Play sfx
                gameplay.PlaySFX(gameplay.perfectDrop);

                //Snap to middle
                var pos = main.transform.position;
                pos.x = collider.transform.position.x;
                main.transform.position = pos;

                //Notify bonus state
                gameplay.StartBonus();
            }

            MessageSender.SendMessage(this, MessageType.DeltaSentByBlock, delta, 0f);

            if (collider.gameObject.GetComponent<Block>() == null) return;
            main.transform.parent = collider.transform;
        }
    }
}
