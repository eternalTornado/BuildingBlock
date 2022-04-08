using System.Collections;
using System.Collections.Generic;
using CodeStringers.UIBase;
using UnityEngine;

public class UIPopupComponent : UIPopupDepth
{
    [HideInInspector] public UIPopupName popupName;
    [HideInInspector] public UIPopupState PopupState = UIPopupState.None;
    private Animator _popupAnimator;
    private Canvas _popupCanvas;
    private UIPopupController _controller;
    public UIPopupController Controller => this._controller;
    public object Parameter { get; set; }

    private void Start()
    {
        if (_popupAnimator)
            _popupAnimator.GetComponent<Animator>();
    }

    public void OnInitPopup(UIPopupController controller)
    {
        _controller = controller;
        PopupState = UIPopupState.None;
        OnInit();
    }

    public void OnShowPopup(object obj)
    {
        if (_controller)
        {
            popupName = _controller.uiPopupName;
        }

        if (PopupState == UIPopupState.Showing || PopupState == UIPopupState.Shown)
        {
            if (PopupState == UIPopupState.Shown)
            {
                Parameter = obj;
                OnShown();
            }
            return;
        }
        Parameter = obj;
        PopupState = UIPopupState.Showing;
        OnShown();
        PopupState = UIPopupState.Shown;
        //Todo: Add callback on Open Popup here
    }

    public void OnHidePopup()
    {
        PopupState = UIPopupState.Hiding;
        StopAllCoroutines();
        OnHidden();
    }

    public void ClosePopup()
    {
        UIManager.HidePopup(popupName);
        OnHidden();
        //Todo: Add callback on Close Popup here
    }

    public virtual void OnInit() { }

    public virtual void OnShown() { }

    public virtual void OnHidden() { }
}

public enum UIPopupState
{
    None,
    Showing,
    Shown,
    Hiding,
    Hidden,
}
