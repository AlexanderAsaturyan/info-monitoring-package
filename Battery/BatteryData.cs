namespace InfoMonitoringNamespace.Battery
{
    public class BatteryData : BaseData
    {
        public override string Category { get; set; } = "Battery";
        public int Level;
        public string ChargeMode;
        public string ChargeState;
        public int DecayRate;
    }
}