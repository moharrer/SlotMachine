namespace SlotMachine.Algo
{

    public interface ISlotMachine
    {
        int[,] Spin(int row, int column);
    }

    public class SlotMachine : ISlotMachine
    {
        public int[,] Spin(int rows, int columns)
        {
            if (rows < 0 || columns < 0)
            {
                throw new UserFriendlyException("Invalid Dimention detected.");
            }

            if (rows < 1 && columns < 1)
            {
                return new int[0, 0];
            }

            Random random = new Random();

            // Create a 2D array with random integers
            int[,] array = new int[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    array[i, j] = random.Next(10); // Generate a random integer between 0 and 9
                }
            }

            return array;

        }


    }



}
