using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtLife;
    [SerializeField] TextMeshProUGUI txtPoint;
    [SerializeField] GameObject pnlBonus;
    [SerializeField] Image imgFill;

    public void UpdateLifeUI(int value)
    {
        txtLife.text = $"Life: {value}";
    }

    public void UpdatePointUI(int value)
    {
        txtPoint.text = $"Points: {value}";
    }

    public void ShowBonus(float time)
    {
        StopAllCoroutines();
        pnlBonus.SetActive(true);
        StartCoroutine(BonusCorou(time));
    }

    public void HideBonus()
    {
        pnlBonus.SetActive(false);
    }

    IEnumerator BonusCorou(float time)
    {
        float timer = 0;
        while (timer < time)
        {
            imgFill.fillAmount = (1f - timer / time);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        HideBonus();
    }
}
