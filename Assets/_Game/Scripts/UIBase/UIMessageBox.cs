using System;
using System.Collections;
using System.Collections.Generic;
using CodeStringers.MessageBox;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.Events;

namespace CodeStringers {
    public class UIMessageBox : MonoBehaviour {
        public static Action<UIMessageBox> OnShowMessage;

        public class MessageButtonControl {
            public Transform tsButton;
            public TextMeshProUGUI buttonText;
            public MessageResult result;

            public MessageButtonControl(Transform transform, TextMeshProUGUI btnText) {
                tsButton = transform;
                buttonText = btnText;
            }

            public void ResetMessageBox() {
                result = MessageResult.None;
                buttonText.text = "";
                tsButton.gameObject.SetActive(false);
            }

            public void SetupInformation(MessageResult result, string text) {
                tsButton.gameObject.SetActive(true);
                this.result = result;
                buttonText.text = text;
            }
        }

        public static bool isShowMessage;
        public GameObject ObjContent;
        public GameObject BGObject;
        public GameObject CloseButton;
        private const string _groupBtnName = "GroupButtons"; // Same name with game object in scene 
        public bool ActiveCloseButton {
            set { CloseButton.SetActive(value); }
        }

        public TextMeshProUGUI ContentMessage;
        public TextMeshProUGUI CaptionMessage;
        //public TextMeshProUGUI ChildContentMessage;
        //Todo: Add more fields for custom message
        //---------------------

        private MessageButtonControl[] messageButton = new MessageButtonControl[4];
        private static List<MessageBoxItem> listItems = new List<MessageBoxItem>();


        // duy test action listenner
        private Action _callbackOk;
        private Action _callbackCancel;
        private Button _buttonActionOk;
        private Button _buttonActionCancel;


        private void Start() {
            ObjContent.SetActive(false);
            for (int i = 0; i < 4; i++) {
                Transform ts = BGObject.transform.Find(_groupBtnName).Find("Btn" + (i + 1));
                if (ts == null) {
                    DebugColor.Log("Missing Btn" + (i + 1), ColorName.Red);
                    continue;
                }

                var btnLabel = ts.GetChild(0).GetComponent<TextMeshProUGUI>();
                messageButton[i] = new MessageButtonControl(ts, btnLabel);
                messageButton[i].ResetMessageBox();
            }
        }

        public GameObject GetButton(int idx) {
            return messageButton[idx].tsButton.gameObject;
        }

        public void OnEscapePressed() {
            if (!isShowMessage)
                return;
            OnCloseMessageBox();
        }

        public void OnCloseMessageBox() {
            if (listItems.Count > 0)
                listItems.RemoveAt(listItems.Count - 1);
            HideMessage();
        }

        public void OnClickedBtnByName(string name) {
            int i = 0;
            switch (name) {
                case "Btn2":
                    i = 1;
                    break;
                case "Btn3":
                    i = 2;
                    break;
                case "Btn4":
                    i = 3;
                    break;
            }
            OnButtonClicked(i);
        }

        private void OnButtonClicked(int i) {
            DebugColor.Log(listItems.Count.ToString(), ColorName.Red);
            bool isClosed = true;
            DebugColor.Log((listItems[listItems.Count - 1].MessageCallback != null).ToString(), ColorName.Green);
            if (listItems[listItems.Count - 1].MessageCallback != null) {
                isClosed = listItems[listItems.Count - 1].MessageCallback(messageButton[i].result);
            }

            if (!isClosed) {
                return;
            }

            listItems.RemoveAt(listItems.Count - 1);
            if (!CheckShownMessageBox())
                HideMessage();


        }

        private bool CheckShownMessageBox() {
            if (listItems.Count > 0) {
                MessageBoxItem item = listItems[listItems.Count - 1];
                ShowMessage(item);
                return true;
            }

            return false;
        }

        private void ResetMessageBoxState() {
            for (int i = 0; i < 4; i++) {
                messageButton[i].ResetMessageBox();
            }
        }

        private void SetupButtons(MessageBoxItem item) {
            ResetMessageBoxState();
            switch (item.ButtonState) {
                case ButtonState.Ok:
                    messageButton[0].SetupInformation(MessageResult.Ok, "Okay");
                    break;
                case ButtonState.OkCancel:
                    messageButton[1].SetupInformation(MessageResult.Ok, "Okay");
                    messageButton[2].SetupInformation(MessageResult.Cancel, "Cancel");
                    break;
                case ButtonState.RetryCancel:
                    messageButton[1].SetupInformation(MessageResult.Retry, "Retry");
                    messageButton[2].SetupInformation(MessageResult.Cancel, "Cancel");
                    break;
                case ButtonState.YesNo:
                    messageButton[1].SetupInformation(MessageResult.Yes, "Yes");
                    messageButton[2].SetupInformation(MessageResult.No, "No");
                    break;
            }
        }

        private void SetupButtonsText(MessageBoxItem item, string textYes = null, string textNo = null) {
            ResetMessageBoxState();
            switch (item.ButtonState) {
                case ButtonState.Ok:
                    messageButton[0].SetupInformation(MessageResult.Ok, textYes);
                    break;
                case ButtonState.OkCancel:
                case ButtonState.RetryCancel:
                case ButtonState.YesNo:
                    messageButton[1].SetupInformation(MessageResult.Ok, textYes);
                    messageButton[2].SetupInformation(MessageResult.Cancel, textNo);
                    break;
            }
        }

        #region  MessageBox Function

        private void ShowMessage(MessageBoxItem item) {
            if (item == null) return;
            isShowMessage = true;
            ObjContent.SetActive(true);
            SetupButtons(item);
            ContentMessage.text = item.ContentMessage;
            CaptionMessage.text = item.CaptionMessage;

            BGObject.transform.Find(_groupBtnName).Find("Btn1").GetComponent<Button>().Select();
            OnShowMessage?.Invoke(this);
        }

        public void ShowMessage(string message, string caption, ButtonState buttonState, DefaultButton defaultButton, Callback callback) {
            isShowMessage = true;
            MessageBoxItem item = new MessageBoxItem {
                CaptionMessage = caption,
                ButtonState = buttonState,
                DefautlButton = defaultButton,
                MessageCallback = callback
            };
            item.ContentMessage = message;
            if (listItems.Count > 2)
                listItems.RemoveAt(0);

            listItems.Add(item);
            ShowMessage(item);
        }


        //
        //duy custom button action listenner
        //   
        private void SetButtonListener(Button buttonOk)
        {
            _buttonActionOk = buttonOk;
            _buttonActionOk.Select();
            _buttonActionOk.onClick.AddListener(OnEventButtonOK);
        }

        private void SetButtonListener(Button buttonOk, Button buttonCancel)
        {
            _buttonActionOk = buttonOk;
            _buttonActionCancel = buttonCancel;

            _buttonActionOk.Select();
            _buttonActionCancel.Select();
            _buttonActionOk.onClick.AddListener(OnEventButtonOK);
            _buttonActionCancel.onClick.AddListener(OnEventButtonCancel);
        }

        // sorry for confuse here. it will be better if the msg_box should be deleted  
        public void OnEventButtonOK()
        {
            _callbackOk?.Invoke();
            _callbackOk = null;
            _buttonActionOk.onClick.RemoveListener(OnEventButtonOK);
        }

        // i must refest when its showing
        public void OnEventButtonCancel()
        {
            _callbackCancel?.Invoke();
            _callbackCancel = null;
            _buttonActionCancel.onClick.RemoveListener(OnEventButtonCancel);
        }


        private void ShowMessageOk(MessageBoxItem item, Action callbackOk = null)
        {
            if (item == null)
                return;

            isShowMessage = true;
            ObjContent.SetActive(true);
            SetupButtons(item);

            ContentMessage.text = item.ContentMessage;
            CaptionMessage.text = item.CaptionMessage;

            var buttonOk = messageButton[0].tsButton.GetComponent<Button>();
            SetButtonListener(buttonOk);

            _callbackOk = callbackOk;
            OnShowMessage?.Invoke(this);
        }

        private void ShowMessageYesNo(MessageBoxItem item, Action callbackOk = null, Action callbackCancel = null)
        {
            if (item == null)
                return;

            isShowMessage = true;
            ObjContent.SetActive(true);
            SetupButtons(item);

            ContentMessage.text = item.ContentMessage;
            CaptionMessage.text = item.CaptionMessage;

            var buttonOk = messageButton[1].tsButton.GetComponent<Button>();
            var buttonCancel = messageButton[2].tsButton.GetComponent<Button>();
            SetButtonListener(buttonOk, buttonCancel);

            _callbackOk = callbackOk;
            _callbackCancel = callbackCancel;
            OnShowMessage?.Invoke(this);
        }

        public void ShowMessageBoxOk(string message, string caption, ButtonState buttonState, Action callbackOk = null)
        {
            isShowMessage = true;
            MessageBoxItem item = new MessageBoxItem { CaptionMessage = caption, ButtonState = buttonState, DefautlButton = DefaultButton.None };  

            if (listItems.Count > 2)
                listItems.RemoveAt(0);

            listItems.Add(item);
            ShowMessageOk(item, callbackOk);
        }

        public void ShowMessageBoxOk(string message, string caption, string textOk, ButtonState buttonState, Action callbackOk = null)
        {
            isShowMessage = true;
            MessageBoxItem item = new MessageBoxItem { CaptionMessage = caption, ButtonState = buttonState, DefautlButton = DefaultButton.None };  
            item.ContentMessage = message;

            if (listItems.Count > 2)
                listItems.RemoveAt(0);

            listItems.Add(item);
            ShowMessageOk(item,callbackOk);
            SetupButtonsText(item, textOk, "");
        }

        public void ShowMessageBoxOkCancel(string message, string caption, ButtonState buttonState, Action callbackOk = null, Action callbackCancel = null)
        {
            isShowMessage = true;
            MessageBoxItem item = new MessageBoxItem { CaptionMessage = caption, ButtonState = buttonState, DefautlButton = DefaultButton.None };  
            item.ContentMessage = message;

            if (listItems.Count > 2)
                listItems.RemoveAt(0);

            listItems.Add(item);
            ShowMessageYesNo(item, callbackOk, callbackCancel);
        }

        public void ShowMessageBoxOkCancel(string message, string caption, string textYes, string textNo, ButtonState buttonState, Action callbackOk = null, Action callbackCancel = null)
        {
            isShowMessage = true;
            MessageBoxItem item = new MessageBoxItem { CaptionMessage = caption, ButtonState = buttonState, DefautlButton = DefaultButton.None };  
            item.ContentMessage = message;

            if (listItems.Count > 2)
                listItems.RemoveAt(0);

            listItems.Add(item);
            ShowMessageYesNo(item, callbackOk, callbackCancel);
            SetupButtonsText(item, textYes, textNo);
        }


        public void HideMessage() {
            StartCoroutine(TurnMessageOff());
            ObjContent.SetActive(false);
        }

        public void HideAllMessageBox() {
            listItems.Clear();
            HideMessage();
        }

        #endregion

        IEnumerator TurnMessageOff() {
            yield return new WaitForEndOfFrame();
            isShowMessage = false;
        }
    }
}

