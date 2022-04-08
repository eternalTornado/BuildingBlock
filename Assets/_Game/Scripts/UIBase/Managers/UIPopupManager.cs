using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CodeStringers.Framework;

public class UIPopupManager : ManualSingleton<UIPopupManager>
{
    public const string POPUP_PREFAB_PATH = "Popups/";
    private static Dictionary<UIPopupName, UIPopupComponent> _cachedPopup;
    private Dictionary<UIPopupName, UIPopupController> _cachedController;
    private Dictionary<UIPopupName, UIPopupComponent> _cachedPopupState;
    private Action<UIPopupName> OnShowPopup;
    private Action<UIPopupName> OnClosePopup;
    public static Dictionary<UIPopupName, UIPopupComponent> CachedPopup => _cachedPopup ??= new Dictionary<UIPopupName, UIPopupComponent>();
    public Dictionary<UIPopupName, UIPopupController> CachedController =>
        _cachedController ??= new Dictionary<UIPopupName, UIPopupController>();

    public Dictionary<UIPopupName, UIPopupComponent> CachedPopupState =>
        _cachedPopupState ??= new Dictionary<UIPopupName, UIPopupComponent>();

    public RectTransform popupParent;
    public RectTransform defaultParent;


    public int FrontDepth
    {
        get
        {
            if (this.popupParent == null) this.popupParent = defaultParent;
            int depth = 0;
            foreach (RectTransform rt in popupParent)
            {
                if (!rt.gameObject.activeSelf) continue;
                // var popup = rt.GetComponent<UIPopupComponent>();
                //Todo: Check Always On Top here
                //------------------------------
                if (depth > rt.GetSiblingIndex())
                    continue;
                depth = rt.GetSiblingIndex();
            }
            return depth;
        }
    }

    public void ShowPopup(UIPopupName popupName, object ps = null)
    {
        if (Instance.CachedController.ContainsKey(popupName) && Instance.CachedController[popupName] != null)
        {
            var controller = Instance.CachedController[popupName];
            controller.ShowPopup(popupName, ps);
            OnShowPopup?.Invoke(popupName);
        }
        else
        {
            DebugColor.Log("Cannot Show " + popupName, ColorName.Red);
        }
    }

    public void HidePopup(UIPopupName popupName)
    {
        if (Instance.CachedController.ContainsKey(popupName) && Instance.CachedController[popupName] != null)
        {
            var controller = Instance.CachedController[popupName];
            controller.HidePopup();
            //var keepPopupBg = false;
            int highestDepth = 0;
            if (this.popupParent == null) this.popupParent = defaultParent;
            for (int i = popupParent.childCount - 1; i >= 0; i--)
            {
                var ts = popupParent.GetChild(i);
                if (controller.UIPopupComponent && controller.UIPopupComponent.transform.Equals(ts))
                    continue;
                if (!ts.gameObject.activeSelf)
                    continue;

                var otherPopup = ts.GetComponent(typeof(UIPopupComponent)) as UIPopupComponent;
                if (otherPopup)
                {
                    if (otherPopup.PopupState == UIPopupState.Hiding)
                        continue;

                    //keepPopupBg = true;
                    if (highestDepth <= otherPopup.UIDepth)
                        highestDepth = otherPopup.UIDepth;
                }
            }
            OnClosePopup?.Invoke(popupName);

        }
    }

    public static UIPopupComponent GetPopup(UIPopupName popupName)
    {
        if (CachedPopup.ContainsKey(popupName) && CachedPopup[popupName] != null)
        {
            return CachedPopup[popupName];
        }
        var popupObject = CreatePopup(popupName);
        if (popupObject != null)
        {
            Dictionary<UIPopupName, UIPopupComponent> cached = CachedPopup;
            if (cached.ContainsKey(popupName))
                cached[popupName] = popupObject;
            else
                cached.Add(popupName, popupObject);
        }
        return popupObject;
    }

    public static bool IsPopupActive(UIPopupName popupName)
    {
        if (!CachedPopup.ContainsKey(popupName) || CachedPopup[popupName] == null) return false;

        return GetPopup(popupName).isActiveAndEnabled;
    }

    private void Start()
    {
        InitController();
    }

    private void InitController()
    {
        var popupsController = transform.GetComponentsInChildren<UIPopupController>();
        foreach (var controller in popupsController)
        {
            controller.InitController(this, controller.uiPopupName);
            CachedController.Add(controller.uiPopupName, controller);
        }
    }

    private static UIPopupComponent CreatePopup(UIPopupName popupName)
    {
        var goPopup = Resources.Load<GameObject>(POPUP_PREFAB_PATH + popupName);
        if (goPopup == null)
        {
            DebugColor.Log(POPUP_PREFAB_PATH + popupName + " NOT FOUND, Please Create New Prefab ", ColorName.Red);
            return null;
        }

        goPopup = Instantiate(goPopup);
        var popup = goPopup.GetComponent<UIPopupComponent>();
        if (popup == null)
        {
            Destroy(goPopup);
            DebugColor.Log("Please Add UIPopupComponent To This Popup", ColorName.Red);
            return null;
        }
        else
        {
            if (Instance.popupParent == null) Instance.popupParent = Instance.defaultParent;
            popup.transform.SetParent(Instance.popupParent, false);
        }

        return popup;
    }

}
