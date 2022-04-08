using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMainMenu : UIPopupComponent
{
    public void OnClickedPlay()
    {
        Gameplay.Instance.ResetGame();
        this.ClosePopup();
    }
}
