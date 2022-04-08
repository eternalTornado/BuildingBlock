using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TestConfig : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI txtOutput;
    GameConfigManager _gameConfigManager;
    public void OnClickedTest()
    {
        var id = Int32.Parse(inputField.text);
        var config = this._gameConfigManager.ConfigTest.GetConfigById(id);

        txtOutput.text = config.desc;
    }
}
