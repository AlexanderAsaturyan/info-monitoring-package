package com.DefaultCompany.infomonitoring;

import android.content.Context;
import android.app.NotificationManager;
import android.os.Build;
import androidx.core.app.NotificationManagerCompat;

public class NotificationMonitor {
    private Context context;
    
        public NotificationMonitor(Context context) {
            this.context = context;
        }
        
        public boolean areNotificationsEnabled() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
            NotificationManager manager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
            return manager.areNotificationsEnabled();
        } else {
            return NotificationManagerCompat.from(context).areNotificationsEnabled();
        }
    }
}