using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dispatcher;
using UniRx;
using DG.Tweening;

namespace BlockLogic
{
    public class StaticState : BaseState
    {
        private float totalDelta;
        private float shakeDegree;
        public StaticState(Block main) : base(main) { }

        public override void EnterState()
        {
            base.EnterState();

            main.rb2d.velocity = new Vector2(0f, main.rb2d.velocity.y);

            main.IsFounderBlock = Gameplay.Instance.NumBlock == 0;
            totalDelta = 0;
            shakeDegree = 0;

            Gameplay.Instance.PlaySFX(Gameplay.Instance.audioDrop);

            if (Gameplay.Instance.IsInBonus())
            {
                main.ShowBonus(Gameplay.Instance.BonusTime - Gameplay.Instance.bonusTimer);
            }

            MessageSender.SendMessage(this, MessageType.OnDoneDroppingBlock, main, 0f);

            MessageDispatcher.AddListener(MessageType.DeltaSentByBlock, DeltaSentByBlock);
            MessageDispatcher.AddListener(MessageType.OnEndGame, OnEndGame);
        }

        public override void ExitState()
        {
            base.ExitState();

            MessageDispatcher.RemoveListener(MessageType.DeltaSentByBlock, DeltaSentByBlock);
            MessageDispatcher.RemoveListener(MessageType.OnEndGame, OnEndGame);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (!main.IsFounderBlock) return;

            main.transform.rotation = Quaternion.Lerp(
                    Quaternion.Euler(0f, 0f, -shakeDegree),
                    Quaternion.Euler(0f, 0f, shakeDegree),
                        (Mathf.Sin(Time.time * shakeDegree) + 1) / 2f);
        }

        public override void HandleOnCollisionEnter2D(Collision2D collider)
        {
            base.HandleOnCollisionEnter2D(collider);

            //Wait for 1 frame for the collider to calculate its fate
            Observable.TimerFrame(1).Subscribe(_ =>
            {
                var otherBlock = collider.gameObject.GetComponent<Block>();
                if (otherBlock == null ||
                    otherBlock.currentState != otherBlock.StaticState)
                    return;

                main.rb2d.bodyType = RigidbodyType2D.Static;
                main.transform.DORotateQuaternion(Quaternion.identity, 0.1f);
            }).AddTo(main);
        }

        private void DeltaSentByBlock(IMessage message)
        {
            if (!main.IsFounderBlock) return;
            var delta = (float)message.Data;

            totalDelta += delta < 0.1f ? 0 : delta;
            shakeDegree = GetShakeDegreeByDelta(totalDelta);
        }

        private void OnEndGame(IMessage nessage)
        {
            totalDelta = 0;
            shakeDegree = 0;
        }

        private float GetShakeDegreeByDelta(float totalDelta)
        {
            //there used to be lots of calculating here
            shakeDegree = totalDelta;
            return Mathf.Clamp(shakeDegree, 0f, 2.5f);
        }
    }
}
