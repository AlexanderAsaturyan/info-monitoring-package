using System;
using InfoMonitoringNamespace.Battery;
using UnityEngine;

namespace InfoMonitoringNamespace
{
    public class InfoMonitoringManager : MonoBehaviour
    {
        [SerializeField] private InfoMonitoringConfig infoMonitoringConfig;

        // Singleton instance
        public static InfoMonitoringManager Instance { get; private set; }

        // Events for battery and network updates
        public event Action<string> OnInfoMonitorEvent;

        public event Action<BaseData> InfoUpdated;

        private BatteryManager _batteryManager;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeManager()
        {
            if (Instance == null)
            {
                GameObject managerObject = new GameObject("InfoMonitoringManager");
                Instance = managerObject.AddComponent<InfoMonitoringManager>();
                DontDestroyOnLoad(managerObject);
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);

            if (infoMonitoringConfig == null)
            {
                infoMonitoringConfig = Resources.Load<InfoMonitoringConfig>("InfoMonitoringConfig");
                if (infoMonitoringConfig == null)
                {
                    Debug.LogError("InfoMonitoringConfig not found in Resources!");
                    return;
                }
            }

            if (infoMonitoringConfig.batteryConfig.TrackEnabled)
            {
                _batteryManager = new BatteryManager(infoMonitoringConfig.batteryConfig);
                _batteryManager.StartBatteryChecks();
            }
        }

        private void OnEnable()
        {
            if (_batteryManager != null)
            {
                _batteryManager.OnBatteryDataUpdated += OnAnyDataChanged;
            }
        }

        private void OnDisable()
        {
            if (_batteryManager != null)
            {
                _batteryManager.OnBatteryDataUpdated -= OnAnyDataChanged;
                _batteryManager.StopBatteryChecks();
            }
        }

        private void Start()
        {
            InitializeMonitoring();
        }

        private void OnAnyDataChanged(BaseData baseData)
        {
            InfoUpdated?.Invoke(baseData);
        }

        /// <summary>
        /// Initializes monitoring on the platform.
        /// </summary>
        public void InitializeMonitoring()
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
            OnInfoMonitorEvent?.Invoke(jsonData); //OnInfoMonitorEvent
        }
    }
}