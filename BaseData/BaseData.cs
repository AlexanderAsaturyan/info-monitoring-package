namespace DefaultNamespace
{
    public class BaseData
    {
        public virtual string Category { get; set;}
    }

    public class BatteryData : BaseData
    {
        public override string Category = "Battery";
        public int Level;
        public string Type;
        public string State;
        public int DecayRate;
    }
}