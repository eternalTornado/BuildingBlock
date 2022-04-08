using UnityEngine;
using UnityEngine.UI;

namespace CodeStringers.MessageBox {
    
    public enum MessageResult 
    {
        None, 
        Ok, 
        Cancel, 
        Yes,
        No,
        Retry,
        Ignore,
    }

    public enum ButtonState {
        Ok, 
        OkCancel,
        YesNo,
        YesNoCancel,
        RetryCancel,
    }

    public enum DefaultButton {
        Button1,
        Button2, 
        Button3,
        Button4,
        None
    }

    public delegate bool Callback(MessageResult messageResult);

    public class MessageBoxItem {
        public ButtonState ButtonState;
        public DefaultButton DefautlButton;
        public string ContentMessage;
        public string CaptionMessage;
        public Callback MessageCallback;

        public MessageBoxItem() {
            ButtonState = ButtonState.Ok;
            DefautlButton = DefaultButton.Button1;
            ContentMessage = string.Empty;
            CaptionMessage = string.Empty;
        }

        public MessageBoxItem(string contentMessage, string captionMessage, ButtonState buttonState, DefaultButton defaultButton, Callback callback = null) {
            ContentMessage = contentMessage;
            CaptionMessage = captionMessage;
            ButtonState = buttonState;
            DefautlButton = defaultButton;
        }
    }
}
