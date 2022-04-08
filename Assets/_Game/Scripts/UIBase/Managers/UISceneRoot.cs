using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeStringers.UIBase
{
    [RequireComponent(typeof(RectTransform))]
    public class UISceneRoot : MonoBehaviour
    {
        private void Awake()
        {
            if (UIPopupManager.Instance)
                UIPopupManager.Instance.popupParent = transform.GetComponent<RectTransform>();
        }
    }
}