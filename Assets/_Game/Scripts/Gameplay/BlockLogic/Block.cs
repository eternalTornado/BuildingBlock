using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using BlockLogic;
using Dispatcher;

public class Block : MonoBehaviour
{
    public Sprite sprNormal;
    public Sprite sprHighlight;
    public AudioClip audioDrop;
    public AudioClip audioCrash;
    public SpriteRenderer sprRenderer;
    public Rigidbody2D rb2d;
    public GameObject particle;

    public bool IsFounderBlock;

    private Gameplay gameplay => Gameplay.Instance;

    private void Awake()
    {
        InitStates();
    }

    private void OnEnable()
    {
        gameplay.OnResetGame += OnResetGame;

        particle.SetActive(false);

        MessageDispatcher.AddListener(MessageType.OnBonus, OnBonus);
    }

    private void OnDisable()
    {
        gameplay.OnResetGame -= OnResetGame;

        MessageDispatcher.RemoveListener(MessageType.OnBonus, OnBonus);
    }

    private void FixedUpdate()
    {
        currentState?.OnFixedUpdate();
    }

    public void Release()
    {
        currentState.OnRelease();
    }

    public void HideBonus()
    {
        particle.SetActive(false);
    }

    public void ShowBonus(float time)
    {
        StopAllCoroutines();
        particle.SetActive(true);
        StartCoroutine(BonusCorou(time));
    }

    private IEnumerator BonusCorou(float time)
    {
        float timer = 0;
        while (timer < time)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        particle.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        currentState.HandleOnCollisionEnter2D(collider);
    }

    private void OnBonus(IMessage message)
    {
        ShowBonus(gameplay.BonusTime);
    }

    private void OnResetGame()
    {
        gameplay.ReleaseBlock(this);
    }

    #region State

    public BaseState ReadyState;
    public BaseState DropState;
    public BaseState StaticState;
    public BaseState HighlightState;
    public BaseState FallState;
    public BaseState ReturnPoolState;

    public BaseState currentState;

    private void InitStates()
    {
        ReadyState = new ReadyState(this);
        DropState = new DropState(this);
        StaticState = new StaticState(this);
        HighlightState = new HighlightState(this);
        FallState = new FallState(this);
        ReturnPoolState = new ReturnPoolState(this);
    }

    public void TransitionToState(BaseState newState)
    {
        if (newState == null)
        {
            Debug.LogError("New state null ne");
            return;
        }
        currentState?.ExitState();

        currentState = newState;
        currentState.EnterState();
    }

    #endregion
}
