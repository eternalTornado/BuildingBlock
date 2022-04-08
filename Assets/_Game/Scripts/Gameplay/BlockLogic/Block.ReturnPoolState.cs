using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Dispatcher;

namespace BlockLogic
{

    public class ReturnPoolState : BaseState
    {
        private Gameplay gameplay => Gameplay.Instance;
        public ReturnPoolState(Block main) : base(main) { }

        public override void EnterState()
        {
            base.EnterState();

            Gameplay.Instance.PlaySFX(Gameplay.Instance.audioCrash);

            Observable.Timer(System.TimeSpan.FromSeconds(1.5f)).Subscribe(_ =>
            {
                MessageSender.SendMessage(this, MessageType.OnDoneDroppingBlock, main, 0f);
                gameplay.ReleaseBlock(main);
            }).AddTo(gameplay);
        }
    }
}
