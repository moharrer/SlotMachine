namespace SlotMachine.ViewModel
{
    public class SlotMachineResultModel
    {
        public SlotMachineResultModel()
        {
            Result = new int[0, 0];
        }
        public SlotMachineResultModel(int[,] Result, decimal Win, decimal Balance)
        {
            this.Result = Result;
            this.Win = Win;
            this.Balance = Balance;
        }

        public int[,] Result { get; set; }
        public decimal Win { get; set; }
        public decimal Balance { get; set; }
    }
}
