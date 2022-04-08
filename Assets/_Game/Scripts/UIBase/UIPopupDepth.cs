using UnityEngine;

namespace CodeStringers.UIBase 
{
    public class UIPopupDepth : MonoBehaviour {
        public int UIDepth {
            get => transform.GetSiblingIndex();
            set => transform.SetSiblingIndex(value);
        }

        public void SetForward() {
            transform.SetSiblingIndex(UIDepth + 1);
        }

        public void SetBackward() {
            transform.SetSiblingIndex(UIDepth - 1);
        }

        public void SetFront() {
            transform.SetAsLastSibling();
        }

        public void SetBack() {
            transform.SetAsFirstSibling();
        }
    }
}

