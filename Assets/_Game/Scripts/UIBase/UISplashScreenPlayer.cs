using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UniRx;

namespace CodeStringers.SplashScreen {
    public class UISplashScreenPlayer : MonoBehaviour {
        [Header("Title")]
        public float timeBeforeShowingTitle = 1f;
        public float titleFadeInDuration = 0.4f;

        [Header("Loading")]
        public float timeBeforeShowLoading = 1.5f;
        public float loadingFadeInDuration = 0.1f;

        [Header("Background")]
        public float fadeTime = 1f;
        [Space]

        public Transform SplashScreen;
        public TextMeshProUGUI txtTitle;
        public Image imgLoading;

        public System.Action OnShowSplashScreen;
        public System.Action OnHideSplashScreen;

        private CanvasGroup canvas;
        private CanvasGroup Canvas {
            get {
                if (canvas == null)
                    canvas = SplashScreen.GetComponent<CanvasGroup>();
                return canvas;
            }
        }

        public void ShowSplashScreen() {
            if (SplashScreen == null) return;
            if (SplashScreen.gameObject.activeSelf == true) return;
            SplashScreen.gameObject.SetActive(true);
            OnShowSplashScreen?.Invoke();

            ResetSplashScreen();
            ShowTitleSequence();
            ShowLoadingSequence();
        }

        public void HideSplashScreen() {
            if (SplashScreen == null) return;

            StopAllCoroutines();

            Canvas.DOFade(0, fadeTime).OnComplete(() => {
                Canvas.blocksRaycasts = false;
                SplashScreen.gameObject.SetActive(false);
                OnHideSplashScreen?.Invoke();
            });
        }

        public bool IsShowingSplashScreen() {
            return SplashScreen != null && SplashScreen.gameObject.activeSelf;
        }

        #region Animation

        private void ResetSplashScreen() {
            txtTitle.alpha = 0;

            var color = imgLoading.color;
            color.a = 0;
            imgLoading.color = color;

            Canvas.DOFade(1, 0);
            Canvas.blocksRaycasts = true;
        }

        private void ShowTitleSequence() {
            Observable.Timer(System.TimeSpan.FromSeconds(timeBeforeShowingTitle)).Subscribe(_ => {
                StartCoroutine(Corou_ShowTitle());
            }).AddTo(this);
        }

        IEnumerator Corou_ShowTitle() {
            var timer = 0f;
            while (txtTitle.alpha < 254) {
                txtTitle.alpha = Mathf.Lerp(0, 1, timer);
                timer += 1 / titleFadeInDuration * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        private void ShowLoadingSequence() {
            Observable.Timer(System.TimeSpan.FromSeconds(timeBeforeShowLoading)).Subscribe(_ => {
                StartCoroutine(Corou_ShowLoading());
            }).AddTo(this);
        }

        IEnumerator Corou_ShowLoading() {
            var timer = 0f;
            float alpha = 0f;
            while (alpha < 254) {
                alpha = Mathf.Lerp(0, 1, timer);
                imgLoading.color = new Color(imgLoading.color.r, imgLoading.color.g, imgLoading.color.b, alpha);
                timer += 1 / loadingFadeInDuration * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion
    }
}

