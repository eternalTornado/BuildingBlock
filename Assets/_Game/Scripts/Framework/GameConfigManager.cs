using UnityEngine;
public interface IGameConfigManager
{
    ConfigTest ConfigTest { get; }
}
public class GameConfigManager : IGameConfigManager
{
    public string CONF_PATH = "Configs/";

    #region CONFIGS

    private ConfigTest _configTest;
    public ConfigTest ConfigTest
    {
        get
        {
            if (this._configTest != null) return this._configTest;
            else
            {
                this._configTest = new ConfigTest();
                _configTest.LoadFromAssetPath(CONF_PATH + "ConfigTest");
                return this._configTest;
            }
        }
    }

    #endregion


}
