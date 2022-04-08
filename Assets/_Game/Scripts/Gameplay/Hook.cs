using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dispatcher;
using UniRx;

public class Hook : MonoBehaviour
{
    public float rotateSpeed = 10f;

    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] Transform spawnBlockPos;

    private Gameplay gamePlay => Gameplay.Instance;
    private PlayerInput playerInput => PlayerInput.Instance;
    private Block currentBlock;

    private void Start()
    {
        playerInput.OnScreenTap += ReleaseBlock;
        gamePlay.OnResetGame += OnResetGame;
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        MessageDispatcher.AddListener(MessageType.OnDoneDroppingBlock, OnDoneDroppingBlock);
        MessageDispatcher.AddListener(MessageType.OnEndGame, OnEndGame);
    }

    private void Unsubscribe()
    {
        MessageDispatcher.RemoveListener(MessageType.OnDoneDroppingBlock, OnDoneDroppingBlock);
        MessageDispatcher.RemoveListener(MessageType.OnEndGame, OnEndGame);
    }

    private void StartMoving()
    {
        RotateRight();
        CircleDownPeak();

        SpawnBlock();
    }

    private void OnDoneDroppingBlock(IMessage message)
    {
        //Hanlde 1 frame later to check for game ending
        Observable.TimerFrame(1).Subscribe(_ =>
        {
            if (gamePlay.Life > 0)
                SpawnBlock();
        }).AddTo(this);

    }

    private void OnEndGame(IMessage message)
    {
        tween?.Kill();
        tweenUpDown?.Kill();
    }

    private void SpawnBlock()
    {
        currentBlock = gamePlay.GetBlock();
        currentBlock.transform.parent = this.transform;
        currentBlock.transform.localPosition = spawnBlockPos.transform.localPosition;
        currentBlock.transform.localRotation = Quaternion.identity;

        currentBlock.TransitionToState(currentBlock.ReadyState);
    }

    private void ReleaseBlock()
    {
        if (currentBlock == null)
        {
            Debug.LogError("Release khi block null ne");
            return;
        }

        if (currentBlock.currentState != currentBlock.ReadyState)
        {
            Debug.LogError("block not ready. current state is: " + currentBlock.currentState.ToString());
            return;
        }

        currentBlock?.Release();
    }

    private void OnResetGame()
    {
        StartMoving();
    }

    #region Rotate
    private Tweener tween;
    private Tweener tweenUpDown;

    private void RotateRight()
    {
        float value = 0f;
        tween = DOTween.To(x => value = x, 0, 1f, 2f).SetEase(Ease.InOutQuad);
        tween.OnUpdate(() =>
        {
            this.transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(new Vector3(0f, 0f, -20f)),
                Quaternion.Euler(new Vector3(0f, 0f, 20f)),
                value
            );
        })
        .OnStart(() =>
        {
            CircleDownPeak();
        })
        .OnComplete(() =>
        {
            RotateLeft();
        });
    }
    private void RotateLeft()
    {
        float value = 1f;
        tween = DOTween.To(x => value = x, 1f, 0f, 2f).SetEase(Ease.InOutQuad);
        tween.OnUpdate(() =>
        {
            this.transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(new Vector3(0f, 0f, -20f)),
                Quaternion.Euler(new Vector3(0f, 0f, 20f)),
                value
            );
        })
        .OnStart(() =>
        {
            CircleUpPeak();
        })
        .OnComplete(() =>
        {
            RotateRight();
        });
    }

    private void CircleDownPeak()
    {
        float value = 0f;
        tweenUpDown = DOTween.To(x => value = x, 0f, 1f, 1f);
        tweenUpDown.OnUpdate(() =>
        {
            this.transform.localPosition =
            Vector3.Lerp(new Vector3(0f, 6f, 0f),
            new Vector3(0f, 5.5f), value);
        })
        .OnComplete(() =>
        {
            CircleDownBack();
        });
    }

    private void CircleDownBack()
    {
        float value = 1f;
        tweenUpDown = DOTween.To(x => value = x, 1f, 0f, 1f);
        tweenUpDown.OnUpdate(() =>
        {
            this.transform.localPosition =
            Vector3.Lerp(new Vector3(0f, 6f, 0f),
            new Vector3(0f, 5.5f), value);
        });
    }

    private void CircleUpPeak()
    {
        float value = 0f;
        tweenUpDown = DOTween.To(x => value = x, 0f, 1f, 1f);
        tweenUpDown.OnUpdate(() =>
        {
            this.transform.localPosition =
            Vector3.Lerp(new Vector3(0f, 6f, 0f),
            new Vector3(0f, 6.5f), value);
        })
        .OnComplete(() =>
        {
            CircleUpBack();
        });
    }

    private void CircleUpBack()
    {
        float value = 1f;
        tweenUpDown = DOTween.To(x => value = x, 1f, 0f, 1f);
        tweenUpDown.OnUpdate(() =>
        {
            this.transform.localPosition =
            Vector3.Lerp(new Vector3(0f, 6f, 0f),
            new Vector3(0f, 6.5f), value);
        });
    }
    #endregion
}
