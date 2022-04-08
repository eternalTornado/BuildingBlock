using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupEndGame : UIPopupComponent
{
    [SerializeField] TextMeshProUGUI txtPoint;
    [SerializeField] TextMeshProUGUI txtHeight;

    private Gameplay gameplay => Gameplay.Instance;
    public override void OnShown()
    {
        base.OnShown();

        gameplay.PlaySFX(gameplay.audioOver);

        txtPoint.text = $"Points: {gameplay.Point}";
        txtHeight.text = $"Max Height: {gameplay.NumBlock}";

        gameplay.StopBGM();
    }

    public void OnClickedOk()
    {
        gameplay.PlayBGM();
        gameplay.ResetGame();
        this.ClosePopup();
    }
}
