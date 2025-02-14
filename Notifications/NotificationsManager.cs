using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace InfoMonitoringNamespace.Notifications
{
    public class NotificationsManager
    {
        public event Action<BaseData> OnNotificationsDataUpdated;
        
        private NotificationConfig _notificationConfig;
        private NotificationsData _data = new NotificationsData();
        private bool _lastNotificationsEnabledStatus;
        private CancellationTokenSource _cancellationTokenSource;
        
        public NotificationsManager(NotificationConfig notificationConfig)
        {
            _notificationConfig = notificationConfig;
        }

        public async void StartNotificationsCheck()
        {
            _lastNotificationsEnabledStatus = GetNotificationsEnabledStatus();
            _cancellationTokenSource = new CancellationTokenSource();
            await CheckNotificationsEnabledStatusChangeAsync(_cancellationTokenSource.Token);
        }
        
        public void StopNotificationCheck()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
        
        public bool GetNotificationsEnabledStatus()
        {
            try
            {
                using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (var notificationMonitor = new AndroidJavaObject(
                            "com.DefaultCompany.infomonitoring.NotificationMonitor",
                            currentActivity))
                        {
                            return notificationMonitor.Call<bool>("areNotificationsEnabled");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to check notification status: {ex.Message}");
                return false;
            }
        }
        
        private async Task CheckNotificationsEnabledStatusChangeAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    bool currentNotificationsEnabledStatus = GetNotificationsEnabledStatus();
                    if (_lastNotificationsEnabledStatus != currentNotificationsEnabledStatus)
                    {
                        Debug.Log("Notifications Enabled Status Changed!");
                        _data.AreEnabled = currentNotificationsEnabledStatus;
                        OnNotificationsDataUpdated?.Invoke(_data);
                        _lastNotificationsEnabledStatus = currentNotificationsEnabledStatus;
                    }
                    
                    await Task.Delay(5000, cancellationToken);
                }
            }

            catch (TaskCanceledException)
            {
                Debug.LogError("TaskCanceledException Notifications Enabled");
            }

            catch (Exception ex)
            {
                Debug.LogError($"Error in NotificationsEnabledStatus Check: {ex.Message}");
            }
        }
    }
}