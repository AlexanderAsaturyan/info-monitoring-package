using System;
using UnityEngine;

namespace InfoMonitoringNamespace
{
    [CreateAssetMenu (fileName = "InfoMonitoringConfig", menuName = "Configs/InfoMonitoring")] 
    public class InfoMonitoringConfig : ScriptableObject
    {
        public BatteryConfig batteryConfig;
    }
   
    [Serializable]
    public class BatteryConfig 
    {
        public bool TrackEnabled = true;
        public int ReportLevel = 2;
    }
}