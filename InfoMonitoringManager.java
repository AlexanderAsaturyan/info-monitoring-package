package com.example.infomonitoring;

import android.content.Context;
import com.unity3d.player.UnityPlayer;

public class InfoMonitoringManager {

    public static void initialize(Context context, String unityObjectName) {
        try {
            String batteryStatus = BatteryMonitor.getBatteryStatus(context);
          //  String networkStatus = NetworkMonitor.getNetworkStatus(context);

            UnityPlayer.UnitySendMessage(unityObjectName, "OnNativeInfoUpdate", batteryStatus);
          //    UnityPlayer.UnitySendMessage(unityObjectName, "OnNativeInfoUpdate", networkStatus);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
