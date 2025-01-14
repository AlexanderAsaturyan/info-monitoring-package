using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace InfoMonitoringNamespace.Battery
{
    public class BatteryManager 
    {
        public event Action<BaseData> OnBatteryDataUpdated;

        private BatteryConfig _batteryConfig;
        private BatteryData _data = new BatteryData();

        private int _lastBatteryLevel;
        private string _lastBatteryChargeMode;
        private string _lastBatteryChargeState;

        private CancellationTokenSource _cancellationTokenSource;

        public BatteryManager(BatteryConfig batteryConfig)
        {
            _batteryConfig = batteryConfig;
        }

        // private void OnEnable()
        // {
        //     //StartBatteryChecks();
        //     _cancellationTokenSource = new CancellationTokenSource();
        //     _ = CheckBatteryLevelChangeAsync(_cancellationTokenSource.Token);
        // }

        // private void OnDisable()
        // {
        //     if (_cancellationTokenSource != null)
        //     {
        //         _cancellationTokenSource.Cancel();
        //         _cancellationTokenSource.Dispose();
        //         _cancellationTokenSource = null;
        //     }
        //
        //     StopBatteryChecks();
        // }

        public async void StartBatteryChecks()
        {
            _data.Category = "Battery";
            _lastBatteryLevel = GetBatteryLevel();
            _lastBatteryChargeMode = GetBatteryChargeMode();
            _lastBatteryChargeState = GetBatteryChargeState();

            _cancellationTokenSource = new CancellationTokenSource();
            var batteryLevelTask = CheckBatteryLevelChangeAsync(_cancellationTokenSource.Token);
            var batteryModeTask  = CheckBatteryChargeModeChangeAsync(_cancellationTokenSource.Token);
            var batteryStateTask = CheckBatteryChargeStateChangeAsync(_cancellationTokenSource.Token);

            await Task.WhenAll(batteryLevelTask, batteryModeTask, batteryStateTask);
        }

        public void StopBatteryChecks()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        // private async Task CheckBatteryLevelChangeAsync()
        // {
        //     while (true)
        //     {
        //         int currentBatteryLevel = GetBatteryLevel();
        //         if (Mathf.Abs(_lastBatteryLevel - currentBatteryLevel) == _batteryConfig.ReportLevel)
        //         {
        //             Debug.LogError($"Battery Level Changed by {_batteryConfig.ReportLevel}!");
        //             _data.Level = currentBatteryLevel;
        //             OnBatteryDataUpdated?.Invoke(_data);
        //             _lastBatteryLevel = currentBatteryLevel;
        //         }
        //
        //         await Task.Delay(1000);
        //     }
        // }

        private async Task TestLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Debug.LogError("iteration until cancel");
                }

                await Task.Delay(3000, cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.LogError("ex: " + ex.Message);
            }
        }

        private async Task CheckBatteryLevelChangeAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int currentBatteryLevel = GetBatteryLevel();
                    if (Mathf.Abs(_lastBatteryLevel - currentBatteryLevel) == _batteryConfig.ReportLevel)
                    {
                        Debug.Log($"Battery Level Changed by {_batteryConfig.ReportLevel}%!");
                        _data.Level = currentBatteryLevel;
                        OnBatteryDataUpdated?.Invoke(_data);
                        _lastBatteryLevel = currentBatteryLevel;
                    }
                    
                    await Task.Delay(5000, cancellationToken);
                }
            }

            catch (TaskCanceledException)
            {
                Debug.LogError("TaskCanceledException level");
            }

            catch (Exception ex)
            {
                Debug.LogError($"Error in Battery Level Check: {ex.Message}");
            }
        }

        private async Task CheckBatteryChargeModeChangeAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    string currentBatteryChargeMode = GetBatteryChargeMode();
                    if (_lastBatteryChargeMode != currentBatteryChargeMode)
                    {
                        Debug.Log("Battery Charge Mode Changed!");
                        _data.ChargeMode = currentBatteryChargeMode;
                        OnBatteryDataUpdated?.Invoke(_data);
                        _lastBatteryChargeMode = currentBatteryChargeMode;
                    }
                    
                    await Task.Delay(5000, cancellationToken);
                }
            }

            catch (TaskCanceledException)
            {
                Debug.LogError("TaskCanceledException Mode");
            }

            catch (Exception ex)
            {
                Debug.LogError($"Error in Charge Mode Check: {ex.Message}");
            }
        }

        private async Task CheckBatteryChargeStateChangeAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    string currentBatteryChargeState = GetBatteryChargeState();
                    if (_lastBatteryChargeState != currentBatteryChargeState)
                    {
                        Debug.Log("Battery Charge State Changed!");
                        _data.ChargeState = currentBatteryChargeState;
                        OnBatteryDataUpdated?.Invoke(_data);
                        _lastBatteryChargeState = currentBatteryChargeState;
                    }
                    
                    await Task.Delay(5000, cancellationToken);
                }
            }

            catch (TaskCanceledException)
            {
                Debug.LogError("TaskCanceledException State");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in Charge State Check: {ex.Message}");
            }
        }

        public int GetBatteryLevel()
        {
#if UNITY_ANDROID
            try
            {
                using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (var batteryPlugin = new AndroidJavaObject(
                            "com.DefaultCompany.infomonitoring.BatteryMonitor",
                            currentActivity))
                        {
                            return batteryPlugin.Call<int>("getBatteryLevel");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get battery level: {ex.Message}");
                return -1;
            }
#endif
            return -1;
        }

        public string GetBatteryChargeState()
        {
#if UNITY_ANDROID
            try
            {
                using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (var batteryPlugin = new AndroidJavaObject(
                            "com.DefaultCompany.infomonitoring.BatteryMonitor",
                            currentActivity))
                        {
                            return batteryPlugin.Call<string>("getBatteryChargeState");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get battery charge state: {ex.Message}");
                return "Unknown";
            }
#endif
            return "Unknown";
        }

        public string GetBatteryChargeMode()
        {
#if UNITY_ANDROID
            try
            {
                using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (var batteryPlugin = new AndroidJavaObject(
                            "com.DefaultCompany.infomonitoring.BatteryMonitor",
                            currentActivity))
                        {
                            return batteryPlugin.Call<string>("getBatteryChargeMode");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get battery charge mode: {ex.Message}");
                return "Unknown";
            }
#endif
            return "Unknown";
        }
    }
}