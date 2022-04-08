using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupController : MonoBehaviour
{

    public UIPopupGame uIPopupGame = UIPopupGame._Game;
    public UIPopupName uiPopupName = UIPopupName.None;
    private UIPopupManager _popupManager;
    private float hideTime = 0.1f;
    public UIPopupComponent UIPopupComponent
    {
        get
        {
            if (_popupManager && _popupManager.CachedPopupState.ContainsKey(uiPopupName))
                return _popupManager.CachedPopupState[uiPopupName];
            return null;
        }
        set
        {
            if (_popupManager)
            {
                if (_popupManager.CachedPopupState.ContainsKey(uiPopupName))
                    _popupManager.CachedPopupState[uiPopupName] = value;
                else
                    _popupManager.CachedPopupState.Add(uiPopupName, value);
            }
        }
    }
    public void InitController(UIPopupManager popupManager, UIPopupName popupName)
    {
        uiPopupName = popupName;
        _popupManager = popupManager;
    }

    public void ShowPopup(UIPopupName popupName, object ps = null)
    {

        uiPopupName = popupName;
        if (UIPopupComponent && UIPopupComponent.PopupState != UIPopupState.None &&
            UIPopupComponent.PopupState != UIPopupState.Hidden)
        {
            UIPopupComponent.SetFront();
            UIPopupComponent.OnShown();
            return;
        }
        StartCoroutine(ShowPopupCoroutine(ps));
    }

    public void HidePopup()
    {
        UIPopupComponent popup = UIPopupComponent;
        if (popup == null ||
            popup.PopupState == UIPopupState.Hiding ||
            popup.PopupState == UIPopupState.Hidden ||
            popup.PopupState == UIPopupState.Showing)
            return;
        var animator = popup.GetComponent<Animator>();
        if (animator)
        {
            //Todo: Setup Animation here
            //--------------------------
            Invoke("OnHidePopupCompleted", hideTime);
        }
        else
        {
            OnHidePopupCompleted();
        }
    }

    private IEnumerator ShowPopupCoroutine(object ps)
    {
        if (UIPopupComponent == null)
        {
            UIPopupComponent = UIPopupManager.GetPopup(uiPopupName);
            if (UIPopupComponent)
            {
                //Todo: Run Animation here
                //------------------------

                UIPopupComponent.OnInitPopup(this);
                UIPopupComponent.gameObject.SetActive(true);
            }
        }
        if (UIPopupComponent == null)
            yield break;
        if (UIPopupComponent.PopupState != UIPopupState.Showing || UIPopupComponent.PopupState != UIPopupState.Shown)
        {
            //Todo: Run Animation here
            //------------------------
            UIPopupComponent.UIDepth = _popupManager.FrontDepth + 1;
            UIPopupComponent.gameObject.SetActive(true);
            UIPopupComponent.OnShowPopup(ps);
        }
    }

    private void OnHidePopupCompleted()
    {
        if (UIPopupComponent == null || UIPopupComponent.gameObject == null) return;
        UIPopupComponent popup = UIPopupComponent;
        popup.PopupState = UIPopupState.Hidden;
        UIPopupComponent.gameObject.SetActive(false);

    }
}
