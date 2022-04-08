using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeStringers.Utils
{
    public static class CSGameObjectUtils
    {

        public static void ClearAllChild(Transform parent, bool unloadUnusedAsset = false)
        {
            var allChild = (from Transform child in parent select child.gameObject).ToList();
            parent.transform.DetachChildren();
            if (Application.isEditor)
                allChild.ForEach(Object.DestroyImmediate);
            else
                allChild.ForEach(Object.Destroy);

            if (unloadUnusedAsset)
                Resources.UnloadUnusedAssets();
        }

        public static void SetActiveAllChild(Transform parent, bool isActive)
        {
            for (var i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive(isActive);
            }
        }

        public static GameObject AddChild(Transform parent, string name = "ChildGO")
        {
            var c = new GameObject();
            c.name = name;
            c.transform.SetParent(parent.transform);
            c.transform.localPosition = Vector3.zero;
            c.transform.localScale = Vector3.one;
            c.transform.localEulerAngles = Vector3.zero;
            return c;
        }

        public static Component FindComponentInParent(Transform tsChild, Type type)
        {
            var t = tsChild.parent;
            if (t == null)
                return null;
            if (t.GetComponent(type) != null)
                return t.GetComponent(type);
            return FindComponentInParent(t, type);
        }

        public static GameObject FindParentWithTag(GameObject objChild, string tag)
        {
            var t = objChild.transform;
            while (t.parent != null)
            {
                if (t.parent.CompareTag(tag))
                {
                    return t.parent.gameObject;
                }
                t = t.parent.transform;
            }
            return null;
        }

        public static GameObject FindChildWithName(Transform parent, string name)
        {

            for (var i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).name.Equals(name))
                    return parent.GetChild(i).gameObject;
            }
            return null;
        }

        public static GameObject LoadPrefab(string path)
        {
            try
            {
                var go = Resources.Load(path, typeof(GameObject)) as GameObject;
                if (go != null) return go;
                DebugColor.Log("Cannot Load Prefab " + path, ColorName.Red);
                return null;
            }
            catch (Exception e)
            {
                DebugColor.Log("Cannot Load Prefab " + path + " error: " + e.Message, ColorName.Red);
            }
            return null;
        }

        // public static GameObject LoadGameObject(string path, bool resetScale = true) {
        //     return LoadGameObject(null, path, resetScale);
        // }
        //
        // public static GameObject LoadGameObject(GameObject prefab, bool resetScale = true) {
        //     return LoadGameObject(null, prefab, resetScale);
        // }
        public static GameObject LoadGameObject(Transform parent, string path, bool resetScale = true)
        {
            var prefab = LoadPrefab(path);
            if (prefab == null) return null;

            var go = (GameObject)Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            if (resetScale)
                go.transform.localScale = Vector3.one;

            return go;
        }

        public static GameObject LoadGameObject(Transform parent, GameObject prefab, bool resetScale = true)
        {
            if (prefab == null) return null;

            var go = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(true);
            if (resetScale)
                go.transform.localScale = Vector3.one;

            return go;
        }

        public static void SetGameObjectParent(this GameObject go, Transform parent, bool resetTransform = true)
        {
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(true);
        }

    }
}

