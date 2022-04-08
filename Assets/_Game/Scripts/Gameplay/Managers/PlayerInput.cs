using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoSingleton<PlayerInput>
{
    public Action OnScreenTap;

    public void TriggerOnScreenTap()
    {
        OnScreenTap?.Invoke();
    }
}
