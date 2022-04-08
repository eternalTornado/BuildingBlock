using UnityEngine;
using UnityEditor;
using UnityEngineInternal;

[CustomEditor(typeof(UIPopupController)), CanEditMultipleObjects]
public class UIPopupControllerEditor : UnityEditor.Editor
{
    private string POPUP_PREFAB_LOCATION = "Assets/_Game/Resources/" + UIPopupManager.POPUP_PREFAB_PATH;
    private const string POPUP_PARENT = "PopupParent";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var popupController = target as UIPopupController;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Open"))
        {

            if (popupController.uIPopupGame == UIPopupGame._Game)
            {
                POPUP_PREFAB_LOCATION = "Assets/_Game/Resources/" + UIPopupManager.POPUP_PREFAB_PATH;
            }
            else
            {
                POPUP_PREFAB_LOCATION = string.Format("Assets/_Game/{0}/Resources/" + UIPopupManager.POPUP_PREFAB_PATH, popupController.uIPopupGame.ToString());
            }
            var isLoad = LoadPopup(POPUP_PREFAB_LOCATION);
            if (!isLoad && popupController)
                DebugColor.Log($"Dialog {popupController.uiPopupName} Not Found ", ColorName.Red);
        }

        if (GUILayout.Button("Close"))
        {
            ClosePopup();
        }

        GUILayout.EndHorizontal();

    }

    private bool LoadPopup(string path)
    {
        var popupController = target as UIPopupController;
        var parent = PopupParent();
        if (parent && popupController)
        {
            var popup = FindPopup();
            if (!popup)
            {
                string prefabPath = path + popupController.uiPopupName + ".prefab";
                var goPopup = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
                if (goPopup)
                {
                    popup = PrefabUtility.InstantiatePrefab(goPopup) as GameObject;
                    popup.gameObject.name = popupController.uiPopupName.ToString();
                    popup.transform.SetParent(parent.transform, false);
                }
            }
            else
            {
                popup.gameObject.name = popupController.uiPopupName.ToString();
                popup.transform.SetParent(parent.transform, false);
            }

            if (popup)
            {
                Selection.activeGameObject = popup;
                return true;
            }
        }
        return false;
    }

    private void ClosePopup()
    {
        var popup = FindPopup();
        if (popup)
            DestroyImmediate(popup);
    }

    private void SavePopup() { }

    private GameObject PopupParent()
    {
        var parent = GameObject.Find(POPUP_PARENT);
        return parent;
    }

    private GameObject FindPopup()
    {
        var popupController = target as UIPopupController;
        var parent = PopupParent();
        if (parent && popupController)
        {
            var ts = parent.transform.Find(popupController.uiPopupName.ToString());
            return ts ? ts.gameObject : null;
        }
        return null;
    }

}
