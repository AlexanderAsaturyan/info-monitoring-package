namespace InfoMonitoringNamespace.Notifications
{
    public class NotificationsData : BaseData
        {
            public override string Category { get; set; } = "Notifications";
            public bool AreEnabled;
        }
}