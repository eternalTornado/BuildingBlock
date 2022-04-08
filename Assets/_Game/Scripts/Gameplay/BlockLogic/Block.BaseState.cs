using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BlockLogic
{
    public abstract class BaseState
    {
        public Block main;

        public Action OnEnterState;
        public Action OnExitState;

        public BaseState(Block _main)
        {
            main = _main;
        }

        public virtual void EnterState()
        {
            OnEnterState?.Invoke();
        }
        public virtual void ExitState()
        {
            OnExitState?.Invoke();
        }

        public virtual void OnFixedUpdate() { }

        public virtual void OnRelease() { }
        public virtual void HandleOnCollisionEnter2D(Collision2D collider) { }
    }
}

