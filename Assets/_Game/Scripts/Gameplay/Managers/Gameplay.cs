using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Dispatcher;
using DG.Tweening;
using System;
using CodeStringers.UIBase;

public class Gameplay : MonoSingleton<Gameplay>
{
    public AudioClip audioDrop;
    public AudioClip audioCrash;
    public AudioClip audioOver;
    public AudioClip perfectDrop;
    [SerializeField] Block blockPrefab;
    [SerializeField] GameUI gameUI;
    [SerializeField] AudioSource bgmPlayer;

    #region  Pool
    private IObjectPool<Block> pool;
    private IObjectPool<Block> Pool
    {
        get
        {
            if (pool == null)
            {
                pool = new UnityEngine.Pool.ObjectPool<Block>(CreateBlock, OnTakeFromPool, OnReturnToPool, null, true, 10);
            }

            return pool;
        }
    }

    private Block CreateBlock()
    {
        var go = GameObject.Instantiate(blockPrefab);
        go.transform.parent = this.transform;
        return go;
    }

    private void OnTakeFromPool(Block go)
    {
        go.gameObject.SetActive(true);
    }
    private void OnReturnToPool(Block go)
    {
        go.currentState = null;
        go.transform.parent = this.transform;
        go.gameObject.SetActive(false);
    }
    #endregion

    public int NumBlock = 0;

    private int life;
    public int Life
    {
        get { return life; }
        set
        {
            life = value;
            gameUI.UpdateLifeUI(value);
        }
    }
    private int point;
    public int Point
    {
        set { point = value; gameUI.UpdatePointUI(value); }
        get { return point; }
    }

    public float BonusTime = 5f;
    public float bonusTimer { get; private set; }

    public event Action OnResetGame;

    public Block GetBlock()
    {
        return Pool.Get();
    }

    public void ReleaseBlock(Block go)
    {
        Pool.Release(go);
    }

    public void UpdateLife(int value)
    {
        Life += value;

        if (Life <= 0)
            EndGame();
    }

    public void StartBonus()
    {
        bonusTimer = 0;
        gameUI.ShowBonus(BonusTime);

        MessageSender.SendMessage(MessageType.OnBonus);
    }

    public void ResetGame()
    {
        Point = 0;
        Life = 5;
        NumBlock = 0;
        gameUI.HideBonus();

        bonusTimer = BonusTime;

        OnResetGame?.Invoke();
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    public bool IsInBonus()
    {
        return bonusTimer < BonusTime;
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlayBGM()
    {
        bgmPlayer.Play();
    }

    private void Start()
    {
        Subscribe();

        UIManager.ShowPopup(UIPopupName.PopupMainMenu);
    }

    private void Update()
    {
        if (bonusTimer < BonusTime)
            bonusTimer += Time.deltaTime;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
    private void Subscribe()
    {
        MessageDispatcher.AddListener(MessageType.OnDoneDroppingBlock, OnDoneDroppingBlock);
    }

    private void Unsubscribe()
    {
        MessageDispatcher.RemoveListener(MessageType.OnDoneDroppingBlock, OnDoneDroppingBlock);
    }

    private void OnDoneDroppingBlock(IMessage message)
    {
        var data = (Block)message.Data;
        if (data.currentState == data.StaticState) // succeeded
        {
            NumBlock++;
            Point += IsInBonus() ? 2 : 1;

            if (NumBlock < 3) return;

            //Move camera up
            var pos = this.transform.position;
            pos.y += blockPrefab.GetComponent<BoxCollider2D>().size.y;
            this.transform.DOMove(pos, 0.5f);
        }
        else
        {
            UpdateLife(-1);
        }
    }

    private void EndGame()
    {
        gameUI.HideBonus();
        bonusTimer = BonusTime;
        MessageSender.SendMessage(MessageType.OnEndGame);

        //Check entire building
        this.transform.DOMoveY(0, 3f)
        .OnComplete(() =>
        {
            UIManager.ShowPopup(UIPopupName.PopupEndGame);
        });
    }
}
