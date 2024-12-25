using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class InfoMonitoringManager : MonoBehaviour
    {
        // Singleton instance
        public static InfoMonitoringManager Instance { get; private set; }

        // Events for battery and network updates
        public event Action<string> OnInfoMonitorEvent;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeMonitoring();
        }

        /// <summary>
        /// Initializes monitoring on the platform.
        /// </summary>
        private void InitializeMonitoring()
        {
#if UNITY_ANDROID
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass infoManager = new AndroidJavaClass("com.example.infomonitoring.InfoMonitoringManager");

                infoManager.CallStatic("initialize", currentActivity, gameObject.name);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error initializing Android monitoring: {e.Message}");
            }
#endif
        }

        /// <summary>
        /// Called by the native side to forward updates to Unity.
        /// </summary>
        /// <param name="jsonData">JSON string containing update data.</param>
        public void OnNativeInfoUpdate(string jsonData)
        {
            OnInfoMonitorEvent?.Invoke(jsonData);  //OnInfoMonitorEvent
        }
    }
}