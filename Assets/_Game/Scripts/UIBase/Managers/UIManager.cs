using System;
using CodeStringers.MessageBox;
using UnityEngine;
using CodeStringers.Framework;
using CodeStringers.SplashScreen;
using TMPro;
using UnityEngine.UI;

namespace CodeStringers.UIBase
{
    public class UIManager : ManualSingleton<UIManager>
    {
        public UIPopupManager PopupManager;
        public Camera PopupCamera;
        [HideInInspector]
        public Canvas PopupCanvas;
        [SerializeField]
        CanvasScaler _canvasScaler;

        public GameObject uipopupParent;

        public static UIMessageBox MessageBox;
        public GameObject LoadingObj;
        public UISplashScreenPlayer splashPlayer;

        public TextMeshProUGUI TextVersion; // Web GL only

        public static Action OnShowLoading;
        public static Action OnHideLoading;
        public Action OnShowSplashScreen
        {
            get { return splashPlayer.OnShowSplashScreen; }
            set { splashPlayer.OnShowSplashScreen = value; }
        }
        public Action OnHideSplashSCreen
        {
            get { return splashPlayer.OnHideSplashScreen; }
            set { splashPlayer.OnHideSplashScreen = value; }
        }
        public static Action OnTouchUI;
        public static Action<GameObject> OnFristObjectInteract;
        public static Action<GameObject> OnUIElementInteract;
        private static bool _isFristInteract;
        //Todo Show loading
        private static bool _onLoading;
        private static float _curTimeLoading;
        private const int TimeOutLoading = 60;

        ScreenOrientation _currentOrientation;

        public override void Awake()
        {
            base.Awake();
            this._currentOrientation = ScreenOrientation.Portrait;
        }

        private void Start()
        {
            if (PopupCanvas == null)
                PopupCanvas = GetComponent<Canvas>();

            if (PopupManager == null)
                PopupManager = GetComponentInChildren<UIPopupManager>();

            //Set Index Popup Manager Component
            PopupManager.transform.SetSiblingIndex(0);

            if (MessageBox == null)
            {
                var mb = transform.Find("UIMessageBox");
                if (mb != null)
                    MessageBox = mb.GetComponent<UIMessageBox>();
                else
                    DebugColor.Log("Couldn't Find UIMessageBox", ColorName.Red);
            }

            if (LoadingObj == null)
            {
                Transform ts = transform.Find("Loading");
                if (ts != null)
                    LoadingObj = ts.gameObject;
                else
                    DebugColor.Log("Couldn't Find UI Loading", ColorName.Red);
            }
            TextVersion.gameObject.SetActive(false);

#if UNITY_WEBGL && TEXT_VERSION
            TextVersion.gameObject.SetActive(true);
            TextVersion.text = "Version " + Application.version;
#endif


        }

        private void Update()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (this._currentOrientation != Screen.orientation)
            {
                float w = this._canvasScaler.referenceResolution.x;
                float h = this._canvasScaler.referenceResolution.y;
                this._currentOrientation = Screen.orientation;
                if (
                    this._currentOrientation == ScreenOrientation.Portrait
                    || this._currentOrientation == ScreenOrientation.PortraitUpsideDown
                )
                {
                    if (w > h)
                    {
                        float tmp = w;
                        w = h;
                        h = tmp;
                    }
                    this._canvasScaler.referenceResolution = new Vector2(w, h);
                }
                else
                {
                    if (w < h)
                    {
                        float tmp = w;
                        w = h;
                        h = tmp;
                    }
                    this._canvasScaler.referenceResolution = new Vector2(w, h);
                }
            }
#endif
            /*
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0)) {
                OnTouchUI?.Invoke();
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null) {
                    if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null) {
                        if (!_isFristInteract) {
                            _isFristInteract = true;
                            OnFristObjectInteract?.Invoke(EventSystem.current.currentSelectedGameObject);
                        }
                    }
                    OnUIElementInteract?.Invoke(EventSystem.current.currentSelectedGameObject);
                }
            }
#elif UNITY_ANDROID && UNITY_IOS
            if (Input.touchCount > 0) {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) {
                    OnTouchUI?.Invoke();
                    if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null) {
                        if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null) {
                            if (!_isFristInteract) {
                                _isFristInteract = true;
                                OnFristObjectInteract?.Invoke(EventSystem.current.currentSelectedGameObject);
                            }
                        }
                        OnUIElementInteract?.Invoke(EventSystem.current.currentSelectedGameObject);
                    }
                }
            }
#endif
            */
        }

        public static void ShowPopup(UIPopupName popupName, object ps = null)
        {
            if (!Instance || !Instance.PopupManager)
                return;
            Instance.PopupManager.ShowPopup(popupName, ps);
        }

        public static void HidePopup(UIPopupName popupName)
        {
            if (!Instance || !Instance.PopupManager)
                return;
            Instance.PopupManager.HidePopup(popupName);
        }

        public static void HideAllPopups()
        {
            foreach (var kvp in Instance.PopupManager.CachedPopupState)
                if (IsShowPopup(kvp.Key))
                    HidePopup(kvp.Key);
        }

        public static bool AnyPopupShown()
        {
            foreach (var kvp in Instance.PopupManager.CachedPopupState)
                if (IsShowPopup(kvp.Key))
                    return true;

            return false;
        }

        public static void DestroyPopup(UIPopupGame _gameType)
        {
            try
            {
                foreach (var kvp in Instance.PopupManager.CachedPopupState)
                {
                    UIPopupComponent component = GetPopup(kvp.Key);
                    UIPopupGame gameType = component.Controller.uIPopupGame;
                    if (gameType == _gameType)
                    {
                        Destroy(component.gameObject);
                        Instance.PopupManager.CachedPopupState.Remove(kvp.Key);
                    }
                }
            }
            catch { }
        }

        public static UIPopupComponent GetPopup(UIPopupName popupName)
        {
            return UIPopupManager.GetPopup(popupName);
        }

#if UNITY_EDITOR
        [ContextMenu("ShowMessageBox for test")]
        public void ShowMessageTest_EDITOR()
        {
            ShowMessage("Test");
        }

#endif
        public static void ShowMessage(string message)
        {
            MessageBox.ShowMessage(message, "NOTICE", ButtonState.Ok, DefaultButton.Button1, null);
            MessageBox.ActiveCloseButton = false;
        }

        public static void ShowMessage(string message, bool enableCloseButton)
        {
            MessageBox.ShowMessage(message, "NOTICE", ButtonState.Ok, DefaultButton.Button1, null);
            MessageBox.ActiveCloseButton = enableCloseButton;
        }

        public static void ShowMessage(string message, string caption)
        {
            MessageBox.ShowMessage(message, caption, ButtonState.Ok, DefaultButton.Button1, null);
            MessageBox.ActiveCloseButton = false;
        }

        public static void ShowMessage(string message, string caption, bool enableCloseButton)
        {
            MessageBox.ShowMessage(message, caption, ButtonState.Ok, DefaultButton.Button1, null);
            MessageBox.ActiveCloseButton = enableCloseButton;
        }

        public static void ShowMessage(string message, string caption, Callback callback)
        {
            MessageBox.ShowMessage(
                message,
                caption,
                ButtonState.Ok,
                DefaultButton.Button1,
                callback
            );
            MessageBox.ActiveCloseButton = false;
        }

        public static void ShowMessage(
            string message,
            string caption,
            Callback callback,
            bool enableCloseButton
        )
        {
            MessageBox.ShowMessage(
                message,
                caption,
                ButtonState.Ok,
                DefaultButton.Button1,
                callback
            );
            MessageBox.ActiveCloseButton = enableCloseButton;
        }

        public static void ShowMessage(
            string message,
            string caption,
            ButtonState buttonState,
            Callback callback
        )
        {
            MessageBox.ShowMessage(message, caption, buttonState, DefaultButton.Button1, callback);
            MessageBox.ActiveCloseButton = false;
        }

        public static void ShowMessage(
            string message,
            string caption,
            ButtonState buttonState,
            Callback callback,
            bool enableCloseButton
        )
        {
            MessageBox.ShowMessage(message, caption, buttonState, DefaultButton.Button1, callback);
            MessageBox.ActiveCloseButton = enableCloseButton;
        }


        // duy test custom button click with action listener
        public static void ShowMessageBoxOk(string message, string caption, Action callbackOK = null)
        {
            MessageBox.ShowMessageBoxOk(message, caption, ButtonState.Ok, callbackOK);
            MessageBox.ActiveCloseButton = false;
        }

        public static void ShowMessageBoxOk(string message, string caption, string textOk, Action callbackOK = null)
        {
            MessageBox.ShowMessageBoxOk(message, caption, textOk, ButtonState.Ok, callbackOK);
            MessageBox.ActiveCloseButton = false;
        }

        public static void ShowMessageBoxOkCancel(string message, string caption, Action callbackOK = null, Action callbackCancel = null)
        {
            MessageBox.ShowMessageBoxOkCancel(message, caption, ButtonState.OkCancel, callbackOK, callbackCancel);
            MessageBox.ActiveCloseButton = false;
        }

        public static void ShowMessageBoxOkCancel(string message, string caption, string textOk, string textCancel, Action callbackOK = null, Action callbackCancel = null)
        {
            MessageBox.ShowMessageBoxOkCancel(message, caption, textOk, textCancel, ButtonState.OkCancel, callbackOK, callbackCancel);
            MessageBox.ActiveCloseButton = false;
        }


        public static void HideAllMessageBox()
        {
            MessageBox.HideAllMessageBox();
        }

        public static void ShowLoading()
        {
            if (Instance.LoadingObj != null)
            {
                Instance.LoadingObj.SetActive(true);
                _onLoading = true;
                _curTimeLoading = 0f;
            }
            OnShowLoading?.Invoke();
        }

        public static void HideLoading()
        {
            if (Instance.LoadingObj != null)
            {
                Instance.LoadingObj.SetActive(false);
                _onLoading = false;
                _curTimeLoading = 0f;
            }
            OnHideLoading?.Invoke();
        }

        public static bool IsShownLoading()
        {
            return Instance.LoadingObj && Instance.LoadingObj.activeSelf;
        }

        public static bool IsShowPopup(UIPopupName popupName)
        {
            try
            {
                if (Instance.PopupManager.CachedPopupState.ContainsKey(popupName))
                {
                    if (Instance.PopupManager.CachedPopupState[popupName].isActiveAndEnabled)
                        return true;
                }
                return false;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log("phu.tran isShowPopupfail  " + e.ToString());
#endif
                return false;
            }
        }

        public static void ShowSplashScreen()
        {
            Instance.splashPlayer.ShowSplashScreen();
        }

        public static void HideSplashScreen()
        {
            Instance.splashPlayer.HideSplashScreen();
        }

        public static bool IsShowingSplashScreen()
        {
            return Instance.splashPlayer.IsShowingSplashScreen();
        }
    }
}
