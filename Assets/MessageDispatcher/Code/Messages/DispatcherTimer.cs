using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dispatcher
{
    public sealed class DispatcherTimer : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator Start()
        {
            WaitForEndOfFrame lWaitForEndOfFrame = new WaitForEndOfFrame();
            while (true)
            {
                yield return lWaitForEndOfFrame;
                MessageDispatcher.FrameCounter++;
            }
        }

        public void OnDisable()
        {
            MessageDispatcher.ClearMessages();
            MessageDispatcher.ClearListeners();
        }

        private void Update()
        {
            MessageDispatcher.Update();
        }
    }
}