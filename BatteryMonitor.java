package com.DefaultCompany.infomonitoring;

import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.BatteryManager;
import android.os.PowerManager;

public class BatteryMonitor {
    private Context context;
    
    public BatteryMonitor(Context context) {
        this.context = context;
    }
    
    public int getBatteryLevel() {
        IntentFilter intentFilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
        Intent batteryStatus = context.registerReceiver(null, intentFilter);

        if (batteryStatus != null) {
            int level = batteryStatus.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
            int scale = batteryStatus.getIntExtra(BatteryManager.EXTRA_SCALE, -1);
            return (int) ((level / (float) scale) * 100); 
        }
        return -1; 
    }
    
    public String getBatteryChargeState() {
        IntentFilter intentFilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
        Intent batteryStatus = context.registerReceiver(null, intentFilter);

        if (batteryStatus != null) {
            int status = batteryStatus.getIntExtra(BatteryManager.EXTRA_STATUS, -1);

            switch (status) {
                case BatteryManager.BATTERY_STATUS_CHARGING:
                    return "Charging";
                case BatteryManager.BATTERY_STATUS_FULL:
                    return "Full";
                case BatteryManager.BATTERY_STATUS_DISCHARGING:
                    return "Discharging";
                case BatteryManager.BATTERY_STATUS_NOT_CHARGING:
                    return "Not Charging";
                default:
                    return "Unknown";
            }
        }                                                                                                                                                                  
        return "Error: Unable to determine battery state";
    }

    
    public String getBatteryChargeMode() {
        IntentFilter intentFilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
                Intent batteryStatus = context.registerReceiver(null, intentFilter);
        
                if (batteryStatus != null) {
                    int chargePlug = batteryStatus.getIntExtra(BatteryManager.EXTRA_PLUGGED, -1);
        
                    switch (chargePlug) {
                        case BatteryManager.BATTERY_PLUGGED_USB:
                            return "USB";
                        case BatteryManager.BATTERY_PLUGGED_AC:
                            return "AC";
                        case BatteryManager.BATTERY_PLUGGED_WIRELESS:
                            return "Wireless";
                        default:
                            return "Not Charging";
                    }
                } else {
                    return "Unknown";
                }
    }
}
