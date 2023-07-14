namespace Core
{
    public class AppConfiguration : BaseEntity
    {
        public AppConfiguration()
        {
            SlotMachine = new SlotMachineConfiguration();
        }

        public SlotMachineConfiguration SlotMachine { get; set; }
    }

    public class SlotMachineConfiguration
    {
        public int SlotHeight { get; set; } = 0;
        public int SlotLength { get; set; } = 0;
    }

}